
namespace Sparkle.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sparkle.EmailTemplates;
    using Sparkle.UI;
    using Sparkle.UnitTests.Mocks;
    using System;
    using System.Collections.Generic;

    [TestClass]
    public class DefaultEmailTemplateProviderTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Lang.Source = Strings.Load(Environment.CurrentDirectory, "App1", "en-us");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Lang.Source = null;
        }

        [TestMethod]
        public void Works1()
        {
            var provider = new DefaultEmailTemplateProvider();
            string key = "Test";
            var model = new SimpleEmailModel
            {
                Name = "Test name",
            };

            var result = provider.Process(key, null, null, null, model);
            Assert.IsTrue(result.Contains("TESTTEST"));
        }

        [TestMethod]
        public void WorksWithTimezone()
        {
            var provider = new DefaultEmailTemplateProvider();
            string key = "Test";
            var model = new SimpleEmailModel
            {
                Name = "Test name",
            };
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");

            var result = provider.Process(key, null, null, tz, model);
            Assert.IsTrue(result.Contains("TESTTEST"));
        }

        [TestClass]
        public class FindIncludesMethod
        {
            [TestMethod]
            public void NoIncludeReturnsEmptyList()
            {
                string template = string.Empty;
                var target = new TestDefaultEmailTemplateProvider();
                IList<string> result = target.FindIncludes_Public(template);
                Assert.IsNotNull(result);
                Assert.AreEqual(0, result.Count);
            }

            [TestMethod]
            public void OneLineOneIncludeReturnsOneEntry()
            {
                string template = "@model SomeModel\r\n" +
                    "@* Razor Include: _Timeline.cshtml *@\r\n" +
                    "<html> here</html>\r\n";
                var target = new TestDefaultEmailTemplateProvider();
                IList<string> result = target.FindIncludes_Public(template);
                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("_Timeline.cshtml", result[0]);
            }

            [TestMethod]
            public void OneLineTwoIncludeReturnsOneEntry()
            {
                string template = "@model SomeModel\r\n" +
                    "@* Razor Include: _Timeline.cshtml, Hello.cshtml *@\r\n" +
                    "<html> here</html>\r\n";
                var target = new TestDefaultEmailTemplateProvider();
                IList<string> result = target.FindIncludes_Public(template);
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual("_Timeline.cshtml", result[0]);
                Assert.AreEqual("Hello.cshtml", result[1]);
            }

            [TestMethod]
            public void OneLineThreeIncludeReturnsOneEntry()
            {
                string template = "@model SomeModel\r\n" +
                    "@* Razor Include: _Timeline.cshtml, Hello.cshtml , Yep-Nop.cshtml *@\r\n" +
                    "<html> here</html>\r\n";
                var target = new TestDefaultEmailTemplateProvider();
                IList<string> result = target.FindIncludes_Public(template);
                Assert.IsNotNull(result);
                Assert.AreEqual(3, result.Count);
                Assert.AreEqual("_Timeline.cshtml", result[0]);
                Assert.AreEqual("Hello.cshtml", result[1]);
                Assert.AreEqual("Yep-Nop.cshtml", result[2]);
            }

            [TestMethod]
            public void TwoLineOneIncludeReturnsOneEntry()
            {
                string template = "@model SomeModel\r\n" +
                    "@* Razor Include: _Timeline.cshtml *@\r\n" +
                    "@* Razor Include: Hello.cshtml *@\r\n" +
                    "<html> here</html>\r\n";
                var target = new TestDefaultEmailTemplateProvider();
                IList<string> result = target.FindIncludes_Public(template);
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual("_Timeline.cshtml", result[0]);
                Assert.AreEqual("Hello.cshtml", result[1]);
            }
        }

        public class TestDefaultEmailTemplateProvider : DefaultEmailTemplateProvider
        {
            public IList<string> FindIncludes_Public(string template)
            {
                return this.FindIncludes(template);
            }
        }
    }
}
