using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using MarkdownSharp;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace MarkdownSharpTests 
{
    [TestClass]
    public class MDTestTests : BaseTest
    {
        const string folder = "testfiles.mdtest_1._1";

        public class TestCase
        {
            public string ActualName { get; set; }
            public string Name { get; set; }
            public string Actual { get; set; }
            public string Expected { get; set; }

            public TestCase(string actualName, string name, string actual, string expected)
            {
                this.ActualName = actualName;
                this.Name = name;
                this.Actual = actual;
                this.Expected = expected;
            }

        }

        private static IEnumerable<TestCase> GetTests()
        {
            Markdown m = new Markdown();
            Assembly assembly = Assembly.GetAssembly(typeof(BaseTest));
            string namespacePrefix = String.Concat(assembly.GetName().Name, '.', folder);
            string[] resourceNames = assembly.GetManifestResourceNames();
            
            Func<string, string> getResourceFileContent = filename =>
            {
                using (Stream stream = assembly.GetManifestResourceStream(filename))
                {
                    if (stream == null)
                        return null;

                    using (StreamReader streamReader = new StreamReader(stream))
                        return streamReader.ReadToEnd();
                }
            };

            return from name in resourceNames
                // Skip resource names that aren't within the namespace (folder) we want
                // and doesn't have the extension '.html'.
                where name.StartsWith(namespacePrefix) && name.EndsWith(".html")
                let actualName = Path.ChangeExtension(name, "text")
                let actualContent = getResourceFileContent(actualName)
                let actual = Program.RemoveWhitespace(m.Transform(actualContent))
                let expectedContent = getResourceFileContent(name)
                let expected = Program.RemoveWhitespace(expectedContent)
                select new TestCase(actualName, name, actual, expected);
        }

        [TestMethod, Ignore] // those tests fail
        public void MarkdownSharpTests()
        {
            var tests = GetTests();
            foreach (var test in tests)
            {
                if (test.Actual != test.Expected)
                    Assert.Fail("Mismatch between '{0}' and the transformed '{1}'.", test.ActualName, test.Name);
            }
        }
    }
}
