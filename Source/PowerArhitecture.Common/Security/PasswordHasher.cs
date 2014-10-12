using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Specifications;
using Microsoft.AspNet.Identity;

namespace PowerArhitecture.Common.Security
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
            return _cryptography.Decrypt(hashedPassword) == providedPassword
                ? PasswordVerificationResult.Success
                : PasswordVerificationResult.Failed;
        }
    }
}
