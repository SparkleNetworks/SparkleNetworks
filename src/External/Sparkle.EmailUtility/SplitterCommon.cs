
namespace Sparkle.EmailUtility
{
    using HtmlAgilityPack;
    using Sparkle.EmailUtility.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    public static class SplitterCommon
    {
        private static readonly Regex SignatureRegex = new Regex("\n(--[ \u00a0\u200b]+\n.*)$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly string[] ExcludedHtmlElementNames = { "style", "script", "head", "title", "meta", "frame", "iframe", "link", "base", "object", "embed" };

        private static readonly string[] HtmlBlocks = { "div", "p", "pre", "body", "html", "form", "blockquote", "article", "section", "center", "dl", "dt", "dd", "fieldset", "h1", "h2", "h3", "h4", "h5", "h6", "textarea", "table" };
        private static readonly string[] HtmlInline = { "strong", "em", "span", "a", "u", "b", "i", "cite", "code" };

        public static string SplitSignature(string input, out string signature)
        {
            var message = HttpUtility.HtmlDecode(input);
            var match = SignatureRegex.Match(message);
            if (match.Success)
            {
                signature = match.Groups[1].Value;
                message = SignatureRegex.Replace(message, "");
            }
            else
            {
                signature = null;
            }

            return message;
        }

        public static string ScrubHtml(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var tree = NewScrubHtmlRec(htmlDoc.DocumentNode);
            var toRet = HtmlAgilityTreeToString(tree);

            return toRet;
        }

        public static HtmlAgilityNode NewScrubHtmlRec(HtmlNode document)
        {
            var node = new HtmlAgilityNode();
            if ((document.NodeType == HtmlNodeType.Element || document.NodeType == HtmlNodeType.Document) && document.ChildNodes.Count == 1 && document.ChildNodes.First().NodeType != HtmlNodeType.Text)
                return NewScrubHtmlRec(document.ChildNodes.First());
            if (document.NodeType == HtmlNodeType.Text)
            {
                node.BlockType = HtmlAgilityBlockType.Inline;
                node.NodeType = HtmlAgilityNodeType.Text;
                node.Text = document.InnerText;
                return node;
            }
            else if (document.NodeType == HtmlNodeType.Element)
            {
                node.BlockType = HtmlBlocks.Contains(document.Name.ToLowerInvariant()) ? HtmlAgilityBlockType.Block : HtmlAgilityBlockType.Inline;
                if (document.Name == "br")
                    node.NodeType = HtmlAgilityNodeType.Br;
                else if (document.Name == "p")
                    node.NodeType = HtmlAgilityNodeType.Par;
                else if (document.Name == "div")
                    node.NodeType = HtmlAgilityNodeType.Div;
                else if (document.Name == "pre")
                    node.NodeType = HtmlAgilityNodeType.Pre;
                else if (document.Name == "span")
                    node.NodeType = HtmlAgilityNodeType.Span;
                else if (document.Name == "a")
                    node.NodeType = HtmlAgilityNodeType.Link;
                else if (document.Name == "b" || document.Name == "strong")
                    node.NodeType = HtmlAgilityNodeType.Strong;
                else if (document.Name == "i" || document.Name == "em")
                    node.NodeType = HtmlAgilityNodeType.Ita;
                else
                    node.NodeType = HtmlAgilityNodeType.Element;

                foreach (var att in document.Attributes)
                {
                    node.Attributes.Add(att.Name, att.Value);
                }
            }

            foreach (var child in document.ChildNodes.Where(o => !ExcludedHtmlElementNames.Contains(o.Name)))
            {
                node.Content.Add(NewScrubHtmlRec(child));
            }

            return node;
        }

        public static string HtmlAgilityTreeToString(HtmlAgilityNode tree)
        {
            if (tree.NodeType == HtmlAgilityNodeType.Text && tree.BlockType == HtmlAgilityBlockType.Inline)
                return tree.Text;

            IList<IList<string>> blocks = new List<IList<string>>();
            blocks.Add(new List<string>());
            MakeBlocks(tree, blocks);

            IList<string> toJoin = new List<string>();
            foreach (var block in blocks)
            {
                toJoin.Add(string.Join("", block.ToArray()));
            }

            return string.Join("\n", toJoin.ToArray());
        }

        private static int GetInlineElementsLength(HtmlAgilityNode node)
        {
            int length = 0;
            foreach (var child in node.Content)
            {
                if (child.NodeType != HtmlAgilityNodeType.Text)
                    length += GetInlineElementsLength(child);
                else
                    length += child.Text.Length;
            }

            return length;
        }

        private static void AddToBlocks(IList<IList<string>> blocks, bool wasPar)
        {
            if (blocks.Last().Count > 0)
            {
                //if (wasPar)
                //    blocks.Last().Add("\n");
                blocks.Add(new List<string>());
            }
        }

        private static string MakeInline(HtmlAgilityNode node, StringBuilder builder)
        {
            if (node.NodeType == HtmlAgilityNodeType.Text)
                return node.Text;
            if (node.NodeType == HtmlAgilityNodeType.Br)
                return "\n";

            var sb = builder != null ? builder : new StringBuilder(GetInlineElementsLength(node));
            foreach (var child in node.Content)
            {
                if (child.NodeType == HtmlAgilityNodeType.Text)
                    sb.Append(child.Text);
                else if (child.NodeType == HtmlAgilityNodeType.Br)
                    sb.Append("\n");
                else
                    MakeInline(child, sb);
            }

            return sb.ToString();
        }

        private static void AddToInline(IList<IList<string>> blocks, string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                blocks.Last().Add(str);
            }
        }

        private static void MakeBlocks(HtmlAgilityNode node, IList<IList<string>> blocks)
        {
            foreach (var child in node.Content)
            {
                if (child.BlockType == HtmlAgilityBlockType.Inline || child.NodeType == HtmlAgilityNodeType.Pre)
                {
                    var inlineStr = MakeInline(child, null);
                    if (child.NodeType == HtmlAgilityNodeType.Pre)
                    {
                        inlineStr = "    " + inlineStr.Replace("\n", "\n    ");
                    }
                    else if (child.NodeType == HtmlAgilityNodeType.Link)
                    {
                        var href = "";
                        if (child.Attributes.ContainsKey("href"))
                            href = child.Attributes["href"];
                        inlineStr = "[" + inlineStr.Replace("\n", "") + "](" + href + ")";
                    }
                    else if (child.NodeType == HtmlAgilityNodeType.Strong)
                    {
                        inlineStr = "__" + inlineStr + "__";
                    }
                    else if (child.NodeType == HtmlAgilityNodeType.Ita)
                    {
                        inlineStr = "*" + inlineStr + "*";
                    }
                    else if (child.NodeType != HtmlAgilityNodeType.Br)
                    {
                        inlineStr = inlineStr.Replace("\n", "");
                    }
                    AddToInline(blocks, inlineStr);
                }
                else
                {
                    AddToBlocks(blocks, child.NodeType == HtmlAgilityNodeType.Par ? true : false);
                    MakeBlocks(child, blocks);
                    AddToBlocks(blocks, child.NodeType == HtmlAgilityNodeType.Par ? true : false);
                }
            }

        }

    }
}
