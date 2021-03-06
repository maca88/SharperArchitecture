﻿using System.Linq;
using System.Reflection;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Steps;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.Utils;

namespace SharperArchitecture.DataAccess.MappingSteps
{
    public class CustomPropertyStep : IAutomappingStep
    {
        private readonly IConventionFinder conventionFinder;
        private readonly IAutomappingConfiguration cfg;

        public CustomPropertyStep(IConventionFinder conventionFinder, IAutomappingConfiguration cfg)
        {
            this.conventionFinder = conventionFinder;
            this.cfg = cfg;
        }

        public bool ShouldMap(Member member)
        {
            if (HasExplicitTypeConvention(member))
                return true;

            return IsMappableToColumnType(member);
        }

        private bool HasExplicitTypeConvention(Member property)
        {
            // todo: clean this up!
            //        What it's doing is finding if there are any IUserType conventions
            //        that would be applied to this property, if there are then we should
            //        definitely automap it. The nasty part is that right now we don't have
            //        a model, so we're having to create a fake one so the convention will
            //        apply to it.
            var conventions = conventionFinder
                .Find<IPropertyConvention>()
                .Where(c =>
                {
                    if (!typeof(IUserTypeConvention).IsAssignableFrom(c.GetType()))
                        return false;

                    var criteria = new ConcreteAcceptanceCriteria<IPropertyInspector>();
                    var acceptance = c as IConventionAcceptance<IPropertyInspector>;

                    if (acceptance != null)
                        acceptance.Accept(criteria);

                    var propertyMapping = new PropertyMapping
                    {
                        Member = property
                    };
                    propertyMapping.Set(x => x.Type, Layer.Defaults, new TypeReference(property.PropertyType));
                    return criteria.Matches(new PropertyInspector(propertyMapping));
                });

            return conventions.FirstOrDefault() != null;
        }

        private static bool IsMappableToColumnType(Member property)
        {
            return property.PropertyType.Namespace == "System"
                    || property.PropertyType.FullName == "System.Drawing.Bitmap"
                    || property.PropertyType.IsEnum;
        }

        public void Map(ClassMappingBase classMap, Member member)
        {
            if (member.DeclaringType != classMap.Type) //Inherited member
            {
                var declaredMember = classMap.Type.GetMember(member.Name,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);
                if(declaredMember.Any()) 
                    return; //Overriden member
            }
            classMap.AddProperty(GetPropertyMapping(classMap.Type, member));
        }

        private PropertyMapping GetPropertyMapping(System.Type type, Member property)
        {
            var mapping = new PropertyMapping
            {
                ContainingEntityType = type,
                Member = property
            };

            var columnMapping = new ColumnMapping();
            columnMapping.Set(x => x.Name, Layer.Defaults, property.Name);
            mapping.AddColumn(Layer.Defaults, columnMapping);

            mapping.Set(x => x.Name, Layer.Defaults, mapping.Member.Name);
            mapping.Set(x => x.Type, Layer.Defaults, GetDefaultType(property));

            SetDefaultAccess(property, mapping);

            return mapping;
        }

        void SetDefaultAccess(Member member, PropertyMapping mapping)
        {
            var resolvedAccess = MemberAccessResolver.Resolve(member);

            if (resolvedAccess != Access.Property && resolvedAccess != Access.Unset)
            {
                // if it's a property or unset then we'll just let NH deal with it, otherwise
                // set the access to be whatever we determined it might be
                mapping.Set(x => x.Access, Layer.Defaults, resolvedAccess.ToString());
            }

            if (member.IsProperty && !member.CanWrite)
            {
                mapping.Set(x => x.Access, Layer.Defaults, cfg.GetAccessStrategyForReadOnlyProperty(member).ToString());
            }
        }

        static TypeReference GetDefaultType(Member property)
        {
            var type = new TypeReference(property.PropertyType);

            if (property.PropertyType.IsEnum())
                type = new TypeReference(typeof(GenericEnumMapper<>).MakeGenericType(property.PropertyType));

            if (property.PropertyType.IsNullable() && property.PropertyType.IsEnum())
                type = new TypeReference(typeof(GenericEnumMapper<>).MakeGenericType(property.PropertyType.GetGenericArguments()[0]));

            return type;
        }

    }
}