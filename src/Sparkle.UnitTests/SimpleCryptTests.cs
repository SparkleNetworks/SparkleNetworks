
namespace Sparkle.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sparkle.Infrastructure.Crypto;
    using Sparkle.Services.Networks.Objects;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class SimpleCryptTests
    {
        [TestClass]
        public class EncryptMethod
        {
            [TestMethod]
            public void EncryptDecrypt_SameString()
            {
                var value = "azertyuiopqsdghjklmwxcvbn";
                var password = "0123456789";

                // execute
                var encrypted = SimpleCrypt.Encrypt(value, password, false);
                var decrypted = SimpleCrypt.Decrypt(encrypted, password);

                Assert.AreEqual(value, decrypted);
            }

            [TestMethod]
            public void EncryptDecrypt_SameString2()
            {
                var value = "azertyuiopqsdghjklmwxcvbazertyuiopqsdghjklmwxcvbnazertyuiopqsdghjklmwxcvbnazertyuiopqsdghjklmwxcvbnazertyuiopqsdghjklmwxcvbnazertyuiopqsdghjklmwxcvbnazertyuiopqsdghjklmwxcvbnazertyuiopqsdghjklmwxcvbnazertyuiopqsdghjklmwxcvbnazertyuiopqsdghjklmwxcvbnn";
                var password = "azertyuiopqsdghjklmwxcvbn0123456789azertyuiopqsdghjklmwxcvbn";

                // execute
                var encrypted = SimpleCrypt.Encrypt(value, password, false);
                var decrypted = SimpleCrypt.Decrypt(encrypted, password);

                Assert.AreEqual(value, decrypted);
            }

            [TestMethod]
            public void StatsCounterHitLink1()
            {
                var entity = new StatsCounterHitLink(
                    "CounterCategory", "CounterName", 123456,
                    "identifier", 523, 145);

                var encoded = entity.ToEncryptedJson();
                var decoded = StatsCounterHitLink.FromEncryptedJson(encoded);

                Assert.AreEqual(entity.Id, decoded.Id);
                ////Assert.AreEqual(entity.Category, decoded.Category);
                ////Assert.AreEqual(entity.Name, decoded.Name);
                Assert.AreEqual(entity.NetworkId, decoded.NetworkId);
                Assert.AreEqual(entity.UserId, decoded.UserId);
                Assert.AreEqual(entity.Identifier, decoded.Identifier);
            }

            [TestMethod]
            public void StatsCounterHitLink2()
            {
                var entity = new StatsCounterHitLink(
                    "CounterCategory", "CounterName", 123456,
                    null, null, null);

                var encoded = entity.ToEncryptedJson();
                var decoded = StatsCounterHitLink.FromEncryptedJson(encoded);

                Assert.AreEqual(entity.Id, decoded.Id);
                ////Assert.AreEqual(entity.Category, decoded.Category);
                ////Assert.AreEqual(entity.Name, decoded.Name);
                Assert.AreEqual(entity.NetworkId, decoded.NetworkId);
                Assert.AreEqual(entity.UserId, decoded.UserId);
                Assert.AreEqual(entity.Identifier, decoded.Identifier);
            }
        }
    }
}
