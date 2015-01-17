using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Utils;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.DataCaching;
using PowerArhitecture.DataCaching.Specifications;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Linq;
using Ninject.Extensions.Logging;
using PowerArhitecture.Domain;
using IUser = PowerArhitecture.Common.Specifications.IUser;

namespace PowerArhitecture.Authentication
{
    public class AuthenticationCache : BaseApplicationCache, IAuthenticationCache
    {
        private const string UserKey = "User_{0}";
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly Type _userType = typeof(User);
        private static MethodInfo _queryMethod;
        private IAuthenticationSettings _settings;

        static AuthenticationCache()
        {
            _queryMethod = ReflectionHelper.GetMethodDefinition((UnitOfWork p) => p.Query<VersionedEntity>());
        }

        public AuthenticationCache(IEventAggregator eventAggregator, ILogger logger, IDataCache dataCache, IUnitOfWorkFactory unitOfWorkFactory,
            ISessionEventListener sessionEventListener, IDataCachingSettings settings, IAuthenticationSettings authSettings)
            : base(eventAggregator, logger, dataCache, sessionEventListener, settings)
        {
            if(!string.IsNullOrEmpty(authSettings.UserClass))
                _userType = Type.GetType(authSettings.UserClass, true);
            _unitOfWorkFactory = unitOfWorkFactory;
            _settings = authSettings;
        }

        public override void Initialize()
        {
            SetMonitoringTypes();
        }

        public override void Refresh()
        {
            FillCache();
        }

        #region Updated

        protected override object EntityPreUpdating(object entity, NHibernate.ISession session)
        {
            return GetUser(entity, session);
        }

        protected override void EntityUpdated(object entity, NHibernate.ISession session)
        {
            var user = entity as IUser;
            if (user == null) return;
            FillCache(query => query.Where("Id = @0", user.Id));
        }

        #endregion

        #region Deleted

        protected override object EntityPreDeleting(object entity, ISession session)
        {
            return GetUser(entity, session);
        }

        protected override void EntityDeleted(object entity, ISession session)
        {
            var user = (IUser)entity;
            var userKey = GetUserKey(user.UserName);
            DataCache.Delete(userKey);
        }

        #endregion

        #region Inserted

        protected override object EntityPreInserting(object entity, ISession session)
        {
            return GetUser(entity, session);
        }

        protected override void EntityInserted(object entity, ISession session)
        {
            var user = entity as IUser;
            if (user == null) return;
            FillCache(query => query.Where("Id = @0", user.Id));
        }

        #endregion

        public IUser GetUser(string userName)
        {
            return DataCache.Get<IUser>(GetUserKey(userName));
        }

        public void InsertOrUpdateUser(IUser user)
        {
            DataCache.InsertOrUpdate(GetUserKey(user.UserName), user);
        }

        public void DeleteUser(IUser user)
        {
            DataCache.Delete(GetUserKey(user.UserName));
        }

        private void FillCache(Func<IQueryable, IQueryable> queryFn = null)
        {
            using (var unitOfWork = (UnitOfWork)_unitOfWorkFactory.GetNew())
            {
                var query = (IQueryable)_queryMethod.MakeGenericMethod(_userType).Invoke(unitOfWork, null);
                query = query
                    .Include("Claims")
                    .Include("Logins")
                    .Include("Settings")
                    .Include("Organization.OrganizationRoles.Role.RolePermissions.Permission")
                    .Include("Organization.OrganizationRoles.Role.PermissionPatterns")
                    .Include("UserRoles.Role.RolePermissions.Permission")
                    .Include("UserRoles.Role.PermissionPatterns");

                if (!string.IsNullOrEmpty(_settings.AdditionalUserIncludes))
                {
                    var includes = _settings.AdditionalUserIncludes.Split(',');
                    query = includes.Aggregate(query, (current, include) => current.Include(include.Trim()));
                }

                if (queryFn != null)
                    query = queryFn(query);

                var users = query.ToList<IUser>();
                //Store each user separately in cache (slower at startup, faster on update, delete and insert)
                foreach (var user in users)
                {
                    var userKey = GetUserKey(user.UserName);
                    DataCache.InsertOrUpdate(userKey, user);
                }
            }
        }

        private string GetUserKey(string userName)
        {
            return string.Format(UserKey, userName);
        }

        private object GetUser(object entity, ISession session)
        {
            var claim = entity as UserClaim;
            if (claim != null)
                return claim.User;
            var login = entity as UserLogin;
            if (login != null)
                return login.User;
            var userRole = entity as UserRole;
            if (userRole != null)
                return userRole.User;
            var role = entity as Role;
            if (role != null)
                return role.UserRoles.Select(o => o.User);
            return entity is IUser ? entity : null;
        }

        private void SetMonitoringTypes() //TODO: add settings
        {
            MonitoringTypes.Add(_userType);
            MonitoringTypes.Add(typeof(UserClaim));
            MonitoringTypes.Add(typeof(UserLogin));
            MonitoringTypes.Add(typeof(Role));
        }
    }
}
