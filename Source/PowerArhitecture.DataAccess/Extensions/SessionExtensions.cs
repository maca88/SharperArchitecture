using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PowerArhitecture.Domain;
using Iesi.Collections;
using NHibernate.Metadata;
using NHibernate.Proxy;
using NHibernate.Type;

namespace NHibernate
{
    public static class SessionExtensions
    {
        public static IEnumerable<T> DeepCopy<T>(this ISession session, IEnumerable<T> entities) where T : class, IEntity
        {
            var resolvedEntities = new Dictionary<object, object>();
            return entities.Select(entity => session.DeepCopy(entity, resolvedEntities)).ToList();
        }

        public static T DeepCopy<T>(this ISession session, T entity) where T : class, IEntity
        {
            // forward to resolver
            return (T)session.DeepCopy(entity, entity.GetType(), new Dictionary<object, object>());
        }

        public static IEnumerable DeepCopy(this ISession session, IEnumerable entities)
        {
            var collection = (IEnumerable)CreateNewCollection(entities.GetType());
            var resolvedEntities = new Dictionary<object, object>();
            foreach (var entity in entities)
            {
                AddItemToCollection(collection, session.DeepCopy(entity, entity.GetType(), resolvedEntities));
            }
            return collection;
        }

        private static T DeepCopy<T>(this ISession session, T entity, IDictionary<object, object> resolvedEntities) where T : class, IEntity
        {
            return (T)session.DeepCopy(entity, GetUnproxiedType(entity), resolvedEntities);
        }

        private static object DeepCopy(this ISession session, object entity, System.Type entityType, IDictionary<object, object> resolvedEntities)
        {
            if (entity == null)
                return entityType.GetDefaultValue();

            if (!NHibernateUtil.IsInitialized(entity))
                return entityType.GetDefaultValue();

            if (resolvedEntities.ContainsKey(entity))
                return resolvedEntities[entity];

            var copiedEntity = Activator.CreateInstance(entityType);

            resolvedEntities.Add(entity, copiedEntity);

            IClassMetadata entityMetadata;
            try
            {
                entityMetadata = session.SessionFactory.GetClassMetadata(entityType);
            }
            catch (Exception ex)
            {
                return entityType.GetDefaultValue();
            }

            var propertyInfos = entityType.GetProperties();

            foreach (var propertyInfo in propertyInfos)
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

                if(!NHibernateUtil.IsPropertyInitialized(entity, propertyInfo.Name))
                    continue;

                var propertyValue = propertyInfo.GetValue(entity, null);
                if (!NHibernateUtil.IsInitialized(propertyValue))
                    continue;
                var propType = propertyInfo.PropertyType;
                if (entityPropertyType.IsCollectionType)
                {
                    var propertyList = CreateNewCollection(propType);
                    propertyInfo.SetValue(copiedEntity, propertyList, null);
                    AddItemToCollection(propertyList, propertyValue, o => session.DeepCopy(o, GetUnproxiedType(o), resolvedEntities));
                }
                else if (entityPropertyType.IsEntityType)
                {
                    propertyInfo.SetValue(copiedEntity, session.DeepCopy(propertyValue, propType, resolvedEntities), null);
                }
                else if (propType.IsSimpleType())
                {
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
