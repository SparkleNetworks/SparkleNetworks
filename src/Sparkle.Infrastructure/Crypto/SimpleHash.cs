
#if !SSC
namespace Sparkle.Infrastructure.Crypto
{
    using System;
    using System.Text;
    using System.Security.Cryptography;
    using System.Security;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;
    using System.Diagnostics;

    /// <summary>
    /// This class generates and compares hashes using MD5, SHA1, SHA256, SHA384, 
    /// and SHA512 hashing algorithms. Before computing a hash, it appends a
    /// randomly generated salt to the plain text, and stores this salt appended
    /// to the result. To verify another plain text value against the given hash,
    /// this class will retrieve the salt value from the hash string and use it
    /// when computing a new hash of the plain text. Appending a salt value to
    /// the hash may not be the most efficient approach, so when using hashes in
    /// a real-life application, you may choose to store them separately. You may
    /// also opt to keep results as byte arrays instead of converting them into
    /// base64-encoded strings.
    /// </summary>
    public class SimpleHash
    {
        /// <summary>
        /// Generates a hash for the given plain text value and returns a
        /// base64-encoded result. Before the hash is computed, a random salt
        /// is generated and appended to the plain text. This salt is stored at
        /// the end of the hash value, so it can be used later for hash
        /// verification.
        /// </summary>
        /// <param name="plainText">
        /// Plaintext value to be hashed. The function does not check whether
        /// this parameter is null.
        /// </param>
        /// <param name="saltBytes">
        /// Salt bytes. This parameter can be null, in which case a random salt
        /// value will be generated.
        /// </param>
        /// <returns>
        /// Hash value formatted as a base64-encoded string.
        /// </returns>
        public static string ComputeHash(string plainText, byte[] saltBytes, Func<HashAlgorithm> algoFactory)
        {
            if (plainText == null)
                throw new ArgumentNullException("plainText");
            if (algoFactory == null)
                throw new ArgumentNullException("algoFactory");

            // If salt is not specified, generate it on the fly.
            if (saltBytes == null)
            {
                // Define min and max salt sizes.
                int minSaltSize = 4;
                int maxSaltSize = 8;

                // Generate a random number for the size of the salt.
                Random random = new Random();
                int saltSize = random.Next(minSaltSize, maxSaltSize);

                // Allocate a byte array, which will hold the salt.
                saltBytes = new byte[saltSize];

                // Initialize a random number generator.
                using (var rng = new RNGCryptoServiceProvider())
                    // Fill the salt with cryptographically strong byte values.
                    rng.GetNonZeroBytes(saltBytes);
            }

            // Convert plain text into a byte array.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Allocate array, which will hold plain text and salt.
            byte[] plainTextWithSaltBytes = new byte[plainTextBytes.Length + saltBytes.Length];

            // Copy plain text bytes into resulting array.
            for (int i = 0; i < plainTextBytes.Length; i++)
                plainTextWithSaltBytes[i] = plainTextBytes[i];

            // Append salt bytes to the resulting array.
            for (int i = 0; i < saltBytes.Length; i++)
                plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];

            // Because we support multiple hashing algorithms, we must define
            // hash object as a common (abstract) base class. We will specify the
            // actual hashing algorithm class later during object creation.
            HashAlgorithm hash;
            byte[] hashBytes = null;
            using (hash = algoFactory())
            {
                // Compute hash value of our plain text with appended salt.
                hashBytes = hash.ComputeHash(plainTextWithSaltBytes);
            }

            // Create array which will hold hash and original salt bytes.
            byte[] hashWithSaltBytes = new byte[hashBytes.Length + saltBytes.Length];

            // Copy hash bytes into resulting array.
            for (int i = 0; i < hashBytes.Length; i++)
                hashWithSaltBytes[i] = hashBytes[i];

            // Append salt bytes to the result.
            for (int i = 0; i < saltBytes.Length; i++)
                hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];

            // Convert result into a base64-encoded string.
            string hashValue = Convert.ToBase64String(hashWithSaltBytes);

