
namespace Sparkle.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sparkle.Services.Authentication;
    using System;

    [TestClass]
    public class CalendarKeyTests
    {
        [TestMethod]
        public void EnsureKeyComputationValidity()
        {
            var id = new Guid("{5740F5BD-A69B-4702-B2D1-49781CB9739E}");
            var result = Keys.ComputeForCalendar(id);

            Assert.AreEqual("aHknKlrHqu2erAHgeKZIiQbb", result);
        }

        [TestMethod]
        public void EnsureRecoveryKeyComputationValidity()
        {
            var id = new Guid("{5740F5BD-A69B-4702-B2D1-49781CB9739E}");
            var date = new DateTime(2010, 10, 12, 12, 2, 5);
            var result = Keys.ComputeForAccount(id, date);

            Assert.AreEqual("BimFqdRVx8WTFixjBXtHnQzz", result);
        }
    }
}
