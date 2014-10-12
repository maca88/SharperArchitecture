using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Authentication.Repositories;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Internationalization;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Validation;
using FluentValidation.Results;
using Microsoft.AspNet.Identity;
using FluentValidation;

namespace PowerArhitecture.Authentication.Validators
{
    public class UserValidator : PAValidator<User>, IIdentityValidator<User>
    {
        private readonly IUserRepository _repository;

        public UserValidator(IUserRepository repository)
        {
            _repository = repository;
            RuleSet(ValidationRuleSet.Delete, () => Custom(AssertNonSystemUser));
            RuleSet(ValidationRuleSet.InsertUpdate, () => Custom(ValidateUserName));
        }

        private ValidationFailure AssertNonSystemUser(User user)
        {
            return user.IsSystemUser ? ValidationFailure(I18N.Translate("System user cannot be deleted.")) : null;
        }

        private ValidationFailure ValidateUserName(User user)
        {
            if (!string.IsNullOrWhiteSpace(user.UserName))
            {
                if (!Regex.IsMatch(user.UserName, "^[A-Za-z0-9]+$"))
                    return ValidationFailure(o => o.UserName, I18N.Translate("'{0}' is not in the correct format.", 
                        I18N.Translate("UserName")));
            }
            else
                return ValidationFailure(o => o.UserName, I18N.Translate("'{0}' should not be empty.", I18N.Translate("UserName")));

            return _repository.GetLinqQuery().Any(o => o.UserName == user.UserName)
                ? ValidationFailure(o => o.UserName, I18N.Translate("'{0}' '{1}' already exists.", I18N.Translate("UserName"), user.Name)) 
                : null;
        }

        Task<IdentityResult> IIdentityValidator<User>.ValidateAsync(User item)
        {
            var result = Validate(item, ValidationRuleSet.AttributeInsertUpdate);
            return Task.FromResult(result.IsValid
                ? IdentityResult.Success
                : new IdentityResult(result.Errors.Select(o => o.ErrorMessage).ToList()));
        }

    }
}
