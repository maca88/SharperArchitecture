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
using NHibernate.Linq;
using IUser = PowerArhitecture.Authentication.Specifications.IUser;

namespace PowerArhitecture.Authentication.Validators
{
    public abstract class UserValidator<TUser> : Validator<TUser>, IIdentityValidator<TUser>
        where TUser : class, IUser, IEntity<long>, new()
    {
        private readonly IUserRepository<TUser> _repository;

        public UserValidator(IUserRepository<TUser> repository)
        {
            _repository = repository;
            RuleSet(ValidationRuleSet.Delete, () => Custom(AssertNonSystemUser));
            RuleSet(ValidationRuleSet.Update, () => Custom(ValidateUserName));
            RuleSet(ValidationRuleSet.Insert, () => CustomAsync(CheckDuplicates));
        }

        private ValidationFailure AssertNonSystemUser(TUser user)
        {
            return user.IsSystemUser ? Failure(I18N.Translate("System user cannot be deleted.")) : null;
        }

        private ValidationFailure ValidateUserName(TUser user)
        {
            if (!string.IsNullOrWhiteSpace(user.UserName))
            {
                if (!Regex.IsMatch(user.UserName, "^[A-Za-z0-9]+$"))
                    return Failure(o => o.UserName, I18N.Translate("'{0}' is not in the correct format.", 
                        I18N.Translate("UserName")));
            }
            else
                return Failure(o => o.UserName, I18N.Translate("'{0}' should not be empty.", I18N.Translate("UserName")));
            return Success;
        }

        private async Task<ValidationFailure> CheckDuplicates(TUser user)
        {
            return await _repository.Query().AnyAsync(o => o.UserName == user.UserName)
                ? Failure(o => o.UserName, I18N.Translate("'{0}' '{1}' already exists.", I18N.Translate("UserName"), user.UserName))
                : null;
        }

        async Task<IdentityResult> IIdentityValidator<TUser>.ValidateAsync(TUser item)
        {
            var result = await ValidateAsync(item, ValidationRuleSet.AttributeInsertUpdate);
            return result.IsValid
                ? IdentityResult.Success
                : new IdentityResult(result.Errors.Select(o => o.ErrorMessage).ToList());
        }

    }
}
