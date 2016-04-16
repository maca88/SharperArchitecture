using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using FluentValidation;
using FluentValidation.Validators;
using NHibernate;
using PowerArhitecture.Breeze.Metadata;
using PowerArhitecture.Breeze.Specification;
using PowerArhitecture.Domain;
using PowerArhitecture.Validation;
using PowerArhitecture.Validation.Specifications;
using FluentValidation.Internal;
using PowerArhitecture.Breeze.Attributes;
using PowerArhitecture.Common.Exceptions;
using PowerArhitecture.Domain.Attributes;

namespace PowerArhitecture.Breeze
{
    public class BreezeMetadataConfigurator
    {
        private readonly IValidatorFactoryExtended _validatorFactory;
        private readonly object _lock = new object();

        public BreezeMetadataConfigurator(IValidatorFactoryExtended validatorFactory)
        {
            _validatorFactory = validatorFactory;
        }

        private void SetFluentValidators(DataProperty dataProp, StructuralType structuralType)
        {
            var nullableProps = new HashSet<string> {"LastModifiedById", "CreatedById", "LastModifiedDate", "CreatedDate"};
            var toReplace = new HashSet<string> {"required", "maxLength"};
            var entityType = structuralType as EntityType;
            foreach (var validator in dataProp.Validators.Where(o => toReplace.Contains(o.Name)).ToList())
            {
                dataProp.Validators.Remove(validator);
                Validator newValidator = null;
                var name = validator.Name;
                /*
                if (name == "required" && nullableProps.Contains(dataProp.NameOnServer))
                {
                    dataProp.IsNullable = true;
                    continue;
                }*/
                    
                switch (name)
                {
                    case "required":
                        //Check if the property is a foreignKey if it is then default type value is not valid
                        if (entityType != null && !string.IsNullOrEmpty(dataProp.NameOnServer) &&
                            entityType.NavigationProperties
                                .Any(o => o.ForeignKeyNamesOnServer != null && o.ForeignKeyNamesOnServer
                                    .Any(fk => fk == dataProp.NameOnServer)))
                        {
                            newValidator = new Validator {Name = "fvNotEmpty"};
                            var defVal = dataProp.PropertyInfo != null
                                ? dataProp.PropertyInfo.PropertyType.GetDefaultValue()
                                : null;
                            newValidator.MergeLeft(FluentValidators.GetParamaters(new NotEmptyValidator(defVal)));
                        }
                        else
                        {
                            newValidator = new Validator { Name = "fvNotNull" };
                            newValidator.MergeLeft(FluentValidators.GetParamaters(new NotNullValidator()));
                        }
                        break;
                    case "maxLength":
                        newValidator = new Validator {Name = "fvLength"};
                        newValidator.MergeLeft(FluentValidators.GetParamaters(new LengthValidator(0, dataProp.MaxLength)));
                        break;
                }
                dataProp.Validators.Add(newValidator);
            }
        }


