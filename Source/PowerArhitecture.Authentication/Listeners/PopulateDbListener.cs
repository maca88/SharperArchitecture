using System;
using System.Threading;
using System.Web;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Helpers;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.Authentication.Specifications;
using Microsoft.AspNet.Identity;
using NHibernate;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Authentication.Listeners
{
    [Priority(1000)]
    public class PopulateDbListener : IListener<PopulateDbEvent>
    {
        private readonly IAuthenticationSettings _authSettings;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthenticationCache _authCache;
        private readonly Type _userType = typeof(User);

        public PopulateDbListener(IAuthenticationSettings authSettings, IPasswordHasher passwordHasher, IAuthenticationCache authCache)
        {
            _authCache = authCache;
            _authSettings = authSettings;
            _passwordHasher = passwordHasher;
            if (!string.IsNullOrEmpty(authSettings.UserClass))
                _userType = Type.GetType(authSettings.UserClass, true);
        }

        public void Handle(PopulateDbEvent e)
        {
            var session = e.Message;

            var systemUser = (IEntity)Activator.CreateInstance(_userType);

            _userType.GetProperty("TimeZoneId").SetValue(systemUser, TimeZoneInfo.Utc.Id);
            _userType.GetProperty("UserName").SetValue(systemUser, _authSettings.SystemUserName);
            _userType.GetProperty("PasswordHash").SetValue(systemUser, _passwordHasher.HashPassword(_authSettings.SystemUserPassword));

            session.Save(systemUser);

            var copySystemUser = (Common.Specifications.IUser)session.DeepCopy(systemUser);

            _authCache.InsertOrUpdateUser(copySystemUser);
            Thread.CurrentPrincipal = copySystemUser;
            if (HttpContext.Current != null)
                HttpContext.Current.User = copySystemUser;


            //session.Flush();
            //PrincipalHelper.SetSystemUser(session.DeepCopy(systemUser));
            //PrincipalHelper.SetCurrentUser(PrincipalHelper.GetSystemUser());

        }
    }
}
