using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Internationalization;
using Microsoft.AspNet.Identity;

namespace PowerArhitecture.Authentication.Validators
{
    public class PasswordValidator : IPasswordValidator
    {
        public Task<IdentityResult> ValidateAsync(string item)
        {
            if (!string.IsNullOrWhiteSpace(item) && (item.Length >= 6)) //TODO: config
            {
                return Task.FromResult(IdentityResult.Success);
            }
            return Task.FromResult(IdentityResult.Failed(I18N.Translate("'{0}' is too short.", I18N.Translate("Password"))));
        }
    }
}
