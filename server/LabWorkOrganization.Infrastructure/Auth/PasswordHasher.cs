using LabWorkOrganization.Domain.Intefaces;
using System.Security.Cryptography;

namespace LabWorkOrganization.Infrastructure.Auth
{
    // Provides secure password hashing and verification using PBKDF2 with SHA256
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;          // Size of the random salt in bytes
        private const int KeySize = 32;           // Size of the derived hash in bytes
        private const int Iterations = 100_000;   // Number of PBKDF2 iterations for security

        // Generates a hashed password in the format: {salt}.{hash}
        public string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            using Rfc2898DeriveBytes pbkdf2 = new(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(KeySize);

            // Combine salt and hash as a base64 string separated by '.'
            return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
        }

        // Verifies a password against a stored hash
        public bool VerifyPassword(string password, string storedHashWithSalt)
        {
            var parts = storedHashWithSalt.Split('.');
            if (parts.Length != 2) return false; // Invalid format

            byte[] salt = Convert.FromBase64String(parts[0]);       // Extract salt
            byte[] storedHash = Convert.FromBase64String(parts[1]); // Extract stored hash

            // Recompute hash using provided password and extracted salt
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(KeySize);

            // Compare computed hash with stored hash
            return hash.SequenceEqual(storedHash); // Returns true if password is correct
        }
    }
}
