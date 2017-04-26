using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SharperArchitecture.Authentication.Specifications;
using SharperArchitecture.Common.Internationalization;

namespace SharperArchitecture.Authentication.Validators
{
    /// <summary>
    /// Custom password validator that uses gettext for validation errors
    /// </summary>
    public class PasswordValidator : Microsoft.AspNet.Identity.PasswordValidator, IPasswordValidator
    {
        public override Task<IdentityResult> ValidateAsync(string item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            var stringList = new List<string>();
            if (string.IsNullOrWhiteSpace(item) || item.Length < RequiredLength)
                stringList.Add(I18N.Translate("Password must be at least {0} characters.", RequiredLength));
            if (RequireNonLetterOrDigit && item.All(IsLetterOrDigit))
                stringList.Add(I18N.Translate("Password must have at least one non letter or digit character."));
            if (RequireDigit && item.All(c => !IsDigit(c)))
                stringList.Add(I18N.Translate("Password must have at least one digit ('0'-'9')."));
            if (RequireLowercase && item.All(c => !IsLower(c)))
                stringList.Add(I18N.Translate("Password must have at least one lowercase ('a'-'z')."));
            if (RequireUppercase && item.All(c => !IsUpper(c)))
                stringList.Add(I18N.Translate("Password must have at least one uppercase ('A'-'Z')."));
            if (stringList.Count == 0)
                return Task.FromResult(IdentityResult.Success);
            return Task.FromResult(IdentityResult.Failed(string.Join(" ", stringList)));

        }
    }
}
