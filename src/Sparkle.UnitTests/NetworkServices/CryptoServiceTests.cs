
namespace Sparkle.UnitTests.NetworkServices
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Sparkle.Data.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Infrastructure.Data;
    using Sparkle.Services.Main.Networks;
    using Sparkle.Services.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;

    [TestClass]
    public class CryptoServiceTests
    {
        public TestContext TestContext { get; set; }

        public LocalContext Context { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            this.Context = new LocalContext();
            this.Context.MockFactory = new MockRepository(MockBehavior.Strict);
            this.Context.Services = this.Context.MockFactory.Create<IServiceFactory>();
            this.Context.RepositoryFactory = this.Context.MockFactory.Create<IRepositoryFactory>();
            this.Context.Target = new CryptoService(this.Context.RepositoryFactory.Object, this.Context.Services.Object);

            this.Context.Configuration = new Dictionary<string, AppConfigurationEntry>();
            var appConfiguration = new AppConfiguration(new Application(), this.Context.Configuration);
            this.Context.Services.SetupGet(x => x.AppConfiguration).Returns(appConfiguration);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetHashGetHashAlgorithmByPurpose_Arg0IsNull()
        {
            this.Context.Target.GetHashAlgorithmByPurpose(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void GetHashGetHashAlgorithmByPurpose_InvalidPurpose()
        {
            string purpose = "Invalid purpose key";
            this.Context.Target.GetHashAlgorithmByPurpose(purpose);
        }

        [TestMethod]
        public void GetHashGetHashAlgorithmByPurpose_OK()
        {
            string purpose = "Purpose1";
            this.Context.Configuration.Add("Security.Purposes." + purpose + ".HashAlgorithmTypeName", new AppConfigurationEntry { RawValue = "System.Security.Cryptography.SHA256Managed", });
            var result = this.Context.Target.GetHashAlgorithmByPurpose(purpose);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(SHA256Managed));
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void GetHashGetHashAlgorithmByPurpose_BadType()
        {
            string purpose = "Purpose1";
            this.Context.Configuration.Add("Security.Purposes." + purpose + ".HashAlgorithmTypeName", new AppConfigurationEntry { RawValue = "lolz", });
            var result = this.Context.Target.GetHashAlgorithmByPurpose(purpose);
        }

        [TestMethod, ExpectedException(typeof(InvalidCastException))]
        public void GetHashGetHashAlgorithmByPurpose_BadType2()
        {
            string purpose = "Purpose1";
            this.Context.Configuration.Add("Security.Purposes." + purpose + ".HashAlgorithmTypeName", new AppConfigurationEntry { RawValue = "System.Object", });
            var result = this.Context.Target.GetHashAlgorithmByPurpose(purpose);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetStaticNoiseByPurpose_Arg0IsNull()
        {
            int size = 128;
            this.Context.Target.GetStaticNoiseByPurpose(null, size);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void GetStaticNoiseByPurpose_InvalidPurpose()
        {
            string purpose = "Invalid purpose key";
            int size = 128;
            this.Context.Target.GetStaticNoiseByPurpose(purpose, size);
        }

        [TestMethod]
        public void GetStaticNoiseByPurpose_SameLength_OK()
        {
            string purpose = "Purpose1";
            int size = 128;
            var data = new byte[size];
            data[0] = 1;
            data[5] = 128;
            var base64 = Convert.ToBase64String(data);
            this.Context.Configuration.Add("Security.Purposes." + purpose + ".StaticNoise", new AppConfigurationEntry { RawValue = base64, });
            byte[] result = this.Context.Target.GetStaticNoiseByPurpose(purpose, size);
            Assert.IsNotNull(result);
            Assert.AreEqual(size, result.Length);
            Assert.IsFalse(result.All(x => x == 0));
            Assert.IsTrue(data.ArraySequenceEquals(result));
        }

        [TestMethod]
        public void GetStaticNoiseByPurpose_Smaller_OK()
        {
            string purpose = "Purpose1";
            int size = 64;
            var data = new byte[size];
            data[0] = 1;
            data[5] = 128;
            var base64 = Convert.ToBase64String(data);
            this.Context.Configuration.Add("Security.Purposes." + purpose + ".StaticNoise", new AppConfigurationEntry { RawValue = base64, });
            byte[] result = this.Context.Target.GetStaticNoiseByPurpose(purpose, size);
            Assert.IsNotNull(result);
            Assert.AreEqual(size, result.Length);
            Assert.IsFalse(result.All(x => x == 0));
            Assert.IsTrue(data.ArraySequenceEquals(result));
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void GetStaticNoiseByPurpose_NoiseTooSmall()
        {
            string purpose = "Purpose1";
            int size = 128;
            var data = new byte[size];
            data[0] = 1;
            data[5] = 128;
            var base64 = Convert.ToBase64String(data);
            this.Context.Configuration.Add("Security.Purposes." + purpose + ".StaticNoise", new AppConfigurationEntry { RawValue = base64, });
            byte[] result = this.Context.Target.GetStaticNoiseByPurpose(purpose, 1024);
            Assert.IsNotNull(result);
            Assert.AreEqual(size, result.Length);
            Assert.IsFalse(result.All(x => x == 0));
            Assert.IsTrue(data.ArraySequenceEquals(result));
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ComputeStaticUserHash_Arg0IsNull()
        {
            int userId = 1234;
            DateTime dateUserCreatedUtc = new DateTime(2012, 1, 15, 16, 45, 29, DateTimeKind.Utc);
            this.Context.Target.ComputeStaticUserHash(null, userId, dateUserCreatedUtc);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void ComputeStaticUserHash_InvalidPurpose()
        {
            int userId = 1234;
            DateTime dateUserCreatedUtc = new DateTime(2012, 1, 15, 16, 45, 29, DateTimeKind.Utc);
            string purpose = "Invalid purpose key";
            this.Context.Target.ComputeStaticUserHash(purpose, userId, dateUserCreatedUtc);
        }

        [TestMethod]
        public void ComputeStaticUserHash_OK1()
        {
            int userId = 1234;
            DateTime dateUserCreatedUtc = new DateTime(2012, 1, 15, 16, 45, 29, DateTimeKind.Utc);
            string purpose = "Purpose1";
            this.Context.Configuration.Add("Security.Purposes." + purpose + ".HashAlgorithmTypeName", new AppConfigurationEntry { RawValue = "System.Security.Cryptography.SHA256Managed", });
            var staticNoise64 = Convert.ToBase64String(new byte[256]);
            this.Context.Configuration.Add("Security.Purposes." + purpose + ".StaticNoise", new AppConfigurationEntry { RawValue = staticNoise64, });
            var result = this.Context.Target.ComputeStaticUserHash(purpose, userId, dateUserCreatedUtc);
            Assert.IsNotNull(result);
            var result64 = Convert.ToBase64String(result);
            Assert.AreEqual("qRdfpxLTI6KlAjW3t2LWovVZPg4FFs9Hnv3jwn4X5og=", result64);
        }

        [TestMethod]
        public void ComputeStaticUserHash_OK2()
        {
            int userId = 1234;
            DateTime dateUserCreatedUtc = new DateTime(2012, 1, 15, 16, 45, 29, DateTimeKind.Utc);
            string purpose = "Purpose1";
            this.Context.Configuration.Add("Security.Purposes." + purpose + ".HashAlgorithmTypeName", new AppConfigurationEntry { RawValue = "System.Security.Cryptography.SHA256Managed", });
            var staticNoise = new byte[256];
            for (int i = 0; i < staticNoise.Length; i++)
            {
                staticNoise[i] = checked((byte)(i % 256));
            }
            var staticNoise64 = Convert.ToBase64String(staticNoise);
            this.Context.Configuration.Add("Security.Purposes." + purpose + ".StaticNoise", new AppConfigurationEntry { RawValue = staticNoise64, });
            var result = this.Context.Target.ComputeStaticUserHash(purpose, userId, dateUserCreatedUtc);
            Assert.IsNotNull(result);
            var result64 = Convert.ToBase64String(result);
            Assert.AreEqual("19FnlWJQgs+w33pMNT1hEIRxVpPOtFdUHaKSca1IgkM=", result64);
        }

        public class LocalContext
        {
            public MockRepository MockFactory { get; set; }

            public CryptoService Target { get; set; }

            public Mock<IServiceFactory> Services { get; set; }

            public Mock<IRepositoryFactory> RepositoryFactory { get; set; }

            public Dictionary<string, AppConfigurationEntry> Configuration { get; set; }
        }
    }
}
