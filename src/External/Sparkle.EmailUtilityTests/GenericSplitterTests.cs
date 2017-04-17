
namespace Sparkle.EmailUtilityTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.IO;
    using Sparkle.EmailUtility.Splitters;
    using Sparkle.EmailUtility.Models;

    [TestClass]
    public class GenericSplitterTests
    {
        private static readonly string ProviderTests = "GenericSplitter";

        private static IDictionary<string, string> testFiles;
        public static IDictionary<string, string> TestFiles(TestContext context)
        {
            if (testFiles == null)
            {
                testFiles = new Dictionary<string, string>();
                var testPath = Path.Combine(
                    context.TestDeploymentDir,
                    ProviderTests);
                var tests = Directory.GetFiles(testPath, "*.txt");
                foreach (var file in tests)
                {
                    var sr = new StreamReader(file);
                    try
                    {
                        testFiles.Add(Path.GetFileNameWithoutExtension(file), sr.ReadToEnd().Replace("\\n", "\n"));
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        if (sr != null)
                            sr.Dispose();
                    }
                }
            }
            return testFiles;
        }

        [TestClass]
        public class IsMatchMethod
        {
            public TestContext TestContext { get; set; }

            [TestMethod]
            public void NewGenericMessage_AlwaysMatch()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlVersion"))
                    Assert.Fail();

                string input = TestFiles(TestContext)["HtmlVersion"];
                var splitter = new SplitterGeneric();

                Assert.IsTrue(splitter.IsMatch(new MessageBody(input)));
            }

        }

        [TestClass]
        public class ProcessMethod
        {
            public TestContext TestContext { get; set; }

            [TestMethod]
            public void NewGenericMessage_NoReplyMatch()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlVersion"))
                    Assert.Fail();

                var splitter = new SplitterGeneric();

                string input = TestFiles(TestContext)["HtmlVersion"];
                var result = splitter.Process(new MessageBody(input, new string[] { "name=\"Split-Here\"" }));

                Assert.AreEqual("Je suis un jolie \"message\" \u00a0\nEnvoyé depuis gmail \u00a0\nEt pourquoi pas avec une signature", result.UserMessage);
                Assert.AreEqual("--\u00a0\nTata Trololo", result.Signature);
            }

            [TestMethod]
            public void NewGenericMessage_GenericReplyProcess()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlReplyVersion"))
                    Assert.Fail();

                var splitter = new SplitterGeneric();

                string input = TestFiles(TestContext)["HtmlReplyVersion"];
                var result = splitter.Process(new MessageBody(input, new string[] { "name=\"Split-Here\"" }));

                Assert.AreEqual("Et voilà ici une petite réponse \u00a0\nVive les tests unitaires !!!", result.UserMessage);
                Assert.AreEqual("--\u00a0\nTata Trololo", result.Signature);
            }

        }

    }
}
