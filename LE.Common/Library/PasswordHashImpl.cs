using System;
using System.Security.Cryptography;

namespace LE.Common.Library
{
    public class PasswordHashImpl : PasswordHash
    {
        // Legacy format (created by this class before hardening): "iterations:salt:hash"
        // written with 1000 iterations of PBKDF2-HMAC-SHA1. New hashes use the
        // versioned format "v2:salt:hash" with PBKDF2-HMAC-SHA256 and a high
        // iteration count. Legacy hashes remain verifiable and are upgraded
        // transparently by callers on next successful login.
        public const int SALT_BYTE_SIZE = 16;
        public const int HASH_BYTE_SIZE = 32;
        public const int PBKDF2_ITERATIONS = 310000;
        public const string CURRENT_FORMAT = "v2";

        public string CreateHash(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[SALT_BYTE_SIZE];
            using (var csprng = RandomNumberGenerator.Create())
            {
                csprng.GetBytes(salt);
            }

            byte[] hash = PBKDF2(password, salt, PBKDF2_ITERATIONS, HASH_BYTE_SIZE, new HashAlgorithmName("SHA256"));
            return CURRENT_FORMAT + ":" +
                Convert.ToBase64String(salt) + ":" +
                Convert.ToBase64String(hash);
        }

        public bool ValidatePassword(string password, string correct_hash)
        {
            if (string.IsNullOrEmpty(correct_hash))
            {
                return false;
            }

            string[] split = correct_hash.Split(':');
            if (split.Length < 3)
            {
                return false;
            }

            if (split[0] == CURRENT_FORMAT)
            {
                // Current format: v2:salt:hash (PBKDF2-HMAC-SHA256)
                byte[] salt = Convert.FromBase64String(split[1]);
                byte[] hash = Convert.FromBase64String(split[2]);
                byte[] testHash = PBKDF2(password, salt, PBKDF2_ITERATIONS, hash.Length, new HashAlgorithmName("SHA256"));
                return SlowEquals(hash, testHash);
            }

            // Legacy format: iterations:salt:hash (PBKDF2-HMAC-SHA1)
            if (!int.TryParse(split[0], out int iterations) || iterations <= 0)
            {
                return false;
            }
            try
            {
                byte[] salt = Convert.FromBase64String(split[1]);
                byte[] hash = Convert.FromBase64String(split[2]);
                byte[] testHash = PBKDF2(password, salt, iterations, hash.Length, new HashAlgorithmName("SHA1"));
                return SlowEquals(hash, testHash);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// True when the stored hash uses the legacy low-iteration format and should be
        /// re-hashed with current parameters after a successful validation.
        /// </summary>
        public bool NeedsRehash(string correct_hash)
        {
            if (string.IsNullOrEmpty(correct_hash))
            {
                return true;
            }
            string[] split = correct_hash.Split(':');
            return split.Length < 3 || split[0] != CURRENT_FORMAT;
        }

        private bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }

        private byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes, HashAlgorithmName hashAlgorithm)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, hashAlgorithm))
            {
                return pbkdf2.GetBytes(outputBytes);
            }
        }
    }
}
