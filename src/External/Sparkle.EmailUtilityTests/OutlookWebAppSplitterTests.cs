using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using Sparkle.EmailUtility.Splitters;
using Sparkle.EmailUtility.Models;
using Sparkle.EmailUtility;

namespace Sparkle.EmailUtilityTests
{
    [TestClass]
    public class OutlookWebAppSplitterTests
    {
        private static readonly string ProviderTests = "OutlookWebAppSplitter";

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
            public void NewOutlookWebAppMessage_NoReplyMatch()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlVersion"))
                    Assert.Fail();

                string input = TestFiles(TestContext)["HtmlVersion"];
                var splitter = new SplitterOutlookWebApp();

                Assert.IsFalse(splitter.IsMatch(new MessageBody(input)));
            }

            [TestMethod]
            public void NewOutlookWebAppMessage_ReplyMatch()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlReplyVersion"))
                    Assert.Fail();

                string input = TestFiles(TestContext)["HtmlReplyVersion"];
                var splitter = new SplitterOutlookWebApp();

                Assert.IsTrue(splitter.IsMatch(new MessageBody(input)));
            }
        }

        [TestClass]
        public class ProcessMethod
        {
            public TestContext TestContext { get; set; }

            [TestMethod]
            public void NewOutlookWebAppMessage_NoMatchProcess()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlVersion"))
                    Assert.Fail();

                var splitter = new SplitterOutlookWebApp();

                string input = TestFiles(TestContext)["HtmlVersion"];
                var result = splitter.Process(new MessageBody(input));

                Assert.IsNull(result.UserMessage);
                Assert.IsNull(result.Signature);
                Assert.IsNull(result.ReplyQuote);
            }

            [TestMethod]
            public void NewOutlookWebAppMessage_GenericMatchProcess()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlVersion"))
                    Assert.Fail();

                var splitter = new SplitterGeneric();

                string input = TestFiles(TestContext)["HtmlVersion"];
                var result = splitter.Process(new MessageBody(input));

                Assert.AreEqual("Voilà un petit message classique  \n\nTout ce qu'il y a de __plus normal__  \n\nAvec un peu de html youpiii​ !!", result.UserMessage);
                Assert.AreEqual("-- \n\nTata Trololo", result.Signature);
            }

            [TestMethod]
            public void NewOutlookWebAppMessage_OutlookReplyProcess()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlReplyVersion"))
                    Assert.Fail();

                var splitter = new SplitterOutlookWebApp();

                string input = TestFiles(TestContext)["HtmlReplyVersion"];
                var result = splitter.Process(new MessageBody(input));

                Assert.AreEqual("Et une petite réponse ici  \n\nLes regex c'est *la *__vie__ eh oui !​  \n\nTout ça depuis outlook web app", result.UserMessage);
                Assert.AreEqual("-- \n\nTata Trololo", result.Signature);
            }

            [TestMethod]
            public void NewOutlookWebAppMessage_OutlookMarkdownAndCodeProcess()
            {
                if (!TestFiles(TestContext).ContainsKey("HtmlVersionWithMarkdownAndCode"))
                    Assert.Fail();

                var splitter = new SplitterOutlookWebApp();

                string input = TestFiles(TestContext)["HtmlVersionWithMarkdownAndCode"];
                var result = SplitterCommon.ScrubHtml(input);

                Assert.AreEqual(result, "Et voilà un autre message en provenance de outlook web app &nbsp;\n\nToujours avec du markdown car on aime ça\n\n\n\nEt aussi du code car c'est cool\n\n\n\n    &nbsp;&nbsp;&nbsp;&nbsp;public&nbsp;class&nbsp;HtmlAgilityNode\n    &nbsp;&nbsp;&nbsp;&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public&nbsp;string&nbsp;Text&nbsp;{&nbsp;get;&nbsp;set;&nbsp;}\n     \n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public&nbsp;HtmlAgilityBlockType&nbsp;BlockType&nbsp;{&nbsp;get;&nbsp;set;&nbsp;}\n     \n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public&nbsp;HtmlAgilityNodeType&nbsp;NodeType&nbsp;{&nbsp;get;&nbsp;set;&nbsp;}\n     \n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public&nbsp;IList&lt;HtmlAgilityNode&gt;&nbsp;Content&nbsp;{&nbsp;get;&nbsp;set;&nbsp;}\n     \n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public&nbsp;HtmlAgilityNode()\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Content&nbsp;=&nbsp;new&nbsp;List&lt;HtmlAgilityNode&gt;();\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}\n     \n    &nbsp;&nbsp;&nbsp;&nbsp;}\n    &#8203;\n    \n\n\n");
            }

            [TestMethod]
            public void NewOutlookWebAppMessage_OutlookAlteredMatchString()
            {
                // Email came from Microsoft Word 14 ...

                if (!TestFiles(TestContext).ContainsKey("HtmlReplyVersionMatchStringStrippedOfQuotes"))
                    Assert.Fail();

                var splitter = new SplitterOutlookWebApp();

                string input = TestFiles(TestContext)["HtmlReplyVersionMatchStringStrippedOfQuotes"];
                var result = splitter.Process(new MessageBody(input));

                Assert.IsNull(result.UserMessage);
            }
        }

    }
}
