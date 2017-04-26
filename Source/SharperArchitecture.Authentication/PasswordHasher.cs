using Microsoft.AspNet.Identity;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.Authentication
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly ICryptography _cryptography;

        public PasswordHasher(ICryptography cryptography)
        {
            _cryptography = cryptography;
        }

        public string HashPassword(string password)
        {
            return _cryptography.Encrypt(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return _cryptography.IsDecryptSupported
                ? (_cryptography.Decrypt(hashedPassword) == providedPassword
                    ? PasswordVerificationResult.Success
                    : PasswordVerificationResult.Failed
                    )
                : (_cryptography.Encrypt(providedPassword) == hashedPassword
                    ? PasswordVerificationResult.Success
                    : PasswordVerificationResult.Failed
                    )
                ;
        }
    }
}