        public void Configure(IDictionary<string, object> metadata, ISessionFactory sessionFactory)
        {
            lock (_lock)
            {
                if (metadata.ContainsKey("processed")) return;

                var metadataSchema = new MetadataSchema(metadata);
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                var clientModelTypes = assemblies
                    .Where(a => a.GetTypes().Any(t => typeof(IClientModel).IsAssignableFrom(t)))
                    .SelectMany(a => a.GetTypes().Where(t => !t.IsAbstract && t.IsClass && !t.IsGenericType && typeof(IClientModel).IsAssignableFrom(t)))
                    .ToList();
                var serverModelTypes = assemblies
                    .Where(a => a.GetTypes().Any(t => typeof(IEntity).IsAssignableFrom(t) || t.GetCustomAttribute<IncludeAttribute>() != null))
                    .SelectMany(a => a.GetTypes().Where(t => typeof(IEntity).IsAssignableFrom(t) || t.GetCustomAttribute<IncludeAttribute>() != null))
                    .ToList();
                var allTypesDict = serverModelTypes.Union(clientModelTypes).Distinct().ToDictionary(o => o.FullName);

                Func<Type, EntityType> getOrCreateEntityType = (type) =>
                {
                    var entityType =
                        metadataSchema.StructuralTypes.FirstOrDefault(o => o.TypeFullName == type.FullName) as EntityType;
                    if (entityType != null) return entityType;
                    //Create new structure as entityType with a fake id property
                    entityType =
                        new EntityType(type)
                        {
                            AutoGeneratedKeyType = AutoGeneratedKeyType.KeyGenerator
                        };
                    metadataSchema.StructuralTypes.Add(entityType);
                    return entityType;
                };

                Func<Type, ComplexType> getOrCreateComplexType = (type) =>
                {
                    var complexType =
                        metadataSchema.StructuralTypes.FirstOrDefault(o => o.TypeFullName == type.FullName) as ComplexType;
                    if (complexType != null) return complexType;
                    //Create new structure as complexType
                    complexType =
                        new ComplexType(type);
                    metadataSchema.StructuralTypes.Add(complexType);
                    return complexType;
                };

                //Add client models to metadata
                foreach (var clientModelType in clientModelTypes)
                {
                    var structType = clientModelType.GetCustomAttribute<ComplexTypeAttribute>() != null
                        ? (StructuralType)getOrCreateComplexType(clientModelType)
                        : getOrCreateEntityType(clientModelType);

                    foreach (var prop in clientModelType.GetProperties())
                    {
                        var isEntityType = typeof (IEntity).IsAssignableFrom(prop.PropertyType);
                        var isClientType = typeof (IClientModel).IsAssignableFrom(prop.PropertyType);

                        //add a nav property
                        if (isEntityType || isClientType)
                        {
                            var syntheticPropName = prop.Name + "Id";
                            DataType dataType;
                            var isComplexType = false;
                            if (isEntityType)
                            {
                                var classMetadata = sessionFactory.GetClassMetadata(prop.PropertyType);
                                dataType = BreezeTypeHelper.GetDataType(classMetadata.IdentifierType.ReturnedClass);
                            }
                            else
                            {
                                //Check if the client entity is a complexType
                                isComplexType = prop.PropertyType.GetCustomAttribute<ComplexTypeAttribute>() != null;
                                dataType = BreezeTypeHelper.GetDataType(typeof (long));
                            }

                            DataProperty property;

                            if (isComplexType)
                            {
                                property = new DataProperty
                                {
                                    ComplexTypeName = string.Format("{0}:#{1}", prop.PropertyType.Name, prop.PropertyType.Namespace),
                                    NameOnServer = prop.Name,
                                    IsNullable = true,
                                };
                                structType.DataProperties.Add(property);
                                continue;
                            }

                            var entityType = structType as EntityType;
                            if (entityType == null)
                                throw new PowerArhitectureException(
                                    "Invalid definition of property '{0}' inside complex type '{1}'. Complex types can not have navigation properties as it is not supported by breeze",
                                    prop.Name,
                                    clientModelType.FullName);

                            property = new DataProperty
                            {
                                DataType = dataType,
                                NameOnServer = syntheticPropName,
                                IsNullable = true,
                            };
                            entityType.DataProperties.Add(property);
                            var navProp = new NavigationProperty
                            {
                                IsScalar = true,
                                EntityTypeName = string.Format("{0}:#{1}", prop.PropertyType.Name, prop.PropertyType.Namespace),
                                NameOnServer = prop.Name,
                                AssociationName = string.Format("AN_{0}_{1}_{2}", prop.PropertyType.Name, prop.Name, syntheticPropName),
                                ForeignKeyNamesOnServer = new List<string> { syntheticPropName }
                            };
                            entityType.NavigationProperties.Add(navProp);
                            metadataSchema.FkMap[string.Format("{0}.{1}", clientModelType.FullName, prop.Name)] = syntheticPropName;
                        }
                        else if (
                            typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) &&
                            prop.PropertyType.IsGenericType && 
                            prop.PropertyType.GetGenericArguments().Length == 1)
                        {
                            var invEntityType = prop.PropertyType.GetGenericArguments()[0];
                            if(!typeof(IEntity).IsAssignableFrom(invEntityType) && !typeof(IClientModel).IsAssignableFrom(invEntityType))
                                continue;

                            //We need to find the related property on the otherside of the relation
                            var invPropNameAttr = prop.GetCustomAttribute<InversePropertyName>();
                            var invPropName = invPropNameAttr != null ? invPropNameAttr.PropertyName : null;

                            var invProperties = string.IsNullOrEmpty(invPropName)
                                ? invEntityType.GetProperties().Where(o => o.PropertyType == clientModelType).ToList()
                                : invEntityType.GetProperties().Where(o => o.Name == invPropName).ToList();

                            if (!invProperties.Any() && !string.IsNullOrEmpty(invPropName))
                                throw new PowerArhitectureException("Inverse property name '{0}' was not found in type '{1}'.", invPropName, invEntityType.FullName);

                            if (invProperties.Count > 1)
                                continue;

                            var entityType = structType as EntityType;
                            if (entityType == null)
                                throw new PowerArhitectureException(
                                    "Invalid definition of property '{0}' inside type '{1}'. Generic argument '{2}' must not be a Complex type",
                                    prop.Name,
                                    clientModelType.FullName,
                                    invEntityType.FullName);

                            string invPropertyName;
                            string invSyntheticPropName;

                            if (!invProperties.Any())
                            {
                                invPropertyName = clientModelType.Name;
                                invSyntheticPropName = invPropertyName + "Id";
                                //We need to create a DataProperty for the foreignKey
                                var property = new DataProperty
                                {
                                    DataType = BreezeTypeHelper.GetDataType(typeof(long)),
                                    NameOnServer = invSyntheticPropName,
                                    IsNullable = true
                                };
                                var invStructType = getOrCreateEntityType(invEntityType);
                                invStructType.DataProperties.Add(property);
                            }
                            else
                            {
                                var invProperty = invProperties.First();
                                invSyntheticPropName = invProperty.Name + "Id";
                                invPropertyName = invProperty.Name;
                            }

                            var navProp = new NavigationProperty
                            {
                                IsScalar = false,
                                EntityTypeName = string.Format("{0}:#{1}", invEntityType.Name, invEntityType.Namespace),
                                NameOnServer = prop.Name,
                                AssociationName = string.Format("AN_{0}_{1}_{2}", clientModelType.Name, invPropertyName, invSyntheticPropName),
                                InvForeignKeyNamesOnServer = new List<string> { invSyntheticPropName }
                            };
                            entityType.NavigationProperties.Add(navProp);
                        }
                        else
                        {
                            var dataProp = new DataProperty(prop)
                            {
                                IsPartOfKey = prop.Name == "Id"
                            };
                            var defAttr = prop.GetCustomAttribute<DefaultValueAttribute>();
                            if (defAttr != null)
                            {
                                dataProp.DefaultValue = defAttr.Value;
                            }
                            structType.DataProperties.Add(dataProp);
                        }
                    }
                    structType["isUnmapped"] = true;
                }

                foreach (var structType in metadataSchema.StructuralTypes.Where(o => allTypesDict.ContainsKey(o.TypeFullName)))
                {
                    var type = allTypesDict[structType.TypeFullName];
                    var modelValidator = _validatorFactory.GetValidator(type) as IEnumerable<IValidationRule>;
                    if(modelValidator == null) continue;
                    var membersRules = modelValidator.OfType<PropertyRule>()
                        .Where(o => o.PropertyName != null)
                        .ToLookup(o => o.PropertyName, o => o);

                    foreach (var dataProp in structType.DataProperties)
                    {
                        SetFluentValidators(dataProp, structType);
                        var propRules = membersRules[dataProp.NameOnServer];
                        foreach (var propRule in propRules.Where(o => o.RuleSet == null || ValidationRuleSet.AttributeInsertUpdateDefault.Contains(o.RuleSet)))
                        {
                            var currVal = propRule.CurrentValidator;
                            var name = FluentValidators.GetName(currVal);
                            if (name == null) continue; //add only registered validators
                            dataProp.Validators.Remove(name);
                            var validator = new Validator { Name = name };
                            validator.MergeLeft(FluentValidators.GetParamaters(currVal));
                            dataProp.Validators.Add(validator);
                        }
                    }
                    //This is not the right approach as we then have to load all navigation properties in order to save the entity
                    //var entityType = structType as EntityType;
                    //if (entityType == null) continue;
                    //foreach (var navProp in entityType.NavigationProperties)
                    //{
                    //    var source = navProp.ForeignKeyNamesOnServer ?? new List<string>();
                    //    var fkPropName = source.FirstOrDefault();
                    //    if (fkPropName == null) 
                    //        continue; //null for inverse fks

                    //    var prop = entityType.DataProperties.First(o => (o.NameOnServer == fkPropName));
                    //    if (prop.IsNullable) continue;
                    //    var notNullVal = new Validator
                    //    {
                    //        Name = "fvNotNull"
                    //    };
                    //    notNullVal.MergeLeft(FluentValidators.GetParamaters(new NotNullValidator()));
                    //    navProp.Validators.Add(notNullVal);
                    //}
                }

                metadata["processed"] = true;                
            }
        }
    }
}