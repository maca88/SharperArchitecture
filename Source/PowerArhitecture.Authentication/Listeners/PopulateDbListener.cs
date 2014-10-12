using System;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Helpers;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.Authentication.Specifications;
using Microsoft.AspNet.Identity;
using NHibernate;

namespace PowerArhitecture.Authentication.Listeners
{
    public class PopulateDbListener : IListener<PopulateDbEvent>
    {
        private readonly IAuthenticationSettings _authSettings;
        private readonly IPasswordHasher _passwordHasher;

        public PopulateDbListener(IAuthenticationSettings authSettings, IPasswordHasher passwordHasher)
        {
            _authSettings = authSettings;
            _passwordHasher = passwordHasher;
        }

        public void Handle(PopulateDbEvent e)
        {
            var session = e.Message;
            var systemUser = new User
            {
                UserName = _authSettings.SystemUserName,
                PasswordHash = _passwordHasher.HashPassword(_authSettings.SystemUserPassword),
                TimeZoneId = TimeZoneInfo.Utc.Id
            };
            session.Save(systemUser);

            //session.Flush();
            //PrincipalHelper.SetSystemUser(session.DeepCopy(systemUser));
            //PrincipalHelper.SetCurrentUser(PrincipalHelper.GetSystemUser());

        }
    }
}
