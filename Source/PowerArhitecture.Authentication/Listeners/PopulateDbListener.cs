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
        private readonly IAuthenticationConfiguration _authConfiguration;
        private readonly IPasswordHasher _passwordHasher;

        public PopulateDbListener(IAuthenticationConfiguration authConfiguration, IPasswordHasher passwordHasher)
        {
            _authConfiguration = authConfiguration;
            _passwordHasher = passwordHasher;
        }

        public void Handle(PopulateDbEvent e)
        {
            var session = e.Message;
            var userType = _authConfiguration.GetUserType();
            var systemUser = (IEntity)Activator.CreateInstance(userType);

            userType.GetProperty("TimeZoneId").SetValue(systemUser, TimeZoneInfo.Utc.Id);
            userType.GetProperty("UserName").SetValue(systemUser, _authConfiguration.SystemUserName);
            userType.GetProperty("PasswordHash").SetValue(systemUser, _passwordHasher.HashPassword(_authConfiguration.SystemUserPassword));

            session.Save(systemUser);
            //session.Flush();
            //PrincipalHelper.SetSystemUser(session.DeepCopy(systemUser));
            //PrincipalHelper.SetCurrentUser(PrincipalHelper.GetSystemUser());

        }
    }
}
