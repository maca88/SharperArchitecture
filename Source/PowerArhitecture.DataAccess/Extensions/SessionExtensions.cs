using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using log4net.Plugin;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using PowerArhitecture.DataAccess.Wrappers;
using PowerArhitecture.Domain;
using Iesi.Collections;
using NHibernate.Metadata;
using NHibernate.Proxy;
using NHibernate.Type;
using Remotion.Linq.Clauses;

namespace NHibernate
{
    public interface IDeepCopyMemberOptions
    {
        string MemberName { get; set; }

        Func<object, object> ResolveUsing { get; set; }

        bool IsResolveUsingSet { get; }

        bool Ignore { get; set; }

        bool CloneAsReference { get; set; }
    }

    public class DeepCopyMemberOptions : IDeepCopyMemberOptions
    {
        public string MemberName { get; set; }

        public Func<object, object> ResolveUsing { get; set; }

        public bool IsResolveUsingSet { get; set; }

        public bool Ignore { get; set; }

        public bool CloneAsReference { get; set; }
    }

    public interface IDeepCopyMemberOptions<TType>
    {
        IDeepCopyMemberOptions<TType> ResolveUsing(Func<TType, object> func);

        IDeepCopyMemberOptions<TType> Ignore(bool value = true);

        IDeepCopyMemberOptions<TType> CloneAsReference(bool value = true);
    }

    public class DeepCopyMemberOptions<TType> : DeepCopyMemberOptions, IDeepCopyMemberOptions<TType>
    {
        public IDeepCopyMemberOptions<TType> ResolveUsing(Func<TType, object> func)
        {
            base.ResolveUsing = o => func((TType) o);
            IsResolveUsingSet = true;
            return this;
        }

        public IDeepCopyMemberOptions<TType> Ignore(bool value = true)
        {
            base.Ignore = value;
            return this;
        }

        public IDeepCopyMemberOptions<TType> CloneAsReference(bool value = true)
        {
            base.CloneAsReference = value;
            return this;
        }
    }

    public interface IDeepCopyTypeOptions
    {
        bool? CloneIdentifier { get; set; }
    }

    public class DeepCopyTypeOptions : IDeepCopyTypeOptions
    {
        public DeepCopyTypeOptions(System.Type type)
        {
            Members = new Dictionary<string, DeepCopyMemberOptions>();
            CloneIdentifier = true;
            Type = type;
        }

        public System.Type Type { get; set; }

        public Dictionary<string, DeepCopyMemberOptions> Members { get; set; }

        public bool? CloneIdentifier { get; set; }
    }

    public interface IDeepCopyTypeOptions<TType>
    {
        IDeepCopyTypeOptions<TType> CloneIdentifier(bool value);

        IDeepCopyTypeOptions<TType> ForMember<TMember>(Expression<Func<TType, TMember>> memberExpr,
            Action<IDeepCopyMemberOptions<TType>> action);
    }

    public class DeepCopyTypeOptions<TType> : DeepCopyTypeOptions, IDeepCopyTypeOptions<TType>
    {
        public DeepCopyTypeOptions() : base(typeof(TType))
        {
        }

        public IDeepCopyTypeOptions<TType> CloneIdentifier(bool value)
        {
            base.CloneIdentifier = value;
            return this;
        }

        public IDeepCopyTypeOptions<TType> ForMember<TMember>(Expression<Func<TType, TMember>> memberExpr, 
            Action<IDeepCopyMemberOptions<TType>> action)
        {
            var memberName = memberExpr.GetFullPropertyName();
            if (!Members.ContainsKey(memberName))
                Members.Add(memberName, new DeepCopyMemberOptions<TType>
                {
                    MemberName = memberName
                });
            action(Members[memberName] as IDeepCopyMemberOptions<TType>);
            return this;
        }
    }

    public class DeepCopyOptions
    {
        public DeepCopyOptions()
        {
            CloneIdentifierValue = true;
            TypeOptions = new Dictionary<System.Type, DeepCopyTypeOptions>();
        }

        internal bool CloneIdentifierValue { get; set; }

        internal bool UseSessionLoadFunction { get; set; }

        internal Func<System.Type, bool> CanCloneAsReferenceFunc { get; set; }

        internal Dictionary<System.Type, DeepCopyTypeOptions> TypeOptions { get; set; } 

        public DeepCopyOptions CloneIdentifier(bool value)
        {
            CloneIdentifierValue = value;
            return this;
        }

        public DeepCopyOptions UseSessionLoad(bool value = true)
        {
            UseSessionLoadFunction = value;
            return this;
        }

        public DeepCopyOptions CanCloneAsReference(Func<System.Type, bool> func)
        {
            CanCloneAsReferenceFunc = func;
            return this;
        }

