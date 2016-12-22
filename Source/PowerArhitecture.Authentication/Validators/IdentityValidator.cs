using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNet.Identity;
using PowerArhitecture.Validation;

namespace PowerArhitecture.Authentication.Validators
{
    public class IdentityValidator<TUser> : IIdentityValidator<TUser>
    {
        private readonly IValidator<TUser> _userValidator;

        public IdentityValidator(IValidator<TUser> userValidator)
        {
            _userValidator = userValidator;
        }

        public async Task<IdentityResult> ValidateAsync(TUser item)
        {
            var result = await _userValidator.ValidateAsync(item, ruleSets: ValidationRuleSet.AttributeInsertUpdate);
            return result.IsValid
                ? IdentityResult.Success
                : new IdentityResult(result.Errors.Select(o => o.ErrorMessage).ToList());
        }
    }
}