            // Return the result.
            return hashValue;
        }

        /// <summary>
        /// Generates a hash for the given plain text value and returns a
        /// base64-encoded result. Before the hash is computed, a random salt
        /// is generated and appended to the plain text. This salt is stored at
        /// the end of the hash value, so it can be used later for hash
        /// verification.
        /// </summary>
        /// <param name="plainText">
        /// Plaintext value to be hashed. The function does not check whether
        /// this parameter is null.
        /// </param>
        /// <param name="saltBytes">
        /// Salt bytes. This parameter can be null, in which case a random salt
        /// value will be generated.
        /// </param>
        /// <returns>
        /// Hash value formatted as a base64-encoded string.
        /// </returns>
        public static string ComputeHash(SecureString plainText, byte[] saltBytes, Func<HashAlgorithm> algoFactory)
        {
            if (plainText == null)
                throw new ArgumentNullException("plainText");
            if (algoFactory == null)
                throw new ArgumentNullException("algoFactory");

            IntPtr pass1 = default(IntPtr);
            try
            {
                pass1 = Marshal.SecureStringToBSTR(plainText);
                return ComputeHash(Marshal.PtrToStringBSTR(pass1), saltBytes, algoFactory);
            }
            catch
            {
                return ComputeHash("", saltBytes, algoFactory);
            }
        }

        /// <summary>
        /// Compares a hash of the specified plain text value to a given hash
        /// value. Plain text is hashed with the same salt value as the original
        /// hash.
        /// </summary>
        /// <param name="plainText">
        /// Plain text to be verified against the specified hash. The function
        /// does not check whether this parameter is null.
        /// </param>
        /// <param name="hashValue">
        /// Base64-encoded hash value produced by ComputeHash function. This value
        /// includes the original salt appended to it.
        /// </param>
        /// <returns>
        /// If computed hash mathes the specified hash the function the return
        /// value is true; otherwise, the function returns false.
        /// </returns>
        public static bool VerifyHash(string plainText, string hashValue, Func<HashAlgorithm> algoFactory)
        {
            byte[] saltBytes = null;
            try
            {
                saltBytes = _extractHash(hashValue, algoFactory);
            }
            catch (ArgumentException)
            {
                return false;
            }

            // Compute a new hash string.
            string expectedHashString = ComputeHash(plainText, saltBytes, algoFactory);

            // If the computed hash matches the specified hash,
            // the plain text value must be correct.
            return (hashValue == expectedHashString);
        }

        /// <summary>
        /// Compares a hash of the specified plain text value to a given hash
        /// value. Plain text is hashed with the same salt value as the original
        /// hash.
        /// </summary>
        /// <param name="plainText">
        /// Plain text to be verified against the specified hash. The function
        /// does not check whether this parameter is null.
        /// </param>
        /// <param name="hashValue">
        /// Base64-encoded hash value produced by ComputeHash function. This value
        /// includes the original salt appended to it.
        /// </param>
        /// <returns>
        /// If computed hash mathes the specified hash the function the return
        /// value is true; otherwise, the function returns false.
        /// </returns>
        public static bool VerifyHash(SecureString plainText, string hashValue, Func<HashAlgorithm> algoFactory)
        {
            byte[] saltBytes = null;
            try
            {
                saltBytes = _extractHash(hashValue, algoFactory);
            }
            catch (ArgumentException)
            {
                return false;
            }

            string expectedHashString = ComputeHash(plainText, saltBytes, algoFactory);

            // If the computed hash matches the specified hash,
            // the plain text value must be correct.
            return (hashValue == expectedHashString);

        }

        private static byte[] _extractHash(string hashValue, Func<HashAlgorithm> algoFactory)
        {
            // Convert base64-encoded hash value into a byte array.
            byte[] hashWithSaltBytes = Convert.FromBase64String(hashValue);

            // We must know size of hash (without salt).
            int hashSizeInBits, hashSizeInBytes;
            using (var hash = algoFactory())
                hashSizeInBits = hash.HashSize;

            // Convert size of hash from bits to bytes.
            hashSizeInBytes = hashSizeInBits / 8;

            // Make sure that the specified hash value is long enough.
            if (hashWithSaltBytes.Length < hashSizeInBytes)
                throw new ArgumentException();

            // Allocate array to hold original salt bytes retrieved from hash.
            byte[] saltBytes = new byte[hashWithSaltBytes.Length -
                                        hashSizeInBytes];

            // Copy salt from the end of the hash to the new array.
            for (int i = 0; i < saltBytes.Length; i++)
                saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];
            return saltBytes;
        }
    }

    public static class MultiHash
    {
        public static string OomanChallenge()
        {
            return Guid.NewGuid().ToString();
        }

        public static string OomanValue(string challenge)
        {
            var start = DateTime.UtcNow;
            var salt1 = new byte[] { 87, 53, 214, 2, 123, 36, };
            var salt2 = new byte[] { 45, 156, 254, 23, 8, 7, };

            string hashed = DoSHA1(challenge, salt1, 3);
            hashed = DoReverse(hashed);
            hashed = DoSHA1(challenge, salt2, 3);

            var time = DateTime.UtcNow - start;
            Trace.TraceInformation("MultiHash.OomanValue took " + time.Milliseconds + " ms to complete");

            return hashed;
        }

        public static bool OomanCheck(string challenge, string value)
        {
            if (string.IsNullOrEmpty(challenge) || string.IsNullOrEmpty(value))
                return false;

            string hashed = OomanValue(challenge);

            return hashed.Trim() == value.Trim();
        }

        public static string EmailSecret(string challenge)
        {
            var start = DateTime.UtcNow;
            var salt1 = new byte[] { 87, 2, 123, 156, 254, 36, };
            var salt2 = new byte[] { 45, 23, 53, 214, 8, 7, };

            string hashed = DoSHA1(challenge, salt1, 4);
            hashed = DoReverse(hashed);
            hashed = DoSHA1(challenge, salt2, 3);

            var time = DateTime.UtcNow - start;
            Trace.TraceInformation("MultiHash.EmailSecret took " + time.Milliseconds + " ms to complete");

            return hashed;
        }

        public static bool EmailSecretCheck(string challenge, string secret)
        {
            if (string.IsNullOrEmpty(challenge) || string.IsNullOrEmpty(secret))
                return false;

            string hashed = EmailSecret(challenge);

            return hashed.Trim() == secret.Trim();
        }

        private static string DoReverse(string value)
        {
            var chars = new char[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                chars[value.Length - i - 1] = value[i];
            }

            return new string(chars);
        }

        private static string DoSHA1(string challenge, byte[] saltBytes, int times)
        {
            Func<HashAlgorithm> algo = () => new SHA1Managed();
            string value = challenge;
            for (int i = 0; i < times; i++)
            {
                value = SimpleHash.ComputeHash(value, saltBytes, algo);
            }

            return value;
        }
    }
}
#endif
