
namespace Sparkle.EmailUtilityTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sparkle.EmailUtility.Models;
    using Sparkle.EmailUtility.Splitters;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    [TestClass]
    public class WindowsMailSplitterTests
    {
        private static readonly string ProviderTests = "WindowsMailSplitter";

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
            public void NewWindowsMailMessage_NoReplyMatch()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlVersion"))
                    Assert.Fail();

                string input = TestFiles(TestContext)["HtmlVersion"];
                var splitter = new SplitterWindowsMail();

                // There we do Assert.IsTrue because Windows Mail message always contains "<div data-signatureblock"
                Assert.IsTrue(splitter.IsMatch(new MessageBody(input)));
            }

            [TestMethod]
            public void NewWindowsMailMessage_ReplyMatch()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlReplyVersion"))
                    Assert.Fail();

                string input = TestFiles(TestContext)["HtmlReplyVersion"];
                var splitter = new SplitterWindowsMail();

                Assert.IsTrue(splitter.IsMatch(new MessageBody(input)));
            }

        }

        [TestClass]
        public class ProcessMethod
        {
            public TestContext TestContext { get; set; }

            [TestMethod]
            public void NewWindowsMailMessage_NoReplyProcess()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlVersion"))
                    Assert.Fail();

                var splitter = new SplitterWindowsMail();

                string input = TestFiles(TestContext)["HtmlVersion"];
                var result = splitter.Process(new MessageBody(input));

                Assert.AreEqual("Voilà un jolie message provenant de windows Mail  \nAvec toujours un __peu de html__ !!", result.UserMessage);
                Assert.AreEqual("-- \nTata Trololo", result.Signature);
            }

            [TestMethod]
            public void NewWindowsMailMessage_GenericMatchProcess()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlVersion"))
                    Assert.Fail();

                var splitter = new SplitterGeneric();

                string input = TestFiles(TestContext)["HtmlVersion"];
                var result = splitter.Process(new MessageBody(input));

                Assert.AreEqual("Voilà un jolie message provenant de windows Mail  \nAvec toujours un __peu de html__ !!", result.UserMessage);
                Assert.AreEqual("-- \nTata Trololo\n\n\n\nSent from Windows Mail", result.Signature);
            }

            [TestMethod]
            public void NewWindowsMailMessage_WindowsMailReplyProcess()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlReplyVersion"))
                    Assert.Fail();

                var splitter = new SplitterWindowsMail();

                string input = TestFiles(TestContext)["HtmlReplyVersion"];
                var result = splitter.Process(new MessageBody(input));

                Assert.AreEqual("Et une __tout aussi__ *jolie réponse*  \nAvec du __html comme__ d'hab", result.UserMessage);
                Assert.AreEqual("-- \nTata Trololo", result.Signature);
            }

        }

    }
}
