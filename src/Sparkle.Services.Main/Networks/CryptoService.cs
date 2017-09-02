
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Services.Networks;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public class CryptoService : ServiceBase, ICryptoService
    {
        [DebuggerStepThrough]
        public CryptoService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
            using (new AesManaged())
            {
            }
        }

        public byte[] ComputeStaticUserHash(string purpose, int userId, DateTime dateUserCreatedUtc)
        {
            if (purpose == null)
                throw new ArgumentNullException("purpose");

            using (HashAlgorithm crypto = this.GetHashAlgorithmByPurpose(purpose))
            {
                const int size = 256; // in bytes
                byte[] data = this.GetStaticNoiseByPurpose(purpose, size);

                var userIdData = BitConverter.GetBytes(userId);
                int index = 127;
                Array.Copy(userIdData, 0, data, index, userIdData.Length);
                var userDateData = Encoding.ASCII.GetBytes(dateUserCreatedUtc.ToString("o", CultureInfo.InvariantCulture));
                index += userDateData.Length;
                Array.Copy(userDateData, 0, data, 127 + sizeof(long), userDateData.Length);

                var userToken = crypto.ComputeHash(data);
                return userToken;
            }
        }

        public HashAlgorithm GetHashAlgorithmByPurpose(string purpose)
        {
            if (purpose == null)
                throw new ArgumentNullException("purpose");

            var configKey = "Security.Purposes." + purpose + ".HashAlgorithmTypeName";
            if (this.Services.AppConfiguration.Values.ContainsKey(configKey))
            {
                var value = this.Services.AppConfiguration.Values[configKey];
                var typeName = value.RawValue ?? value.DefaultRawValue;

                if (!string.IsNullOrEmpty(typeName))
                {
                    Type type;
                    try
                    {
                        type = Type.GetType(typeName, true);
                    }
                    catch (TypeLoadException ex)
                    {
                        throw new InvalidOperationException("Cannot resolve the class type for the desired purpose: " + ex.Message);
                    }

                    var algorithm = (HashAlgorithm)Activator.CreateInstance(type);
                    return algorithm;
                }
                else
                {
                    throw new InvalidOperationException("The specified purpose does not have the desired value. ");
                }
            }
            else
            {
                throw new InvalidOperationException("The specified purpose does not exist. ");
            }
        }

        public SymmetricAlgorithm GetSymmetricAlgorithmByPurpose(string purpose)
        {
            if (purpose == null)
                throw new ArgumentNullException("purpose");

            var configKey1 = "Security.Purposes." + purpose + ".SymmetricAlgorithmTypeName";
            var configKey2 = "Security.Purposes." + purpose + ".Key";
            if (this.Services.AppConfiguration.Values.ContainsKey(configKey1) && this.Services.AppConfiguration.Values.ContainsKey(configKey2))
            {
                var value = this.Services.AppConfiguration.Values[configKey1];
                var typeName = value.RawValue ?? value.DefaultRawValue;
                var key = this.Services.AppConfiguration.Values[configKey2];

                {
                    var algorithm = new AesManaged();
                    algorithm.Key = Convert.FromBase64String(key.RawValue);
                    return algorithm;
                }
                /*
                 * this code does not work for a strange reason
                 * Could not load type 'System.Security.Cryptography.AesManaged' from assembly 'Sparkle.Services.Main, Version=1.0.0.140, Culture=neutral, PublicKeyToken=null'.
                 * 
                if (!string.IsNullOrEmpty(typeName))
                {
                    Type type;
                    try
                    {
                        type = Type.GetType(typeName, true);
                    }
                    catch (TypeLoadException ex)
                    {
                        throw new InvalidOperationException("Cannot resolve the class type for the desired purpose: " + ex.Message);
                    }

                    var algorithm = (SymmetricAlgorithm)Activator.CreateInstance(type);
                    algorithm.Key = Convert.FromBase64String(key.RawValue);
                    return algorithm;
                }
                else
                {
                    throw new InvalidOperationException("The specified purpose does not have the desired value. ");
                }*/
            }
            else
            {
                throw new InvalidOperationException("The specified purpose does not exist. ");
            }
        }

        public byte[] GetStaticNoiseByPurpose(string purpose, int expectedByteLength)
        {
            if (purpose == null)
                throw new ArgumentNullException("purpose");

            var configKey = "Security.Purposes." + purpose + ".StaticNoise";
            if (this.Services.AppConfiguration.Values.ContainsKey(configKey))
            {
                var value = this.Services.AppConfiguration.Values[configKey];
                var base64 = value.RawValue ?? value.DefaultRawValue;
                if (!string.IsNullOrEmpty(base64))
                {
                    var data = Convert.FromBase64String(base64);

                    if (data.Length == expectedByteLength)
                    {
                        return data;
                    }
                    else if (data.Length > expectedByteLength)
                    {
                        var partOfData = new byte[expectedByteLength];
                        Array.Copy(data, partOfData, expectedByteLength);
                        return partOfData;
                    }
                    else
                    {
                        throw new InvalidOperationException("The expected value is too short: " + data.Length + " bytes instead of " + expectedByteLength + ". ");
                    }
                }
                else
                {
                    throw new InvalidOperationException("The specified purpose does not have the desired value. ");
                }
            }
            else
            {
                throw new InvalidOperationException("The specified purpose does not exist. ");
            }
        }

        public byte[] EncryptWithIV(string purpose, byte[] data)
        {
            using (var algo = this.GetSymmetricAlgorithmByPurpose(purpose))
            {
                algo.GenerateIV();
                var iv = algo.IV;
                algo.Mode = CipherMode.CBC;
                using (var encryptor = algo.CreateEncryptor())
                using (MemoryStream memory = new MemoryStream())
                {
                    memory.Write(iv, 0, iv.Length);
                    using (CryptoStream writer = new CryptoStream(memory, encryptor, CryptoStreamMode.Write))
                    {
                        writer.Write(data, 0, data.Length);
                        writer.FlushFinalBlock();
                        var encrypted = memory.ToArray();
                        return encrypted;
                    }
                }
            }
        }

        public byte[] DecryptWithIV(string purpose, byte[] data)
        {
            using (var algo = this.GetSymmetricAlgorithmByPurpose(purpose))
            {
                var ivLength = algo.BlockSize / 8;
                var iv = new byte[ivLength];
                Array.Copy(data, iv, ivLength);
                algo.IV = iv;
                algo.Mode = CipherMode.CBC;
                using (var decryptor = algo.CreateDecryptor())
                using (MemoryStream memory = new MemoryStream())
                {
                    memory.Write(data, ivLength, data.Length - ivLength);
                    memory.Seek(0L, SeekOrigin.Begin);
                    using (CryptoStream reader = new CryptoStream(memory, decryptor, CryptoStreamMode.Read))
                    {
                        var decrypted = new byte[data.Length - ivLength];
                        var decryptedLength = reader.Read(decrypted, 0, decrypted.Length);
                        var result = new byte[decryptedLength];
                        Array.Copy(decrypted, result, decryptedLength);
                        return result;
                    }
                }
            }
        }
    }
}
