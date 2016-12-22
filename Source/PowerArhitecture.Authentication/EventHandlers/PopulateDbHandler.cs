using System;
using Microsoft.AspNet.Identity;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Authentication.EventHandlers
{
    [Priority(1000)]
    public class PopulateDbEventHandler : IEventHandler<PopulateDbEvent>
    {
        private readonly IAuthenticationConfiguration _authSettings;
        private readonly IPasswordHasher _passwordHasher;

        public PopulateDbEventHandler(IAuthenticationConfiguration authSettings, IPasswordHasher passwordHasher)
        {
            _authSettings = authSettings;
            _passwordHasher = passwordHasher;
        }

        public void Handle(PopulateDbEvent e)
        {
            var userType = Package.UserType;
            var session = e.Session;
            var systemUser = (IEntity)Activator.CreateInstance(userType);

            userType.GetProperty("TimeZoneId").SetValue(systemUser, TimeZoneInfo.Utc.Id);
            userType.GetProperty("UserName").SetValue(systemUser, _authSettings.SystemUserName);
            userType.GetProperty("PasswordHash").SetValue(systemUser, _passwordHasher.HashPassword(_authSettings.SystemUserPassword));

            session.Save(systemUser);
        }
    }
}
