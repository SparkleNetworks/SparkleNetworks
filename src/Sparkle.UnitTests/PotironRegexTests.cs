
namespace Sparkle.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Helps buidl regexes to match Lang.T and Lang.M usages in source code.
    /// </summary>
    [TestClass]
    public class PotironRegexTests
    {
        private static List<Regex> regexes = new List<Regex>()
        {
            new Regex(@"Lang\.(?:T|M)\(""([^""]*)""\)"),
        };

        [TestMethod]
        public void LangTSimple()
        {
            string line = @"lala @Lang.T(""hello world"")";
            VerifySingleMatch(line, "hello world");
        }

        [TestMethod]
        public void LangTMultiple()
        {
            string line = @"lala @Lang.T(""hello world"")lala @Lang.T(""hello guys"")";
            var expected = new string[] { "hello world", "hello guys", };
            VerifyExclusiveMatch(line, expected);
        }

        private void VerifyExclusiveMatch(string line, string[] expected)
        {
            var results = GetMatches(line);

            if (results == null || results.Count == 0)
            {
                Assert.Fail(string.Format("No match for '{0}'", expected[0]));
            }

            var matches = GetMatches(line);
            var matched = new Dictionary<string, int>(expected.Length);

            foreach (var exp in expected)
            {
                matched.Add(exp, 0);
            }

            foreach (var item in matches)
            {
                if (matched.ContainsKey(item))
                {
                    matched[item] += 1;
                }
                else
                {
                    Assert.Fail(string.Format("Unexpected match '{0}'", item));
                }
            }

            foreach (var item in matched)
            {
                if (item.Value == 0)
                {
                    Assert.Fail(string.Format("No match for '{0}'", item.Key));
                }
            }
        }

        private static void VerifySingleMatch(string line, string result)
        {
            var results = GetMatches(line);

            if (results == null || results.Count == 0)
            {
                Assert.Fail(string.Format("No match for '{0}'", result));
            }

            foreach (var match in results)
            {
                Assert.AreEqual(result, match);
                ////Assert.Fail(string.Format("Match should have found only '{0}' but '{1}' was found", result, match));
            }
        }

        private static List<string> GetMatches(string line)
        {
            var results = new List<string>();
            foreach (var regex in regexes)
            {
                foreach (Match match in regex.Matches(line))
                {
                    if (match.Success)
                    {
                        int groupIndex = -1;
                        foreach (Group group in match.Groups)
                        {
                            groupIndex += 1;
                            if (groupIndex > 0)
                            {
                                Trace.TraceInformation(string.Format("Captured '{0}'", group.Value));
                                results.Add(group.Value);
                            }
                        }
                    }
                }
            }

            return results;
        }
    }
}
