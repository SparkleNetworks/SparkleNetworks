
#if !SSC
namespace Sparkle.Infrastructure.Crypto
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public static class SimpleCrypt
    {
        private static int _iterations = 2;
        private static int _keySize = 256;
        private static string _hash = "SHA1";
        private static string _salt = "as8947az38490a32";
        private static string _vector = "awl34k4elrias3jq";

        public static string Encrypt(string value, string password, bool asUrlSafeBase64)
        {
            return Encrypt<AesManaged>(value, password, asUrlSafeBase64);
        }

        public static byte[] Encrypt(byte[] value, string password)
        {
            return Encrypt<AesManaged>(value, password);
        }

        public static string Encrypt<T>(string value, string password, bool asUrlSafeBase64)
                where T : SymmetricAlgorithm, new()
        {
            byte[] valueBytes = Encoding.UTF8.GetBytes(value);
            byte[] encrypted = Encrypt<T>(valueBytes, password);

            return ToBase64String(encrypted, asUrlSafeBase64);
        }

        public static byte[] Encrypt<T>(byte[] value, string password) where T : SymmetricAlgorithm, new()
        {
            byte[] vectorBytes = Encoding.ASCII.GetBytes(_vector);
            byte[] saltBytes = Encoding.ASCII.GetBytes(_salt);

            byte[] encrypted;
            using (T cipher = new T())
            {
                PasswordDeriveBytes _passwordBytes =
                    new PasswordDeriveBytes(password, saltBytes, _hash, _iterations);
                byte[] keyBytes = _passwordBytes.GetBytes(_keySize / 8);

                cipher.Mode = CipherMode.CBC;

                using (ICryptoTransform encryptor = cipher.CreateEncryptor(keyBytes, vectorBytes))
                {
                    using (MemoryStream to = new MemoryStream())
                    {
                        using (CryptoStream writer = new CryptoStream(to, encryptor, CryptoStreamMode.Write))
                        {
                            writer.Write(value, 0, value.Length);
                            writer.FlushFinalBlock();
                            encrypted = to.ToArray();
                        }
                    }
                }

                cipher.Clear();
            }

            return encrypted;
        }

        private static string ToBase64String(byte[] encrypted, bool asUrlSafeBase64)
        {
            if (asUrlSafeBase64)
            {
                return
                    Convert.ToBase64String(encrypted, Base64FormattingOptions.None)
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .Replace("=", "%3D");
            }
            else
            {
                return Convert.ToBase64String(encrypted, Base64FormattingOptions.None);
            }
        }

        private static byte[] FromBase64String(string value)
        {
            return Convert.FromBase64String(
                value
                .Replace('-', '+')
                .Replace('_', '/')
                .Replace("%3D", "="));
        }

        /// <exception cref="FormatException">Invalid base64 string</exception>
        public static string Decrypt(string value, string password)
        {
            return Decrypt<AesManaged>(value, password);
        }

        public static byte[] Decrypt(byte[] value, string password)
        {
            int decryptedByteCount;
            return Decrypt<AesManaged>(value, password, out decryptedByteCount);
        }

        public static string Decrypt<T>(string value, string password) where T : SymmetricAlgorithm, new()
        {
            byte[] valueBytes = FromBase64String(value);
            int decryptedByteCount;
            var decrypted = Decrypt<T>(valueBytes, password, out decryptedByteCount);
            return Encoding.UTF8.GetString(decrypted, 0, decryptedByteCount);
        }

        private static byte[] Decrypt<T>(byte[] valueBytes, string password, out int decryptedByteCount) where T : SymmetricAlgorithm, new()
        {
            byte[] vectorBytes = Encoding.ASCII.GetBytes(_vector);
            byte[] saltBytes = Encoding.ASCII.GetBytes(_salt);

            byte[] decrypted;
            decryptedByteCount = 0;

            using (T cipher = new T())
            {
                PasswordDeriveBytes _passwordBytes = new PasswordDeriveBytes(password, saltBytes, _hash, _iterations);
                byte[] keyBytes = _passwordBytes.GetBytes(_keySize / 8);

                cipher.Mode = CipherMode.CBC;

                try
                {
                    using (ICryptoTransform decryptor = cipher.CreateDecryptor(keyBytes, vectorBytes))
                    {
                        using (MemoryStream from = new MemoryStream(valueBytes))
                        {
                            using (CryptoStream reader = new CryptoStream(from, decryptor, CryptoStreamMode.Read))
                            {
                                decrypted = new byte[valueBytes.Length];
                                decryptedByteCount = reader.Read(decrypted, 0, decrypted.Length);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }

                cipher.Clear();
            }

            return decrypted;
        }
    }
}
#endif
