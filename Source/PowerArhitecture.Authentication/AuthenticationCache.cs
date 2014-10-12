using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.DataCaching;
using PowerArhitecture.DataCaching.Specifications;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Linq;
using Ninject.Extensions.Logging;
using IUser = PowerArhitecture.Common.Specifications.IUser;

namespace PowerArhitecture.Authentication
{
    public class AuthenticationCache : BaseApplicationCache, IAuthenticationCache
    {
        private const string UserKey = "User_{0}";
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public AuthenticationCache(IEventAggregator eventAggregator, ILogger logger, IDataCache dataCache, IUnitOfWorkFactory unitOfWorkFactory,
            ISessionEventListener sessionEventListener, IDataCachingSettings settings)
            : base(eventAggregator, logger, dataCache, sessionEventListener, settings)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
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
            var user = entity as User;
            FillCache(o => o.Id == user.Id);
        }

        #endregion

        #region Deleted

        protected override object EntityPreDeleting(object entity, ISession session)
        {
            return GetUser(entity, session);
        }

        protected override void EntityDeleted(object entity, ISession session)
        {
            var user = (User)entity;
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
            var user = entity as User;
            FillCache(o => o.Id == user.Id);
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

        private void FillCache(Expression<Func<User, bool>> condition = null)
        {
            using (var unitOfWork = _unitOfWorkFactory.GetNew())
            {
                var query = unitOfWork.Query<User>()
                                        //.Lock(LockMode.Write)
                                        .Include(o => o.Claims)
                                        .Include(o => o.Logins)
                                        .Include(o => o.Settings)
                                        .Include(o => o.Organization.OrganizationRoles.First().Role.RolePermissions.First().Permission)
                                        .Include(o => o.Organization.OrganizationRoles.First().Role.PermissionPatterns)
                                        .Include(o => o.UserRoles.First().Role.RolePermissions.First().Permission)
                                        .Include(o => o.UserRoles.First().Role.PermissionPatterns);
                if(condition != null)
                    query = query.Where(condition);

                var users = query.ToList();
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
            return entity is User ? entity : null;
        }

        private void SetMonitoringTypes() //TODO: add settings
        {
            MonitoringTypes.Add(typeof(User));
            MonitoringTypes.Add(typeof(UserClaim));
            MonitoringTypes.Add(typeof(UserLogin));
            MonitoringTypes.Add(typeof(Role));
        }
    }
}
