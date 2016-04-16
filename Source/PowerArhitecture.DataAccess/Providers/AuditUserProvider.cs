using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Metadata;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;

namespace PowerArhitecture.DataAccess.Providers
{
    public class AuditUserProvider : IAuditUserProvider
    {
        public static string UserNamePropertyName = "UserName";

        /// <summary>
        /// Used when GetCurrentUserName method return null or an empty string. 
        /// Set this when user type is a string or when principal is not of GenericPrincipal type.
        /// </summary>
        public static string DefaultUserName;

        /// <summary>
        /// Used when GetCurrentPrincipal method returns an GenericPrincipal without a NameIdentifier claim
        /// Set this when principal is of GenericPrincipal type
        /// </summary>
        public static object DefaultUserId;

        /// <summary>
        /// Can boost performance when the user is fetch by its username. Enable only when the users cannot be reinserted with different ids.
        /// </summary>
        public static bool CacheUserIds;

        protected ConcurrentDictionary<string, object> UserNameIds = new ConcurrentDictionary<string, object>(); 

        public virtual object GetCurrentUser(ISession session, Type userType)
        {
            var principal = GetCurrentPrincipal();
            var userName = GetCurrentUserName(principal);

            if (string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(DefaultUserName))
                userName = DefaultUserName;

            if (userType == typeof(string))
            {
                return userName;
            }

            var entity = principal as IEntity;
            if (entity != null)
            {
                return GetEntity(session, userType, entity);
            }

            var genPrincipal = principal as GenericPrincipal;
            if (genPrincipal != null)
            {
                var idClaim = genPrincipal.FindFirst(ClaimTypes.NameIdentifier);
                var objId = idClaim != null ? idClaim.Value : DefaultUserId;

                if (objId != null)
                {
                    var userMetaData = GetUserMetadata(session, userType);
                    if (userMetaData != null)
                    {
                        try
                        {
                            var id = objId.ConvertTo(userMetaData.IdentifierType.ReturnedClass);
                            return GetEntityById(session, userMetaData, userType, id);
                        }
                        catch
                        {
                            
                        }
                        
                    }
                }
            }

            if (typeof (IEntity).IsAssignableFrom(userType))
            {
                var userMetaData = GetUserMetadata(session, userType);
                if (userMetaData == null)
                    return null;

                if (string.IsNullOrEmpty(userName))
                {
                    if(DefaultUserId == null)
                        return null;
                    return GetEntityById(session, userMetaData, userType, DefaultUserId);
                }

                if (CacheUserIds)
                {
                    object userId;
                    if (UserNameIds.TryGetValue(userName, out userId))
                    {
                        return GetEntityById(session, userMetaData, userType, userId);
                    }
                }

                for (var i = 0; i < userMetaData.PropertyNames.Length; i++)
                {
                    if (userMetaData.PropertyNames[i] == UserNamePropertyName &&
                        userMetaData.PropertyTypes[i].ReturnedClass == typeof(string))
                    {
                        IEntity user;
                        if (
                            userMetaData.HasNaturalIdentifier &&
                            userMetaData.NaturalIdentifierProperties.Length == 1 &&
                            userMetaData.NaturalIdentifierProperties.Contains(i))
                        {
                            user = (IEntity) session.CreateCriteria(userType)
                                .Add(Restrictions.NaturalId().Set(UserNamePropertyName, userName))
                                .SetCacheable(true)
                                .UniqueResult();
                        }
                        else
                        {
                            user = (IEntity)session.CreateCriteria(userType)
                                .Add(Restrictions.Eq(UserNamePropertyName, userName))
                                .SetCacheable(true)
                                .UniqueResult();
                        }
                        if (CacheUserIds)
                        {
                            var userId = user.GetId();
                            if (!UserNameIds.ContainsKey(userName))
                                UserNameIds.TryAdd(userName, userId);
                            else
                            {
                                //Check if the user id is the same as the cached one (can happen when user is reinserted)
                                object currUserId;
                                if (UserNameIds.TryGetValue(userName, out currUserId) && currUserId != userId)
                                {
                                    UserNameIds.TryUpdate(userName, userId, currUserId);
                                }

                            }
                            
                        }

                        return user;
                    }
                }
            }

            throw new NotSupportedException(string.Format("Unsupported user type: {0}. Hint: Register a custom IAuditUserProvider", userType));
        }

        /// <summary>
        /// Check if the entity has an entry in the given session, if true then return the same instance otherwise load the entity with the Load session method
        /// </summary>
        /// <param name="session"></param>
        /// <param name="userType"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual object GetEntity(ISession session, Type userType, IEntity entity)
        {
            if (session.GetSessionImplementation().PersistenceContext.IsEntryFor(entity))
                return entity;
            return session.Load(userType, entity.GetId());
        }

        /// <summary>
        /// Return the entity from the persistence context if found otherwise the Load method will be used
        /// </summary>
        /// <param name="session"></param>
        /// <param name="metadata"></param>
        /// <param name="userType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual object GetEntityById(ISession session, IClassMetadata metadata, Type userType, object id)
        {
            var sessionImpl = session.GetSessionImplementation();
            var persister = sessionImpl.Factory.GetEntityPersister(metadata.EntityName);
            var key = sessionImpl.GenerateEntityKey(id, persister);
            var entity = sessionImpl.PersistenceContext.GetEntity(key);
            if (entity != null)
                return entity;
            return session.Load(userType, id);
        }

        /// <summary>
        /// Gets the username from the identity
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        protected virtual string GetCurrentUserName(IPrincipal principal)
        {
            return principal.Identity.Name;
        }

        /// <summary>
        /// Get user principal using the current thread
        /// </summary>
        /// <returns></returns>
        protected virtual IPrincipal GetCurrentPrincipal()
        {
            return Thread.CurrentPrincipal;
        }

        /// <summary>
        /// Get the metadata from the session factory
        /// </summary>
        /// <param name="session"></param>
        /// <param name="userType"></param>
        /// <returns></returns>
        protected virtual IClassMetadata GetUserMetadata(ISession session, Type userType)
        {
            return session.GetSessionImplementation().Factory.GetClassMetadata(userType);
        }
    }
}
