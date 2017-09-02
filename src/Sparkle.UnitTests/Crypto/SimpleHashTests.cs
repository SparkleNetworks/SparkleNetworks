
namespace Sparkle.UnitTests.Crypto
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sparkle.Infrastructure.Crypto;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Security.Cryptography;
    using System.Text;

    [TestClass]
    public class SimpleHashTests
    {
        [TestClass]
        public class ComputeHashMethod
        {
            [TestMethod, ExpectedException(typeof(ArgumentNullException))]
            public void Arg0NullThrows()
            {
                string value = null;
                byte[] salt = new byte[0];
                Func<HashAlgorithm> factory = new Func<HashAlgorithm>(() => new SHA1Managed());
                SimpleHash.ComputeHash(value, salt, factory);
            }

            [TestMethod, ExpectedException(typeof(ArgumentNullException))]
            public void Arg2NullThrows()
            {
                string value = "hello";
                byte[] salt = new byte[0];
                Func<HashAlgorithm> factory = null;
                SimpleHash.ComputeHash(value, salt, factory);
            }

            [TestMethod, ExpectedException(typeof(ArgumentNullException))]
            public void Arg0NullThrows_Secure()
            {
                SecureString value = null;
                byte[] salt = new byte[0];
                Func<HashAlgorithm> factory = new Func<HashAlgorithm>(() => new SHA1Managed());
                SimpleHash.ComputeHash(value, salt, factory);
            }

            [TestMethod, ExpectedException(typeof(ArgumentNullException))]
            public void Arg2NullThrows_Secure()
            {
                SecureString value = ToSecureString("hello");
                byte[] salt = new byte[0];
                Func<HashAlgorithm> factory = null;
                SimpleHash.ComputeHash(value, salt, factory);
            }

            [TestMethod]
            public void CanProduceSameHashWithSameSalt()
            {
                string value = "hello world";
                byte[] salt = new byte[] { 11, 159, 227, 106, 223 };
                Func<HashAlgorithm> factory = new Func<HashAlgorithm>(() => new SHA1Managed());
                string expected = "UoBzD/M5eC3f7NzZtJ5u8gLf3IYLn+Nq3w==";
                var result = SimpleHash.ComputeHash(value, salt, factory);
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void GeneratesSaltWhenNoSaltGiven()
            {
                string value = "hello world";
                byte[] salt = null;
                Func<HashAlgorithm> factory = new Func<HashAlgorithm>(() => new SHA1Managed());
                var result = SimpleHash.ComputeHash(value, salt, factory);
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Length > value.Length);
            }

            [TestMethod]
            public void BackAndForthNoSalt()
            {
                string value = "hello world";
                byte[] salt = null;
                Func<HashAlgorithm> factory = new Func<HashAlgorithm>(() => new SHA1Managed());
                var hashResult = SimpleHash.ComputeHash(value, salt, factory);
                var verifyResult = SimpleHash.VerifyHash(value, hashResult, factory);
                Assert.IsTrue(verifyResult);
            }

            [TestMethod]
            public void BackAndForthWithSalt()
            {
                string value = "hello world";
                byte[] salt = new byte[] { 45, 125, 200, 1, 32, 85, 45, 100 };
                Func<HashAlgorithm> factory = new Func<HashAlgorithm>(() => new SHA1Managed());
                var hashResult = SimpleHash.ComputeHash(value, salt, factory);
                var verifyResult = SimpleHash.VerifyHash(value, hashResult, factory);
                Assert.IsTrue(verifyResult);
            }
        }

        protected static SecureString ToSecureString(string value)
        {
            var secure = new SecureString();
            for (int i = 0; i < value.Length; i++)
            {
                secure.InsertAt(i, value[i]);
            }

            return secure;
        }
    }
}
