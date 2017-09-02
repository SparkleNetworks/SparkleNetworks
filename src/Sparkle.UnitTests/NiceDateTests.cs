using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sparkle.Common;
using Sparkle.Common.Resources;
using System.Globalization;

namespace Sparkle.UnitTests
{
    [TestClass]
    public class NiceDateTests
    {

        /// <summary>
        /// 10 Décembre 2011 10:10:10
        /// </summary>
        DateTime now = new DateTime(2011, 12, 10, 10, 10, 10);

        CultureInfo mainCulture = new CultureInfo("fr-fr");

        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public void YearsAgo()
        {
            string value = MainStrings.NiceDateTime_YearsAgo_;
            Assert.AreNotEqual(
                string.Format(value, 0D),
                new DateTime(2011, 1, 1, 10, 10, 10).ToNiceDelay(now));
            Assert.AreEqual(
                MainStrings.NiceDateTime_PreviousYear,
                now.AddYears(-1).ToNiceDelay(now));
            Assert.AreEqual(
                string.Format(value, 2),
                now.AddYears(-2).ToNiceDelay(now));
            Assert.AreEqual(
                string.Format(value, 5),
                now.AddYears(-5).ToNiceDelay(now));
            Assert.AreEqual(
                string.Format(value, 55),
                now.AddYears(-55).ToNiceDelay(now));
        }

        [TestMethod]
        public void YearsLater()
        {
            string value = MainStrings.NiceDateTime_InYears_;
            Assert.AreNotEqual(
                string.Format(value, 0D),
                new DateTime(2011, 12, 30, 10, 10, 10).ToNiceDelay(now));
            Assert.AreEqual(
                MainStrings.NiceDateTime_NextYear,
                now.AddYears(1).ToNiceDelay(now));
            Assert.AreEqual(
                string.Format(value, 2),
                now.AddYears(2).ToNiceDelay(now));
            Assert.AreEqual(
                string.Format(value, 5),
                now.AddYears(5).ToNiceDelay(now));
            Assert.AreEqual(
                string.Format(value, 55),
                now.AddYears(55).ToNiceDelay(now));
        }

        [TestMethod]
        public void Months()
        {
            DateTime now = new DateTime(2011, 10, 10, 10, 10, 10);
            string value = MainStrings.NiceDateTime_InMonths_;

            Assert.AreNotEqual(
                string.Format(value, 0D),
                new DateTime(2011, 12, 30, 10, 10, 10).ToNiceDelay(now));

            // Nov
            Assert.AreEqual(
                MainStrings.NiceDateTime_NextMonth,
                now.AddMonths(1).ToNiceDelay(now));

            // Dec
            Assert.AreEqual(
                string.Format(value, 2),
                now.AddMonths(2).ToNiceDelay(now));

            // Jan 2012
            Assert.AreEqual(
                string.Format(value, 3),
                now.AddMonths(3).ToNiceDelay(now));

            value = MainStrings.NiceDateTime_MonthsAgo_;

            // Oct
            Assert.AreEqual(
                MainStrings.NiceDateTime_PreviousMonth,
                now.AddMonths(-1).ToNiceDelay(now));

            // Aug
            Assert.AreEqual(
                string.Format(value, 2),
                now.AddMonths(-2).ToNiceDelay(now));

            // Jul
            Assert.AreEqual(
                string.Format(value, 3),
                now.AddMonths(-3).ToNiceDelay(now));

            // Jun
            Assert.AreEqual(
                string.Format(value, 4),
                now.AddMonths(-4).ToNiceDelay(now));

            // May
            Assert.AreEqual(
                string.Format(value, 5),
                now.AddMonths(-5).ToNiceDelay(now));

            // Apr
            Assert.AreEqual(
                string.Format(value, 6),
                now.AddMonths(-6).ToNiceDelay(now));

            // Mar
            Assert.AreEqual(
                string.Format(value, 7),
                now.AddMonths(-7).ToNiceDelay(now));

            // Feb
            Assert.AreEqual(
                string.Format(value, 8),
                now.AddMonths(-8).ToNiceDelay(now));

            // Jan
            Assert.AreEqual(
                string.Format(value, 9),
                now.AddMonths(-9).ToNiceDelay(now));

            // Dec 2010
            Assert.AreEqual(
                string.Format(value, 10),
                now.AddMonths(-10).ToNiceDelay(now));
        }

        [TestMethod]
        public void Days()
        {
            
        }

        [TestMethod]
        public void JustNow()
        {
            for (double i = -2.9D; i < 3D; i += .2D)
            {
                Assert.AreEqual(MainStrings.NiceDateTime_JustNow, now.AddMinutes(i).ToNiceDelay(now));
            }
        }
    }
}
