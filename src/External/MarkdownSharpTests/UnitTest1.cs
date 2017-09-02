using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarkdownSharp;

namespace UnitTestProject2
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAutoHyperlink()
        {
            var optiosn = new MarkdownOptions
            {
                AutoHyperlink = true,
            };
            var markdown = new Markdown(optiosn);
            string input = "http://example.com";
            string expected = "<p><a href=\"http://example.com\">http://example.com</a></p>\n";

            var result = markdown.Transform(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestFreeChevron()
        {
            var markdown = new Markdown();
            string input = "Quelques chevrons libres < > < / >";
            string expected = "<p>Quelques chevrons libres &lt; &gt; &lt; / &gt;</p>\n";

            var result = markdown.Transform(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestHtmlAccent()
        {
            var markdown = new Markdown();
            string input = "Ceci devrait etre un accent aigu : &eacute; é";
            string expected = "<p>Ceci devrait etre un accent aigu : &eacute; é</p>\n";

            var result = markdown.Transform(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDoubleSpace()
        {
            var markdown = new Markdown();
            string input = "Coucou double space  ";
            string expected = "<p>Coucou double space  </p>\n";

            var result = markdown.Transform(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDoubleSpace_InParagraph()
        {
            var markdown = new Markdown();
            string input = "Coucou double space  \nBonjour je suis une deuxieme phrase";
            string expected = "<p>Coucou double space<br />\nBonjour je suis une deuxieme phrase</p>\n";

            var result = markdown.Transform(input);

            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [TestMethod, Ignore] // fix me
        public void TestTextQuotes()
        {
            var markdown = new Markdown();
            string input = "> Ceci est une quote  \n> Et voila la suite\n>\n> Et un saut de ligne";
            string expected = "<blockquote>\n  <p>Ceci est une quote<br />\n Et voila la suite</p>\n\n<p>Et un saut de ligne</p>\n</blockquote>\n";

            var result = markdown.Transform(input);

            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestParagrapheQuotes()
        {
            var markdown = new Markdown();
            string input = "Si je saisie un bloc de texte avec des passages à la ligne, cela n'a pas d'effet. Pour créer un vrai passage à la ligne dans une paragraphe, il faut que la ligne  \nse termine par deux espaces";
            string expected = "<p>Si je saisie un bloc de texte avec des passages à la ligne, cela n&#x27;a pas d&#x27;effet. Pour créer un vrai passage à la ligne dans une paragraphe, il faut que la ligne<br />\nse termine par deux espaces</p>\n";

            var result = markdown.Transform(input);

            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestBaliseHtlToEscape()
        {
            var markdown = new Markdown();
            string input = "Ceci devrait etre echapper : <script>alert(\"toto\");</script>";
            string expected = "<p>Ceci devrait etre echapper : &lt;script&gt;alert(&quot;toto&quot;);&lt;/script&gt;</p>\n";

            var result = markdown.Transform(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAutoHyperlinkPoor()
        {
            var options = new MarkdownOptions
            {
                AutoHyperlink = true,
                HyperlinkTarget = "_blank",
                HyperlinkClass = "external colorAccent",
            };
            var markdown = new Markdown(options);
            string input = "Ceci devrait donner un lien : www.bing.com";
            string expected = "<p>Ceci devrait donner un lien : <a href=\"http://www.bing.com\" target=\"_blank\" class=\"external colorAccent\">www.bing.com</a></p>\n";

            var result = markdown.Transform(input);

            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAutoHyperlinkPoorAndRich()
        {
            var options = new MarkdownOptions
            {
                AutoHyperlink = true,
                HyperlinkTarget = "_blank",
                HyperlinkClass = "external colorAccent",
            };
            var markdown = new Markdown(options);
            string input = "Ceci devrait donner un lien : www.bing.com et ceci aussi http://www.google.com";
            string expected = "<p>Ceci devrait donner un lien : <a href=\"http://www.bing.com\" target=\"_blank\" class=\"external colorAccent\">www.bing.com</a> et ceci aussi <a href=\"http://www.google.com\" target=\"_blank\" class=\"external colorAccent\">http://www.google.com</a></p>\n";

            var result = markdown.Transform(input);

            NUnit.Framework.Assert.AreEqual(expected, result);            
        }

        [TestMethod]
        public void TestAutoHyperlinkTrue()
        {
            var optiosn = new MarkdownOptions
            {
                AutoHyperlink = true,
            };
            var markdown = new Markdown(optiosn);
            string input = "foo http://example.com bar";
            string expected = "<p>foo <a href=\"http://example.com\">http://example.com</a> bar</p>\n";

            var result = markdown.Transform(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAutoHyperlinkNoBoldItalicsWithUnderscores()
        {
            var optiosn = new MarkdownOptions
            {
                AutoHyperlink = true,
            };
            var markdown = new Markdown(optiosn);
            string input = "foo http://ex_ample.com/ bar";
            string expected = "<p>foo <a href=\"http://ex_ample.com/\">http://ex_ample.com/</a> bar</p>\n";

            var result = markdown.Transform(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAutoHyperlinkNoBoldItalicsWithStars()
        {
            var optiosn = new MarkdownOptions
            {
                AutoHyperlink = true,
            };
            var markdown = new Markdown(optiosn);
            string input = "foo http://example.com/page*one.html bar";
            string expected = "<p>foo <a href=\"http://example.com/page\">http://example.com/page</a>*one.html bar</p>\n";

            var result = markdown.Transform(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAutoHyperlinkFalse()
        {
            var optiosn = new MarkdownOptions
            {
                AutoHyperlink = false,
            };
            var markdown = new Markdown(optiosn);
            string input = "foo http://example.com bar";
            string expected = "<p>foo http://example.com bar</p>\n";

            var result = markdown.Transform(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAutoHyperlink_WithTarget()
        {
            var optiosn = new MarkdownOptions
            {
                AutoHyperlink = true,
                HyperlinkTarget = "_blank",
            };
            var markdown = new Markdown(optiosn);
            string input = "foo http://example.com bar";
            string expected = "<p>foo <a href=\"http://example.com\" target=\"_blank\">http://example.com</a> bar</p>\n";

            var result = markdown.Transform(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAutoHyperlink_WithClass()
        {
            var optiosn = new MarkdownOptions
            {
                AutoHyperlink = true,
                HyperlinkClass = "external colorAccent",
            };
            var markdown = new Markdown(optiosn);
            string input = "foo http://example.com bar";
            string expected = "<p>foo <a href=\"http://example.com\" class=\"external colorAccent\">http://example.com</a> bar</p>\n";

            var result = markdown.Transform(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAutoHyperlink_WithTargetAndClass()
        {
            var optiosn = new MarkdownOptions
            {
                AutoHyperlink = true,
                HyperlinkTarget = "_blank",
                HyperlinkClass = "external colorAccent",
            };
            var markdown = new Markdown(optiosn);
            string input = "foo http://example.com bar";
            string expected = "<p>foo <a href=\"http://example.com\" target=\"_blank\" class=\"external colorAccent\">http://example.com</a> bar</p>\n";

            var result = markdown.Transform(input);

            Assert.AreEqual(expected, result);
        }

        //[TestMethod]
        //public void TestAutoHyperlink_WithDollar()
        //{
        //    var optiosn = new MarkdownOptions
        //    {
        //        AutoHyperlink = true,
        //        HyperlinkTarget = "$blank",
        //        HyperlinkClass = "external colorAccent",
        //    };
        //    var markdown = new Markdown(optiosn);
        //    string input = "foo http://example.com bar";
        //    string expected = "<p>foo <a href=\"http://example.com\" target=\"$blank\" class=\"external colorAccent\">http://example.com</a> bar</p>\n";

        //    var result = markdown.Transform(input);

        //    Assert.AreEqual(expected, result);
        //}

        //[TestMethod]
        //public void TestAutoHyperlink_WithDollarVar()
        //{
        //    var optiosn = new MarkdownOptions
        //    {
        //        AutoHyperlink = true,
        //        HyperlinkTarget = "$1",
        //        HyperlinkClass = "external colorAccent",
        //    };
        //    var markdown = new Markdown(optiosn);
        //    string input = "foo http://example.com bar";
        //    string expected = "<p>foo <a href=\"http://example.com\" target=\"$1\" class=\"external colorAccent\">http://example.com</a> bar</p>\n";

        //    var result = markdown.Transform(input);

        //    Assert.AreEqual(expected, result);
        //}

        [TestMethod]
        public void TestAutoHyperlinkFalse_WithAnchor()
        {
            var optiosn = new MarkdownOptions
            {
                AutoHyperlink = false,
                HyperlinkTarget = "$1",
                HyperlinkClass = "external colorAccent",
            };
            var markdown = new Markdown(optiosn);
            string input = "[foo](http://example.com)";
            string expected = "<p><a href=\"http://example.com\" target=\"$1\" class=\"external colorAccent\">foo</a></p>\n";

            var result = markdown.Transform(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAutoHyperlinkTrue_WithAnchor()
        {
            var optiosn = new MarkdownOptions
            {
                AutoHyperlink = true,
                HyperlinkTarget = "$1",
                HyperlinkClass = "external colorAccent",
            };
            var markdown = new Markdown(optiosn);
            string input = "[foo](http://example.com)";
            string expected = "<p><a href=\"http://example.com\" target=\"$1\" class=\"external colorAccent\">foo</a></p>\n";

            var result = markdown.Transform(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod, Ignore] // fix me
        public void TestAutoHyperlinkChevron()
        {
            var optiosn = new MarkdownOptions
            {
                AutoHyperlink = true,
                HyperlinkTarget = "_blank",
                HyperlinkClass = "external colorAccent",
            };
            var markdown = new Markdown(optiosn);
            string input = "Ceci devrait etre un lien : <www.google.com>";
            string expected = "<p>Ceci devrait etre un lien : &lt;<a href=\"http://www.google.com\" target=\"_blank\" class=\"external colorAccent\">www.google.com</a>></p>\n";

            var result = markdown.Transform(input);

            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMarkdownUnderscore()
        {
            var options = new MarkdownOptions
            {
                AutoHyperlink = false,
                StrictBoldItalic = false,
                HyperlinkTarget = "_blank",
                HyperlinkClass = "external colorAccent",
            };
            var markdown = new Markdown(options);
            string input = "[Ceci est un test de lien](http://bing.com)";
            string expected = "<p><a href=\"http://bing.com\" target=\"_blank\" class=\"external colorAccent\">Ceci est un test de lien</a></p>\n";

            var result = markdown.Transform(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMarkdownUnderscoreWithIta()
        {
            var options = new MarkdownOptions
            {
                AutoHyperlink = false,
                HyperlinkTarget = "_blank",
                HyperlinkClass = "external colorAccent",
            };
            var markdown = new Markdown(options);
            string input = "Coucou:\n_[Ceci est un test de lien](http://bing.com/coucou_try)_";
            string expected = "<p>Coucou:\n<em><a href=\"http://bing.com/coucou_try\" target=\"_blank\" class=\"external colorAccent\">Ceci est un test de lien</a></em></p>\n";

            var result = markdown.Transform(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAutoHyperlinkWithUnderscore()
        {
            var options = new MarkdownOptions
            {
                AutoHyperlink = true,
                HyperlinkTarget = "_blank",
                HyperlinkClass = "external colorAccent",
            };
            var markdown = new Markdown(options);
            string input = "Coucou je suis un http://www.hyper_lien.hyper/";
            string expected = "<p>Coucou je suis un <a href=\"http://www.hyper_lien.hyper/\" target=\"_blank\" class=\"external colorAccent\">http://www.hyper_lien.hyper/</a></p>\n";

            var result = markdown.Transform(input);

            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMarkdownAnchorLinkPoor()
        {
            var options = new MarkdownOptions
            {
                AutoHyperlink = true,
                HyperlinkTarget = "_blank",
                HyperlinkClass = "external colorAccent",
            };
            var markdown = new Markdown(options);
            string input = "[Ceci est un test de lien](www.bing.com)  http://autrelien.com";
            string expected = "<p><a href=\"http://www.bing.com\" target=\"_blank\" class=\"external colorAccent\">Ceci est un test de lien</a>  <a href=\"http://autrelien.com\" target=\"_blank\" class=\"external colorAccent\">http://autrelien.com</a></p>\n";

            var result = markdown.Transform(input);

            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAutolinkEmail()
        {
            var options = new MarkdownOptions
            {
                AutoHyperlink = true,
                LinkEmails = true,
                HyperlinkTarget = "_blank",
                HyperlinkClass = "external colorAccent",
            };
            var markdown = new Markdown(options);
            string input = "Coucou ceci est un email : toto@inmysworditrust.com";
            string expected = "<p>Coucou ceci est un email : <a href=\"mailto:toto@inmysworditrust.com\">toto@inmysworditrust.com</a></p>\n";

            var result = markdown.Transform(input);

            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestLineBreaks()
        {
            var markdown = new Markdown();
            string input = "Ceci est un petit paragraphe de test\nPour voir si le br fonctionne bien  \nAlors quel est le verdict?";
            string expected = "<p>Ceci est un petit paragraphe de test\nPour voir si le br fonctionne bien<br />\nAlors quel est le verdict?</p>\n";

            var result = markdown.Transform(input);

            NUnit.Framework.Assert.AreEqual(expected, result);
        }
    }
}