        public DeepCopyOptions ForType<TType>(Action<IDeepCopyTypeOptions<TType>> action)
        {
            var type = typeof (TType);
            if (!TypeOptions.ContainsKey(typeof (TType)))
            {
                var typeOpts = new DeepCopyTypeOptions<TType>();
                typeOpts.CloneIdentifier(CloneIdentifierValue);
                TypeOptions.Add(type, typeOpts);
            }
            action(TypeOptions[type] as IDeepCopyTypeOptions<TType>);
            return this;
        }

        private readonly Dictionary<System.Type, HashSet<string>> _cachedIgnoredMembersResults = new Dictionary<System.Type, HashSet<string>>();

        internal HashSet<string> GetIgnoreMembers(System.Type type)
        {
            if (_cachedIgnoredMembersResults.ContainsKey(type)) return _cachedIgnoredMembersResults[type];
            var result = new HashSet<string>();
            var pairs = TypeOptions.Where(pair => pair.Key.IsAssignableFrom(type)).ToList();
            if (pairs.Any())
            {
                //subclasses have higher priority
                pairs.Sort((pair, valuePair) => pair.Key.IsAssignableFrom(valuePair.Key) ? -1 : 1);
                foreach (var member in pairs
                    .Select(o => o.Value)
                    .SelectMany(o => o.Members)
                    .Select(o => o.Value))
                {
                    if (member.Ignore)
                        result.Add(member.MemberName);
                    else
                        result.Remove(member.MemberName);
                }
            }
            _cachedIgnoredMembersResults.Add(type, result);
            return result;
        }

        private readonly Dictionary<System.Type, Dictionary<string, Func<object, object>>> _cachedResolveFunctions =
            new Dictionary<System.Type, Dictionary<string, Func<object, object>>>();

        internal Func<object, object> GetResolveFunction(System.Type type, string propName)
        {
            if (_cachedResolveFunctions.ContainsKey(type) && _cachedResolveFunctions[type].ContainsKey(propName))
                return _cachedResolveFunctions[type][propName];

            var pairs = TypeOptions.Where(pair => pair.Key.IsAssignableFrom(type)).ToList();
            Func<object, object> result = null;
            if (pairs.Any())
            {
                //subclasses have higher priority
                pairs.Sort((pair, valuePair) => pair.Key.IsAssignableFrom(valuePair.Key) ? -1 : 1);
                foreach (var memberOpt in pairs
                    .Select(o => o.Value)
                    .SelectMany(o => o.Members)
                    .Select(o => o.Value)
                    .Where(o => o.MemberName == propName && o.IsResolveUsingSet))
                {
                    result = memberOpt.ResolveUsing;
                }
            }

            if (!_cachedResolveFunctions.ContainsKey(type))
                _cachedResolveFunctions.Add(type, new Dictionary<string, Func<object, object>>());
            if (!_cachedResolveFunctions[type].ContainsKey(propName))
                _cachedResolveFunctions[type].Add(propName, null);

            _cachedResolveFunctions[type][propName] = result;
            return result;
        }

        internal bool CanCloneIdentifier(System.Type entityType)
        {
            return TypeOptions.ContainsKey(entityType) && TypeOptions[entityType].CloneIdentifier.HasValue
                ? TypeOptions[entityType].CloneIdentifier.Value
                : CloneIdentifierValue;
        }

        internal bool CanCloneAsReference(System.Type entityType, string propertyName)
        {
            if (!TypeOptions.ContainsKey(entityType) || !TypeOptions[entityType].Members.ContainsKey(propertyName)) return false;
            return TypeOptions[entityType].Members[propertyName].CloneAsReference;
        }
    }

    public class DeepCopyParentEntity
    {
        public object Entity { get; set; }

        public AbstractEntityPersister EntityPersister { get; set; }

        public IType ChildType { get; set; }

        public string[] ReferencedColumns { get; set; }
    }

    public static class SessionExtensions
    {
        private static readonly PropertyInfo _isAlreadyDisposedPropInfo;

