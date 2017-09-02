
namespace Sparkle.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sparkle.Common.DataAnnotations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [TestClass]
    public class UrlAttributeTests
    {
        [TestMethod]
        public void BasicUrlValidates()
        {
            string input = "http://dudoij.com/";
            var attr = new UrlAttribute();

            var result = attr.IsValid(input);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void BasicUrlWithUppercaseValidates()
        {
            string input = "http://dudoij.com/About";
            var attr = new UrlAttribute();

            var result = attr.IsValid(input);
            Assert.IsTrue(result);
        }
    }
}
