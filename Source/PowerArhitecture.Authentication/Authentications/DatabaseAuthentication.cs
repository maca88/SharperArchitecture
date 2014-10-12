using System;
using System.Linq;
using System.Security.Principal;
using BAF.Authentication.Entities;
using BAF.Authentication.Repositories;
using BAF.Authentication.Specifications;
using BAF.Common.Helpers;
using BAF.Common.Specifications;
using BAF.DataAccess.Specifications;
using FluentValidation.Results;

namespace BAF.Authentication.Authentications
{/*
    public class DatabaseAuthentication : IAuthentication
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticationResource _locResource;
        private readonly ICryptography _cryptography;

        public DatabaseAuthentication(IUserRepository userRepository, IAuthenticationResource locResource, ICryptography cryptography)
        {
            _userRepository = userRepository;
            _locResource = locResource;
            _cryptography = cryptography;
        }

        public ValidationFailure LogOn(string name, string password)
        {
            var user = _userRepository
                .GetEntitiesQuery()
                .Include(o => o.Roles.First().RightPatterns)
                .Include(o => o.Roles.First().Rights)
                .SingleOrDefault(o => o.UserName == name && o.PasswordHash == _cryptography.Encrypt(password));
            if (user == null)
                return new ValidationFailure("", _locResource.Validation.InvalidUsernameOrPassword);
            PrincipalHelper.SetCurrentUser(_userRepository.DeepCopy(user));
            return null;
        }

        public TUser GetCurrentUser<TUser>() where TUser : class, IPrincipal
        {
            return PrincipalHelper.GetCurrentUser() as TUser;
        }

        public void LogOut()
        {
            throw new NotImplementedException();
        }
    }*/
}
