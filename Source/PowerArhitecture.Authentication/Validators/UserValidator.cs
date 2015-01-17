using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Authentication.Repositories;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Internationalization;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Validation;
using FluentValidation.Results;
using Microsoft.AspNet.Identity;
using FluentValidation;

namespace PowerArhitecture.Authentication.Validators
{
    public class UserValidator : UserValidator<User>
    {
        public UserValidator(IUserRepository<User> repository) : base(repository)
        {
        }
    }

    public abstract class UserValidator<TUser> : PAValidator<TUser>, IIdentityValidator<TUser>
        where TUser : class, Common.Specifications.IUser, IEntity<long>, new()
    {
        private readonly IUserRepository<TUser> _repository;

        public UserValidator(IUserRepository<TUser> repository)
        {
            _repository = repository;
            RuleSet(ValidationRuleSet.Delete, () => Custom(AssertNonSystemUser));
            RuleSet(ValidationRuleSet.InsertUpdate, () => Custom(ValidateUserName));
        }

        private ValidationFailure AssertNonSystemUser(TUser user)
        {
            return user.IsSystemUser ? ValidationFailure(I18N.Translate("System user cannot be deleted.")) : null;
        }

        private ValidationFailure ValidateUserName(TUser user)
        {
            if (!string.IsNullOrWhiteSpace(user.UserName))
            {
                if (!Regex.IsMatch(user.UserName, "^[A-Za-z0-9]+$"))
                    return ValidationFailure(o => o.UserName, I18N.Translate("'{0}' is not in the correct format.", 
                        I18N.Translate("UserName")));
            }
            else
                return ValidationFailure(o => o.UserName, I18N.Translate("'{0}' should not be empty.", I18N.Translate("UserName")));

            return _repository.Query().Any(o => o.UserName == user.UserName)
                ? ValidationFailure(o => o.UserName, I18N.Translate("'{0}' '{1}' already exists.", I18N.Translate("UserName"), user.UserName)) 
                : null;
        }

        Task<IdentityResult> IIdentityValidator<TUser>.ValidateAsync(TUser item)
        {
            var result = Validate(item, ValidationRuleSet.AttributeInsertUpdate);
            return Task.FromResult(result.IsValid
                ? IdentityResult.Success
                : new IdentityResult(result.Errors.Select(o => o.ErrorMessage).ToList()));
        }

    }
}