        static SessionExtensions()
        {
            _isAlreadyDisposedPropInfo = typeof (SessionImpl).GetProperty("IsAlreadyDisposed", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static IEnumerable<T> DeepCopy<T>(this ISession session, IEnumerable<T> entities, DeepCopyOptions opts = null) 
            where T : IEntity
        {
            var resolvedEntities = new Dictionary<object, object>();
            return entities.Select(entity => session.DeepCopy(entity, opts, resolvedEntities)).ToList();
        }

        public static bool IsAlreadyDisposed(this ISession session)
        {
            var sessionWrapper = session as SessionWrapper;
            if (sessionWrapper != null)
                session = sessionWrapper.Session;
            return (bool)_isAlreadyDisposedPropInfo.GetValue(session);
        }

        public static T DeepCopy<T>(this ISession session, T entity, DeepCopyOptions opts = null) where T : IEntity
        {
            // forward to resolver
            return (T)session.DeepCopy(entity, opts, entity.GetType(), new Dictionary<object, object>());
        }

        public static IEnumerable DeepCopy(this ISession session, IEnumerable entities, DeepCopyOptions opts = null)
        {
            var collection = (IEnumerable)CreateNewCollection(entities.GetType());
            var resolvedEntities = new Dictionary<object, object>();
            foreach (var entity in entities)
            {
                AddItemToCollection(collection, session.DeepCopy(entity, opts, entity.GetType(), resolvedEntities));
            }
            return collection;
        }

        private static T DeepCopy<T>(this ISession session, T entity, DeepCopyOptions opts, IDictionary<object, object> resolvedEntities) 
            where T : IEntity
        {
            return (T)session.DeepCopy(entity, opts, GetUnproxiedType(entity), resolvedEntities);
        }

        private static object CopyOnlyForeignKeyProperties(object entity, System.Type entityType,
            AbstractEntityPersister entityMetadata, DeepCopyOptions opts, DeepCopyParentEntity parentEntity)
        {
            var propertyInfos = entityType.GetProperties();

            //Copy only Fks
            foreach (var propertyInfo in propertyInfos
                .Where(p => opts.CanCloneIdentifier(entityType) || entityMetadata.IdentifierPropertyName != p.Name)
                .Where(p => !opts.GetIgnoreMembers(entityType).Contains(p.Name))
                .Where(p => p.GetSetMethod(true) != null))
            {
                IType entityPropertyType;
                try
                {
                    entityPropertyType = entityMetadata.GetPropertyType(propertyInfo.Name);
                }
                catch (Exception ex)
                {
                    continue;
                }
                if (!NHibernateUtil.IsPropertyInitialized(entity, propertyInfo.Name))
                    continue;
                var propertyValue = propertyInfo.GetValue(entity, null);
                if (!NHibernateUtil.IsInitialized(propertyValue))
                    continue;

                var colNames = entityMetadata.GetPropertyColumnNames(propertyInfo.Name);
                if (!entityPropertyType.IsEntityType) continue;
                //Check if we have a parent entity and that is bidirectional related to the current property (one-to-many)
                if (parentEntity.ReferencedColumns.SequenceEqual(colNames))
                {
                    propertyInfo.SetValue(entity, parentEntity.Entity);
                }
            }
            return entity;
        }

        private static object DeepCopy(this ISession session, object entity, DeepCopyOptions opts, System.Type entityType,
            IDictionary<object, object> resolvedEntities, DeepCopyParentEntity parentEntity = null)
        {
            opts = opts ?? new DeepCopyOptions();
            if (entity == null)
                return entityType.GetDefaultValue();

            AbstractEntityPersister entityMetadata;
            try
            {
                entityMetadata = (AbstractEntityPersister)session.SessionFactory.GetClassMetadata(entityType);
            }
            catch (Exception ex)
            {
                return entityType.GetDefaultValue();
            }

            if (!NHibernateUtil.IsInitialized(entity))
                return entityType.GetDefaultValue();

            if (resolvedEntities.ContainsKey(entity) && parentEntity != null)
                return CopyOnlyForeignKeyProperties(resolvedEntities[entity], entityType, entityMetadata, opts, parentEntity);

            if (resolvedEntities.ContainsKey(entity))
                return resolvedEntities[entity];

            if (opts.CanCloneAsReferenceFunc != null && opts.CanCloneAsReferenceFunc(entityType))
                return entity;

            var propertyInfos = entityType.GetProperties();
            var copiedEntity = Activator.CreateInstance(entityType);
            resolvedEntities.Add(entity, copiedEntity);
            
            foreach (var propertyInfo in propertyInfos
                .Where(p => opts.CanCloneIdentifier(entityType) || entityMetadata.IdentifierPropertyName != p.Name)
                .Where(p => !opts.GetIgnoreMembers(entityType).Contains(p.Name))
                .Where(p => p.GetSetMethod(true) != null))
            {
                IType entityPropertyType;
                try
                {
                    entityPropertyType = entityMetadata.GetPropertyType(propertyInfo.Name);
                }
                catch (Exception ex)
                {
                    continue;
                }

                var resolveFn = opts.GetResolveFunction(entityType, propertyInfo.Name);
                if (resolveFn != null)
                {
                    propertyInfo.SetValue(copiedEntity, resolveFn(entity));
                    continue;
                }

                //TODO: verify: false only when entity is a proxy or lazy field/property that is not yet initialized
                if (!NHibernateUtil.IsPropertyInitialized(entity, propertyInfo.Name))
                    continue;
                    
                var propertyValue = propertyInfo.GetValue(entity, null);
                if (!NHibernateUtil.IsInitialized(propertyValue))
                {
                    //Use session load for proxy, works only for references (collections are not supported) 
                    if (
                        propertyValue != null && 
                        NHibernateProxyHelper.IsProxy(propertyValue) && 
                        !(propertyValue is IPersistentCollection) &&
                        opts.UseSessionLoadFunction
                        )
                    {
                        var lazyInit = ((INHibernateProxy) propertyValue).HibernateLazyInitializer;
                        propertyInfo.SetValue(copiedEntity, session.Load(lazyInit.PersistentClass, lazyInit.Identifier));
                    }
                    continue;
                }

                var colNames = entityMetadata.GetPropertyColumnNames(propertyInfo.Name);
                var propType = propertyInfo.PropertyType;
                var copyAsReference = opts.CanCloneAsReference(entityType, propertyInfo.Name);
                if (entityPropertyType.IsCollectionType)
                {
                    var propertyList = CreateNewCollection(propType);
                    propertyInfo.SetValue(copiedEntity, propertyList, null);
                    AddItemToCollection(propertyList, propertyValue, o => copyAsReference
                        ? o
                        : session.DeepCopy(o, opts, GetUnproxiedType(o), resolvedEntities,
                            new DeepCopyParentEntity
                            {
                                Entity = copiedEntity,
                                EntityPersister = entityMetadata,
                                ChildType = entityPropertyType,
                                ReferencedColumns = ((CollectionType) entityPropertyType)
                                    .GetReferencedColumns((ISessionFactoryImplementor) session.SessionFactory)
                            }));
                }
                else if (entityPropertyType.IsEntityType)
                {
                    if (copyAsReference)
                        propertyInfo.SetValue(copiedEntity, propertyValue);
                    //Check if we have a parent entity and that is bidirectional related to the current property (one-to-many)
                    else if (parentEntity != null && parentEntity.ReferencedColumns.SequenceEqual(colNames))
                        propertyInfo.SetValue(copiedEntity, parentEntity.Entity);  
                    else
                        propertyInfo.SetValue(copiedEntity, session.DeepCopy(propertyValue, opts, propType, resolvedEntities), null);
                }
                else if (propType.IsSimpleType())
                {
                    //Check if we have a parent entity and that is bidirectional related to the current property (one-to-many)
                    //we dont want to set FKs to the parent entity as the parent is cloned
                    if (parentEntity != null && parentEntity.ReferencedColumns.Contains(colNames.First()))
                        continue;
                    propertyInfo.SetValue(copiedEntity, propertyValue);
                }
            }
            return copiedEntity;
        }

        /// <summary>
        /// Gets the underlying class type of a persistent object that may be proxied
        /// </summary>
        public static System.Type GetUnproxiedType(object persistentObject)
        {
            var proxy = persistentObject as INHibernateProxy;
            if (proxy != null)
                return proxy.HibernateLazyInitializer.PersistentClass;

            return persistentObject.GetType();
        }

        //can be an interface
        private static object CreateNewCollection(System.Type collectionType)
        {
            var concreteCollType = GetCollectionImplementation(collectionType);
            if(collectionType.IsGenericType)
            {
                concreteCollType = concreteCollType.MakeGenericType(collectionType.GetGenericArguments()[0]);
            }
            var propertyList = Activator.CreateInstance(concreteCollType);
            return propertyList;
        }

        private static void AddItemToCollection(object collection, object item, Func<object, object> editBeforeAdding = null)
        {
            var addMethod = collection.GetType().GetInterfaces()
                        .SelectMany(o => o.GetMethods())
                        .First(o => o.Name == "Add");

            var itemColl = item as IEnumerable;
            if(itemColl != null)
            {
                foreach (var colItem in itemColl)
                {
                    addMethod.Invoke(collection,
                                     editBeforeAdding != null 
                                     ? new[] {editBeforeAdding(colItem)} 
                                     : new[] {colItem});
                }
            }
            else
            {
                addMethod.Invoke(collection,
                                     editBeforeAdding != null
                                     ? new[] { editBeforeAdding(item) }
                                     : new[] { item });
            }
        }

        private static System.Type GetCollectionImplementation(System.Type collectionType)
        {
            if (collectionType.IsAssignableToGenericType(typeof(ISet<>)))
                return typeof(HashSet<>);
            if (collectionType.IsAssignableToGenericType(typeof(IList<>)))
                return typeof(List<>);
            if (collectionType.IsAssignableToGenericType(typeof(ICollection<>)))
                return typeof(List<>);
            if (collectionType.IsAssignableToGenericType(typeof(IEnumerable<>)))
                return typeof(List<>);
            throw new NotSupportedException(collectionType.FullName);
        }
    }
}
