using System.Configuration;
using MarkdownSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarkdownSharpTests
{
    [TestClass]
    public class ConfigTest
    {
        [TestMethod]
        public void LoadFromConfiguration()
        {
            var settings = ConfigurationManager.AppSettings;
            settings.Set("Markdown.AutoHyperlink", "true");
            settings.Set("Markdown.AutoNewlines", "true");
            settings.Set("Markdown.EmptyElementSuffix", ">");
            settings.Set("Markdown.EncodeProblemUrlCharacters", "true");
            settings.Set("Markdown.LinkEmails", "false");
            settings.Set("Markdown.StrictBoldItalic", "true");
            
            var markdown = new Markdown(true);
            Assert.AreEqual(true, markdown.AutoHyperlink);
            Assert.AreEqual(true, markdown.AutoNewLines);
            Assert.AreEqual(">", markdown.EmptyElementSuffix);
            Assert.AreEqual(true, markdown.EncodeProblemUrlCharacters);
            Assert.AreEqual(false, markdown.LinkEmails);
            Assert.AreEqual(true, markdown.StrictBoldItalic);
        }

        [TestMethod]
        public void NoLoadFromConfigFile()
        {
            foreach (var markdown in new[] {new Markdown(), new Markdown(false)})
            {
                Assert.AreEqual(false, markdown.AutoHyperlink);
                Assert.AreEqual(false, markdown.AutoNewLines);
                Assert.AreEqual(" />", markdown.EmptyElementSuffix);
                Assert.AreEqual(false, markdown.EncodeProblemUrlCharacters);
                Assert.AreEqual(true, markdown.LinkEmails);
                Assert.AreEqual(false, markdown.StrictBoldItalic);
            }
        }

        [TestMethod]
        public void AutoHyperlink()
        {
            var markdown = new Markdown();  
            Assert.IsFalse(markdown.AutoHyperlink);
            Assert.AreEqual("<p>foo http://example.com bar</p>\n", markdown.Transform("foo http://example.com bar"));
            markdown.AutoHyperlink = true;
            Assert.AreEqual("<p>foo <a href=\"http://example.com\">http://example.com</a> bar</p>\n", markdown.Transform("foo http://example.com bar"));
        }

        [TestMethod]
        public void AutoNewLines()
        {
            var markdown = new Markdown();
            Assert.IsFalse(markdown.AutoNewLines);
            Assert.AreEqual("<p>Line1\nLine2</p>\n", markdown.Transform("Line1\nLine2"));
            markdown.AutoNewLines = true;
            Assert.AreEqual("<p>Line1<br />\nLine2</p>\n", markdown.Transform("Line1\nLine2"));
        }

        [TestMethod]
        public void EmptyElementSuffix()
        {
            var markdown = new Markdown();
            Assert.AreEqual(" />", markdown.EmptyElementSuffix);
            Assert.AreEqual("<hr />\n", markdown.Transform("* * *"));
            markdown.EmptyElementSuffix = ">";
            Assert.AreEqual("<hr>\n", markdown.Transform("* * *"));
        }

        [TestMethod]
        public void EncodeProblemUrlCharacters()
        {
            var markdown = new Markdown();
            Assert.IsFalse(markdown.EncodeProblemUrlCharacters);
            Assert.AreEqual("<p><a href=\"http:///&#x27;*_[]()/\">Foo</a></p>\n", markdown.Transform("[Foo](/'*_[]()/)"));
            markdown.EncodeProblemUrlCharacters = true;
            Assert.AreEqual("<p><a href=\"http:///&#x27;%2a%5f%5b%5d%28%29/\">Foo</a></p>\n", markdown.Transform("[Foo](/'*_[]()/)"));
        }

        [TestMethod, Ignore] // fix me
        public void LinkEmails()
        {
            var markdown = new Markdown();
            Assert.IsTrue(markdown.LinkEmails);
            Assert.AreEqual("<p><a href=\"&#", markdown.Transform("<aa@bb.com>").Substring(0,14));
            markdown.LinkEmails = false;
            Assert.AreEqual("<p><aa@bb.com></p>\n", markdown.Transform("<aa@bb.com>"));
        }

        [TestMethod]
        public void StrictBoldItalic()
        {
            var markdown = new Markdown();
            Assert.IsFalse(markdown.StrictBoldItalic);
            Assert.AreEqual("<p>before<strong>bold</strong>after before<em>italic</em>after</p>\n", markdown.Transform("before**bold**after before_italic_after"));
            markdown.StrictBoldItalic = true;
            Assert.AreEqual("<p>before*bold*after before_italic_after</p>\n", markdown.Transform("before*bold*after before_italic_after"));
        }
    }
}
