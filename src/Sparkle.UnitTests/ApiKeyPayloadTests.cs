
namespace Sparkle.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sparkle.Services.StatusApi;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    [TestClass]
    public class ApiKeyPayloadTests
    {
        [TestMethod]
        public void EmptyKey()
        {
            var nowString = "2016-02-23T22:06:44.3028855Z";
            var now = DateTime.ParseExact("2016-02-23T22:06:44.9243330Z", "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            string key = "aaaa", secret = "azpoeiazpoeiazpoeipoazei";
            string clientHash = "b35ff1963e80f0e76acbc957aa154564199b628cfcbedfd0c690b6b6329bc80D";
            var payload = new NetworkStatusApiKeyPayload("", nowString, clientHash, "POST", "/HelloWorld?x=y", "{json:\"value\"}");
            var result = payload.Verify(key, secret, now);
            Assert.IsFalse(result);
            Assert.AreEqual(1, payload.VerificationErrors.Count);
            Assert.AreEqual("MissingKey", payload.VerificationErrors[0].Key);
        }

        [TestMethod]
        public void EmptyHash()
        {
            var nowString = "2016-02-23T22:06:44.3028855Z";
            var now = DateTime.ParseExact("2016-02-23T22:06:44.9243330Z", "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            string key = "aaaa", secret = "azpoeiazpoeiazpoeipoazei";
            string clientHash = "";
            var payload = new NetworkStatusApiKeyPayload(key, nowString, clientHash, "POST", "/HelloWorld?x=y", "{json:\"value\"}");
            var result = payload.Verify(key, secret, now);
            Assert.IsFalse(result);
            Assert.AreEqual(1, payload.VerificationErrors.Count);
            Assert.AreEqual("MissingHash", payload.VerificationErrors[0].Key);
        }

        [TestMethod]
        public void VerifyValidHash()
        {
            var nowString = "2016-02-23T22:06:44.3028855Z";
            var now = DateTime.ParseExact("2016-02-23T22:06:44.9243330Z", "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            string key = "aaaa", secret = "azpoeiazpoeiazpoeipoazei";
            string clientHash = "b35ff1963e80f0e76acbc957aa154564199b628cfcbedfd0c690b6b6329bc80D";
            var payload = new NetworkStatusApiKeyPayload(key, nowString, clientHash, "POST", "/HelloWorld?x=y", "{json:\"value\"}");
            var result = payload.Verify(key, secret, now);
            Assert.IsTrue(result);
            Assert.AreEqual(0, payload.VerificationErrors.Count);
        }

        [TestMethod]
        public void ComputeHashValidHash()
        {
            var nowString = "2016-02-23T22:06:44.3028855Z";
            var now = DateTime.ParseExact("2016-02-23T22:06:44.9243330Z", "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            string key = "aaaa", secret = "azpoeiazpoeiazpoeipoazei";
            string clientHash = "b35ff1963e80f0e76acbc957aa154564199b628cfcbedfd0c690b6b6329bc80D";
            var payload = new NetworkStatusApiKeyPayload(key, nowString, clientHash, "POST", "/HelloWorld?x=y", "{json:\"value\"}");
            var result = payload.ComputeHash(secret);
            Assert.IsTrue(clientHash.Equals(result, StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void VerifyValidHash_NoContent()
        {
            var nowString = "2016-02-23T22:06:44.3028855Z";
            var now = DateTime.ParseExact("2016-02-23T22:06:44.9243330Z", "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            string key = "aaaa", secret = "azpoeiazpoeiazpoeipoazei";
            string clientHash = "3a0f8ebcc3b79c8e223e953be730a30f65554a5bff4ec43db9ea63fa7cd5881e";
            var payload = new NetworkStatusApiKeyPayload(key, nowString, clientHash, "GET", "/HelloWorld?x=y", null);
            var result = payload.Verify(key, secret, now);
            Assert.IsTrue(result);
            Assert.AreEqual(0, payload.VerificationErrors.Count);
        }

        [TestMethod]
        public void VerifyInvalidHash()
        {
            var nowString = "2016-02-23T22:06:44.3028855Z";
            var now = DateTime.ParseExact("2016-02-23T22:06:44.9243330Z", "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            string key = "aaaa", secret = "azpoeiazpoeiazpoeipoazei";
            string clientHash = "b35ff1963e80f0e76acbc";
            var payload = new NetworkStatusApiKeyPayload(key, nowString, clientHash, "POST", "/HelloWorld?x=y", "{json:\"value\"}");
            var result = payload.Verify(key, secret, now);
            Assert.IsFalse(result);
            Assert.AreEqual(1, payload.VerificationErrors.Count);
            Assert.AreEqual("InvalidHash", payload.VerificationErrors[0].Key);
        }

        [TestMethod]
        public void InvalidTimeFormat()
        {
            var nowString = "23/02/2016 22:06:44";
            var now = DateTime.ParseExact("2016-02-23T22:06:44.9243330Z", "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            string key = "aaaa", secret = "azpoeiazpoeiazpoeipoazei";
            string clientHash = "b35ff1963e80f0e76acbc";
            var payload = new NetworkStatusApiKeyPayload(key, nowString, clientHash, "POST", "/HelloWorld?x=y", "{json:\"value\"}");
            var result = payload.Verify(key, secret, now);
            Assert.IsFalse(result);
            Assert.AreEqual(1, payload.VerificationErrors.Count);
            Assert.AreEqual("InvalidTime", payload.VerificationErrors[0].Key);
        }

        [TestMethod]
        public void TimeIsTooOld()
        {
            var nowString = "2016-02-23T21:06:44.3028855Z";
            var now = DateTime.ParseExact("2016-02-23T22:06:44.9243330Z", "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            string key = "aaaa", secret = "azpoeiazpoeiazpoeipoazei";
            string clientHash = "b35ff1963e80f0e76acbc";
            var payload = new NetworkStatusApiKeyPayload(key, nowString, clientHash, "POST", "/HelloWorld?x=y", "{json:\"value\"}");
            var result = payload.Verify(key, secret, now);
            Assert.IsFalse(result);
            Assert.AreEqual(1, payload.VerificationErrors.Count);
            Assert.AreEqual("InvalidTime", payload.VerificationErrors[0].Key);
        }

        [TestMethod]
        public void TimeIsTooFuture()
        {
            var nowString = "2016-02-23T23:06:44.3028855Z";
            var now = DateTime.ParseExact("2016-02-23T22:06:44.9243330Z", "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            string key = "aaaa", secret = "azpoeiazpoeiazpoeipoazei";
            string clientHash = "b35ff1963e80f0e76acbc";
            var payload = new NetworkStatusApiKeyPayload(key, nowString, clientHash, "POST", "/HelloWorld?x=y", "{json:\"value\"}");
            var result = payload.Verify(key, secret, now);
            Assert.IsFalse(result);
            Assert.AreEqual(1, payload.VerificationErrors.Count);
            Assert.AreEqual("InvalidTime", payload.VerificationErrors[0].Key);
        }
    }
}
