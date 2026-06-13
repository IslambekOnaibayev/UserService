using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int Iterations = 100_000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        public string Hash(string password)
        {
            var salt = System.Security.Cryptography.RandomNumberGenerator.GetBytes(SaltSize);
            var hash = System.Security.Cryptography.Rfc2898DeriveBytes.Pbkdf2(
                password, salt, Iterations,
                System.Security.Cryptography.HashAlgorithmName.SHA256, HashSize);
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool Verify(string passwordHash, string providedPassword)
        {
            var parts = passwordHash.Split('.');
            if (parts.Length != 2) return false;
            try
            {
                var salt = Convert.FromBase64String(parts[0]);
                var hash = Convert.FromBase64String(parts[1]);
                var provided = System.Security.Cryptography.Rfc2898DeriveBytes.Pbkdf2(
                    providedPassword, salt, Iterations,
                    System.Security.Cryptography.HashAlgorithmName.SHA256, HashSize);
                return System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(hash, provided);
            }
            catch { return false; }
        }
    }
}
