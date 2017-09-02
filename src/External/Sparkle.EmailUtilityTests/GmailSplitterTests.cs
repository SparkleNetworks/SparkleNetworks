
namespace Sparkle.EmailUtilityTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using Sparkle.EmailUtility;
    using Sparkle.EmailUtility.Splitters;
    using Sparkle.EmailUtility.Models;
    using System.Web;

    [TestClass]
    public class GmailSplitterTests
    {
        private static readonly string ProviderTests = "GmailSplitter";

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
            public void NewGmailMessage_NoReplyMatch()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlVersion"))
                    Assert.Fail();

                string input = TestFiles(TestContext)["HtmlVersion"];
                var splitter = new SplitterGmail();

                Assert.IsFalse(splitter.IsMatch(new MessageBody(input)));
            }

            [TestMethod]
            public void NewGmailMessage_ReplyMatch()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlReplyVersion"))
                    Assert.Fail();

                string input = TestFiles(TestContext)["HtmlReplyVersion"];
                var splitter = new SplitterGmail();

                Assert.IsTrue(splitter.IsMatch(new MessageBody(input)));
            }

            [TestMethod]
            public void NewGmailMessage_ReplyMatchAutoLinkedSign()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlReplyVersionAutoLinkedSignature"))
                    Assert.Fail();

                string input = TestFiles(TestContext)["HtmlReplyVersionAutoLinkedSignature"];
                var splitter = new SplitterGmail();

                Assert.IsTrue(splitter.IsMatch(new MessageBody(input)));
            }
        }

        [TestClass]
        public class ProcessMethod
        {
            public TestContext TestContext { get; set; }

            [TestMethod]
            public void NewGmailMessage_NoMatchProcess()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlVersion"))
                    Assert.Fail();

                var splitter = new SplitterGmail();

                string input = TestFiles(TestContext)["HtmlVersion"];
                var result = splitter.Process(new MessageBody(input));

                Assert.IsNull(result.UserMessage);
                Assert.IsNull(result.Signature);
            }

            [TestMethod]
            public void NewGmailMessage_GenericMatchProcess()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlVersion"))
                    Assert.Fail();

                var splitter = new SplitterGeneric();

                string input = TestFiles(TestContext)["HtmlVersion"];
                var result = splitter.Process(new MessageBody(input));

                Assert.AreEqual("Je suis un jolie \"message\" \u00a0\nEnvoyé depuis gmail \u00a0\nEt pourquoi pas avec une signature", result.UserMessage);
                Assert.AreEqual("--\u00a0\nTata Trololo", result.Signature);
            }

            [TestMethod]
            public void NewGmailMessage_GmailReplyProcess()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlReplyVersion"))
                    Assert.Fail();

                var splitter = new SplitterGmail();

                string input = TestFiles(TestContext)["HtmlReplyVersion"];
                var result = splitter.Process(new MessageBody(input));

                Assert.AreEqual("Et voilà ici une petite réponse \u00a0\nVive les tests unitaires !!!", result.UserMessage);
                Assert.AreEqual("--\u00a0\nTata Trololo", result.Signature);
            }

            [TestMethod]
            public void NewGmailMessage_GmailReplyAutoLinkedSignProcess()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlReplyVersionAutoLinkedSignature"))
                    Assert.Fail();

                var splitter = new SplitterGmail();

                string input = TestFiles(TestContext)["HtmlReplyVersionAutoLinkedSignature"];
                var result = splitter.Process(new MessageBody(input));

                Assert.AreEqual("Et voilà ici une petite réponse \u00a0\nVive les tests unitaires !!!", result.UserMessage);
                Assert.IsNull(result.Signature);
            }

            [TestMethod]
            public void NewGmailMessage_GmailMarkdownAndCodeProcess()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlVersionWithMarkdownAndCode"))
                    Assert.Fail();

                var splitter = new SplitterGmail();

                string input = TestFiles(TestContext)["HtmlVersionWithMarkdownAndCode"];
                var result = SplitterCommon.ScrubHtml(input);

                Assert.AreEqual(result, "Ceci est un nouveau message et c'est trop cool &nbsp;\nOn peut utiliser un peu markdown et tout ce construit bien\n\n\nEt même mettre un peu de code :\n\n    &nbsp;&nbsp;&nbsp;&nbsp;public&nbsp;class&nbsp;HtmlAgilityNode\n    &nbsp;&nbsp;&nbsp;&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public&nbsp;string&nbsp;Text&nbsp;{&nbsp;get;&nbsp;set;&nbsp;}\n     \n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public&nbsp;HtmlAgilityBlockType&nbsp;BlockType&nbsp;{&nbsp;get;&nbsp;set;&nbsp;}\n     \n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public&nbsp;HtmlAgilityNodeType&nbsp;NodeType&nbsp;{&nbsp;get;&nbsp;set;&nbsp;}\n     \n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public&nbsp;IList&lt;HtmlAgilityNode&gt;&nbsp;Content&nbsp;{&nbsp;get;&nbsp;set;&nbsp;}\n     \n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public&nbsp;HtmlAgilityNode()\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Content&nbsp;=&nbsp;new&nbsp;List&lt;HtmlAgilityNode&gt;();\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}\n     \n    &nbsp;&nbsp;&nbsp;&nbsp;}\n    \nOn va voir comment à rend trop bien maintenant &nbsp;\n\n\n");
            }
        }

    }
}
