using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Authentication.Specifications;
using SharperArchitecture.Common.Internationalization;
using Microsoft.AspNet.Identity;

namespace SharperArchitecture.Authentication.Validators
{
    /// <summary>
    /// Custom password validator that uses gettext for validation errors
    /// TODO: port all functionality
    /// </summary>
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
