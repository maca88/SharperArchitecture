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

        protected class UserInfo
        {
            public static readonly UserInfo NotFound = new UserInfo(null);

            public UserInfo()
            {
            }

            public UserInfo(object current)
            {
                Current = current;
                Found = true;
            }

            public IClassMetadata Metadata { get; set; }

            public string Name { get; set; }

            public object Id { get; set; }

            public object Current { get; set; }

            public bool NaturalId { get; set; }

            public bool Found { get; }
        }

        public virtual object GetCurrentUser(ISession session, Type userType)
        {
            var userInfo = GetCurrentUserInfo(session, userType);
            if (userInfo.Found || userInfo == UserInfo.NotFound)
            {
                return userInfo.Current;
            }

            if (userInfo.Id != null)
            {
                return GetEntityById(session, userInfo.Metadata, userType, userInfo.Id);
            }

            IEntity user;
            if (userInfo.NaturalId)
            {
                user = (IEntity) session.CreateCriteria(userType)
                    .Add(Restrictions.NaturalId().Set(UserNamePropertyName, userInfo.Name))
                    .SetCacheable(true)
                    .UniqueResult();
            }
            else
            {
                user = (IEntity) session.CreateCriteria(userType)
                    .Add(Restrictions.Eq(UserNamePropertyName, userInfo.Name))
                    .SetCacheable(true)
                    .UniqueResult();
            }
            CacheUserId(user, userInfo.Name);
            return user;
        }

        public virtual async Task<object> GetCurrentUserAsync(ISession session, Type userType)
        {
            var userInfo = GetCurrentUserInfo(session, userType);
            if (userInfo.Found || userInfo == UserInfo.NotFound)
            {
                return userInfo.Current;
            }

            if (userInfo.Id != null)
            {
                return await GetEntityByIdAsync(session, userInfo.Metadata, userType, userInfo.Id);
            }

            IEntity user;
            if (userInfo.NaturalId)
            {
                user = (IEntity) await session.CreateCriteria(userType)
                    .Add(Restrictions.NaturalId().Set(UserNamePropertyName, userInfo.Name))
                    .SetCacheable(true)
                    .UniqueResultAsync();
            }
            else
            {
                user = (IEntity) await session.CreateCriteria(userType)
                    .Add(Restrictions.Eq(UserNamePropertyName, userInfo.Name))
                    .SetCacheable(true)
                    .UniqueResultAsync();
            }
            CacheUserId(user, userInfo.Name);
            return user;
        }

        protected virtual UserInfo GetCurrentUserInfo(ISession session, Type userType)
        {
            var principal = Thread.CurrentPrincipal; // .NET 4.6 and above
            var userName = principal?.Identity?.Name;

            if (string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(DefaultUserName))
                userName = DefaultUserName;

            if (userType == typeof(string))
            {
                return new UserInfo(userName);
            }
            if (principal == null)
            {
                return UserInfo.NotFound;
            }

            Claim idClaim = null;
            var claimPrincipal = principal as ClaimsPrincipal;
            if (claimPrincipal != null)
            {
                idClaim = claimPrincipal.Claims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier);
            }
            var genPrincipal = principal as GenericPrincipal;
            if (genPrincipal != null)
            {
                idClaim = genPrincipal.FindFirst(ClaimTypes.NameIdentifier);
            }

            if (genPrincipal != null || claimPrincipal != null)
            {
                var objId = idClaim != null ? idClaim.Value : DefaultUserId;
                if (objId != null)
                {
                    var userMetaData = GetUserMetadata(session, userType);
                    if (userMetaData != null)
                    {
                        try
                        {
                            var id = objId.ConvertTo(userMetaData.IdentifierType.ReturnedClass);
                            return new UserInfo
                            {
                                Id = id,
                                Metadata = userMetaData
                            };
                        }
                        catch
                        {
                        }
                    }
                }
            }

            if (typeof(IEntity).IsAssignableFrom(userType))
            {
                var userMetaData = GetUserMetadata(session, userType);
                if (userMetaData == null)
                {
                    return UserInfo.NotFound;
                }

                if (string.IsNullOrEmpty(userName))
                {
                    if (DefaultUserId == null)
                    {
                        return UserInfo.NotFound;
                    }
                    return new UserInfo
                    {
                        Metadata = userMetaData,
                        Id = DefaultUserId
                    };
                }

                if (CacheUserIds)
                {
                    object userId;
                    if (UserNameIds.TryGetValue(userName, out userId))
                    {
                        return new UserInfo
                        {
                            Metadata = userMetaData,
                            Id = userId
                        };
                    }
                }

                for (var i = 0; i < userMetaData.PropertyNames.Length; i++)
                {
                    if (userMetaData.PropertyNames[i] == UserNamePropertyName &&
                        userMetaData.PropertyTypes[i].ReturnedClass == typeof(string))
                    {
                        var userInfo = new UserInfo
                        {
                            Name = userName
                        };
                        if (
                            userMetaData.HasNaturalIdentifier &&
                            userMetaData.NaturalIdentifierProperties.Length == 1 &&
                            userMetaData.NaturalIdentifierProperties.Contains(i))
                        {
                            userInfo.NaturalId = true;
                        }
                        return userInfo;
                    }
                }
            }
            throw new NotSupportedException($"Unsupported user type: {userType}. Hint: Register a custom IAuditUserProvider");
        }

        protected virtual void CacheUserId(IEntity user, string userName)
        {
            if (user == null || !CacheUserIds)
            {
                return;
            }
            var userId = user.GetId();
            if (!UserNameIds.ContainsKey(userName))
            {
                UserNameIds.TryAdd(userName, userId);
            }
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

        /// <summary>
        /// Return the entity from the persistence context if found otherwise the Load method will be used
        /// </summary>
        /// <param name="session"></param>
        /// <param name="metadata"></param>
        /// <param name="userType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual Task<object> GetEntityByIdAsync(ISession session, IClassMetadata metadata, Type userType, object id)
        {
            var entity = GetEntityByIdFromContext(session, metadata, userType, id);
            return entity != null ? Task.FromResult(entity) : session.LoadAsync(userType, id);
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
            var entity = GetEntityByIdFromContext(session, metadata, userType, id);
            return entity ?? session.Load(userType, id);
        }

        protected virtual object GetEntityByIdFromContext(ISession session, IClassMetadata metadata, Type userType, object id)
        {
            var sessionImpl = session.GetSessionImplementation();
            var persister = sessionImpl.Factory.GetEntityPersister(metadata.EntityName);
            var key = sessionImpl.GenerateEntityKey(id, persister);
            return sessionImpl.PersistenceContext.GetEntity(key);
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
