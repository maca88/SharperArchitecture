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
        private readonly Type _userType;

        public PopulateDbEventHandler(IAuthenticationConfiguration authSettings, IPasswordHasher passwordHasher)
        {
            _authSettings = authSettings;
            _passwordHasher = passwordHasher;
            _userType = Type.GetType(authSettings.UserClass, true);
        }

        public void Handle(PopulateDbEvent e)
        {
            var unitOfWork = e.UnitOfWork;
            var systemUser = (IEntity)Activator.CreateInstance(_userType);

            _userType.GetProperty("TimeZoneId").SetValue(systemUser, TimeZoneInfo.Utc.Id);
            _userType.GetProperty("UserName").SetValue(systemUser, _authSettings.SystemUserName);
            _userType.GetProperty("PasswordHash").SetValue(systemUser, _passwordHasher.HashPassword(_authSettings.SystemUserPassword));

            unitOfWork.Save(systemUser);
        }
    }
}
