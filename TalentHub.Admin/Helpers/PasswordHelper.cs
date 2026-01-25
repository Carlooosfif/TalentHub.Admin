using System;
using System.Security.Cryptography;

namespace TalentHub.Admin.Helpers
{
    public static class PasswordHelper
    {
        // Genera hash + salt usando PBKDF2
        public static (string Hash, string Salt) HashPassword(string password)
        {
            // 16 bytes de salt
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            // 100 000 iteraciones
            using (var rfc2898 = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = rfc2898.GetBytes(32); // 256 bits
                string hash = Convert.ToBase64String(hashBytes);
                string salt = Convert.ToBase64String(saltBytes);
                return (hash, salt);
            }
        }

        // Valida password contra hash+salt guardados
        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);

            using (var rfc2898 = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = rfc2898.GetBytes(32);
                string computedHash = Convert.ToBase64String(hashBytes);
                return computedHash == storedHash;
            }
        }
    }
}
