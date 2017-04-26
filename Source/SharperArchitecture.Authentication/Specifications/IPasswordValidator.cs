using Microsoft.AspNet.Identity;

namespace SharperArchitecture.Authentication.Specifications
{
    public interface IPasswordValidator : IIdentityValidator<string>
    {
        /// <summary>Minimum required length</summary>
        int RequiredLength { get; set; }

        /// <summary>Require a non letter or digit character</summary>
        bool RequireNonLetterOrDigit { get; set; }

        /// <summary>Require a lower case letter ('a' - 'z')</summary>
        bool RequireLowercase { get; set; }

        /// <summary>Require an upper case letter ('A' - 'Z')</summary>
        bool RequireUppercase { get; set; }

        /// <summary>Require a digit ('0' - '9')</summary>
        bool RequireDigit { get; set; }
    }
}