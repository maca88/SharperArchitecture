using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using Microsoft.AspNet.Identity;
using SharperArchitecture.Authentication.Specifications;
using SharperArchitecture.Common.Attributes;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.DataAccess.Events;
using SharperArchitecture.Domain;

namespace SharperArchitecture.Authentication.EventHandlers
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
            if (session.SessionFactory.GetClassMetadata(userType) == null)
            {
                return;
            }
            var systemUser = (IEntity)Activator.CreateInstance(userType);

            userType.GetProperty("TimeZoneId").SetValue(systemUser, TimeZoneInfo.Utc.Id);
            userType.GetProperty("UserName").SetValue(systemUser, _authSettings.SystemUserName);
            userType.GetProperty("PasswordHash").SetValue(systemUser, _passwordHasher.HashPassword(_authSettings.SystemUserPassword));

            session.Save(systemUser);

            var identity = new GenericIdentity(_authSettings.SystemUserName);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, systemUser.GetId().ToString()));
            Thread.CurrentPrincipal = new GenericPrincipal(identity, null);
        }
    }
}
