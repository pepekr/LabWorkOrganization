using LabWorkOrganization.Domain.Intefaces;
using System.Security.Cryptography;

namespace LabWorkOrganization.Infrastructure.Auth
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;

        public string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            using Rfc2898DeriveBytes pbkdf2 = new(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(KeySize);
            return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
        }

        public bool VerifyPassword(string password, string storedHashWithSalt)
        {
            string[] parts = storedHashWithSalt.Split('.');
            if (parts.Length != 2)
            {
                return false;
            }

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHash = Convert.FromBase64String(parts[1]);

            using Rfc2898DeriveBytes pbkdf2 = new(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(KeySize);

            return hash.SequenceEqual(storedHash);
        }
    }
}
