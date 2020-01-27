using System;
using System.Linq;
using System.Security.Cryptography;

namespace Exchange.Utils
{
    public class PasswordUtil
    {
        private const int HashLength = 128;
        private const int Iterations = 100;
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;
        public static byte[] GenerateSalt()
        {
            var salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }

        public static string HashPassword(string password)
        {
            var salt = GenerateSalt();
            using var algorithm = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithm);
            return $"{Convert.ToBase64String(algorithm.GetBytes(HashLength))}.{Convert.ToBase64String(salt)}";
        }

        public static bool PasswordEqual(string providedPassword, string storedPasswordHash)
        {
            var (passwordHash, salt) = RestorePassword(storedPasswordHash);
            using var algorithm = new Rfc2898DeriveBytes(providedPassword, salt, Iterations, HashAlgorithm);
            return algorithm.GetBytes(HashLength).SequenceEqual(passwordHash);
        }

        private static (byte[] Hash, byte[] Salt) RestorePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException($"Provided password is empty: '{password ?? "null"}'");
            }

            var passwordInfo = password.Split(".");
            if (passwordInfo.Length != 2)
            {
                throw new ArgumentException("Provided password is not of correct type " + password);
            }

            return (Convert.FromBase64String(passwordInfo[0]), Convert.FromBase64String(passwordInfo[1]));
        }
    }
}
