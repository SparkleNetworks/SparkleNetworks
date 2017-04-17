
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;

    public interface ICryptoService
    {
        /// <summary>
        /// Low-level cryptopurpose API. Returns the <see cref="HashAlgorithm"/> that is required for a given cryptographic PURPOSE.
        /// </summary>
        /// <param name="purpose"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">Cannot resolve the class type for the desired purpose; The specified purpose does not exist or does not have the desired value</exception>
        /// <returns></returns>
        HashAlgorithm GetHashAlgorithmByPurpose(string purpose);

        /// <summary>
        /// Low-level cryptopurpose API. Returns the configured static noise that is required for a given cryptographic PURPOSE.
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="expectedByteLength">the number of bytes you expect to be given</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">The specified purpose does not exist or does not have the desired value ; There are not enough bytes in the configured static noise</exception>
        /// <returns></returns>
        byte[] GetStaticNoiseByPurpose(string purpose, int expectedByteLength);

        /// <summary>
        /// High-level cryptopurpose API. Computes the static hash for a given user and a given crypto purpose.
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="userId"></param>
        /// <param name="dateUserCreatedUtc"></param>
        /// <exception cref="InvalidOperationException">Something went wrong</exception>
        /// <returns></returns>
        byte[] ComputeStaticUserHash(string purpose, int userId, DateTime dateUserCreatedUtc);
    }
}
