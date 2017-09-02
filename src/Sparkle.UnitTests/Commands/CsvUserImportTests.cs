
namespace Sparkle.UnitTests.Commands
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sparkle.Commands.Main;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    [TestClass]
    public class CsvUserImportTests
    {
        [TestClass]
        public class PrepareParserMethod
        {
            [TestMethod]
            public void CompanyName()
            {
                var header = "company.name";
                var context = new CsvUserImport.Context
                {
                    Header = header.Split(','),
                };
                var subject = new TestCsvUserImport();
                Assert.IsTrue(subject.ExecutePrepareParser(context));

                Assert.IsNotNull(context.Table);
                Assert.IsNotNull(context.Table.Columns);
                Assert.AreEqual(1, context.Table.Columns.Count);
                Assert.AreEqual(0, context.Table.Columns[0].Index);
            }

            [TestMethod]
            public void CompanyName_UserFirstname()
            {
                var header = "company.name,User.Firstname";
                var context = new CsvUserImport.Context
                {
                    Header = header.Split(','),
                };
                var subject = new TestCsvUserImport();
                Assert.IsTrue(subject.ExecutePrepareParser(context));

                Assert.IsNotNull(context.Table);
                Assert.IsNotNull(context.Table.Columns);
                Assert.AreEqual(2, context.Table.Columns.Count);
                Assert.AreEqual(0, context.Table.Columns[0].Index);
                Assert.AreEqual(1, context.Table.Columns[1].Index);
            }
        }

        [TestClass]
        public class ParseCsvMethod
        {
            [TestMethod]
            public void Simple()
            {
                var csv = "company name,first name" + Environment.NewLine
                    + "company 1,antoine" + Environment.NewLine
                    + "company 2,Céline" + Environment.NewLine
                    + "company unquoted,sarrah" + Environment.NewLine;
                var reader = new StringReader(csv);
                var subject = new TestCsvUserImport();
                var context = new CsvUserImport.Context
                {
                    CsvSeparator = ',',
                };
                Assert.IsTrue(subject.ExecuteParseCsv(context, reader));

                Assert.IsNotNull(context.CsvData);
                Assert.AreEqual(4, context.CsvData.Count);
                for (int i = 0; i < context.CsvData.Count; i++)
                {
                    Assert.AreEqual(2, context.CsvData[i].Count, "row " + i + " should contain 2 values");
                }

                SrkToolkit.Testing.Assert.AreEqual("company name", context.CsvData[0][0]);
                SrkToolkit.Testing.Assert.AreEqual("first name", context.CsvData[0][1]);

                SrkToolkit.Testing.Assert.AreEqual("company 1", context.CsvData[1][0]);
                SrkToolkit.Testing.Assert.AreEqual("antoine", context.CsvData[1][1]);

                SrkToolkit.Testing.Assert.AreEqual("company 2", context.CsvData[2][0]);
                SrkToolkit.Testing.Assert.AreEqual("Céline", context.CsvData[2][1]);

                SrkToolkit.Testing.Assert.AreEqual("company unquoted", context.CsvData[3][0]);
                SrkToolkit.Testing.Assert.AreEqual("sarrah", context.CsvData[3][1]);
            }

            [TestMethod]
            public void SimpleQuotes()
            {
                var csv = "company name,first name" + Environment.NewLine
                    + "company 1,antoine" + Environment.NewLine
                    + "\"company 2\",Céline" + Environment.NewLine
                    + "\"company unquoted\",sarrah" + Environment.NewLine;
                var reader = new StringReader(csv);
                var subject = new TestCsvUserImport();
                var context = new CsvUserImport.Context
                {
                    CsvSeparator = ',',
                };
                Assert.IsTrue(subject.ExecuteParseCsv(context, reader));

                Assert.IsNotNull(context.CsvData);
                Assert.AreEqual(4, context.CsvData.Count);
                for (int i = 0; i < context.CsvData.Count; i++)
                {
                    Assert.AreEqual(2, context.CsvData[i].Count, "row " + i + " should contain 2 values");
                }

                SrkToolkit.Testing.Assert.AreEqual("company name", context.CsvData[0][0]);
                SrkToolkit.Testing.Assert.AreEqual("first name", context.CsvData[0][1]);

                SrkToolkit.Testing.Assert.AreEqual("company 1", context.CsvData[1][0]);
                SrkToolkit.Testing.Assert.AreEqual("antoine", context.CsvData[1][1]);

                SrkToolkit.Testing.Assert.AreEqual("company 2", context.CsvData[2][0]);
                SrkToolkit.Testing.Assert.AreEqual("Céline", context.CsvData[2][1]);

                SrkToolkit.Testing.Assert.AreEqual("company unquoted", context.CsvData[3][0]);
                SrkToolkit.Testing.Assert.AreEqual("sarrah", context.CsvData[3][1]);
            }

            [TestMethod]
            public void EscapedQuotes()
            {
                var csv = "company name,first name" + Environment.NewLine
                    + "comp\"\"any 1,antoine" + Environment.NewLine
                    + "co\"mpany 2,Céline" + Environment.NewLine
                    + "\"company \"\"quoted\"\"\",sarrah" + Environment.NewLine;
                var reader = new StringReader(csv);
                var subject = new TestCsvUserImport();
                var context = new CsvUserImport.Context
                {
                    CsvSeparator = ',',
                };
                Assert.IsTrue(subject.ExecuteParseCsv(context, reader));

                Assert.IsNotNull(context.CsvData);
                Assert.AreEqual(4, context.CsvData.Count);
                for (int i = 0; i < context.CsvData.Count; i++)
                {
                    Assert.AreEqual(2, context.CsvData[i].Count, "row " + i + " should contain 2 values");
                }

                SrkToolkit.Testing.Assert.AreEqual("company name", context.CsvData[0][0]);
                SrkToolkit.Testing.Assert.AreEqual("first name", context.CsvData[0][1]);

                SrkToolkit.Testing.Assert.AreEqual("comp\"\"any 1", context.CsvData[1][0]);
                SrkToolkit.Testing.Assert.AreEqual("antoine", context.CsvData[1][1]);

                SrkToolkit.Testing.Assert.AreEqual("co\"mpany 2", context.CsvData[2][0]);
                SrkToolkit.Testing.Assert.AreEqual("Céline", context.CsvData[2][1]);

                SrkToolkit.Testing.Assert.AreEqual("company \"quoted\"", context.CsvData[3][0]);
                SrkToolkit.Testing.Assert.AreEqual("sarrah", context.CsvData[3][1]);
            }

            [TestMethod]
            public void EmptyCells()
            {
                var csv = "c1,c2,c3" + Environment.NewLine
                    + "a1,,a3" + Environment.NewLine
                    + "b1,b2," + Environment.NewLine
                    + ",c2,c3" + Environment.NewLine
                    + ",," + Environment.NewLine
                    + ",\"\"," + Environment.NewLine;
                var reader = new StringReader(csv);
                var subject = new TestCsvUserImport();
                var context = new CsvUserImport.Context
                {
                    CsvSeparator = ',',
                };
                Assert.IsTrue(subject.ExecuteParseCsv(context, reader));

                Assert.IsNotNull(context.CsvData);
                Assert.AreEqual(6, context.CsvData.Count);
                for (int i = 0; i < context.CsvData.Count; i++)
                {
                    Assert.AreEqual(3, context.CsvData[i].Count, "row " + i + " should contain 2 values");
                }

                SrkToolkit.Testing.Assert.AreEqual("c1", context.CsvData[0][0]);
                SrkToolkit.Testing.Assert.AreEqual("c2", context.CsvData[0][1]);
                SrkToolkit.Testing.Assert.AreEqual("c3", context.CsvData[0][2]);

                SrkToolkit.Testing.Assert.AreEqual("a1", context.CsvData[1][0]);
                SrkToolkit.Testing.Assert.AreEqual("", context.CsvData[1][1]);
                SrkToolkit.Testing.Assert.AreEqual("a3", context.CsvData[1][2]);

                SrkToolkit.Testing.Assert.AreEqual("b1", context.CsvData[2][0]);
                SrkToolkit.Testing.Assert.AreEqual("b2", context.CsvData[2][1]);
                SrkToolkit.Testing.Assert.AreEqual("", context.CsvData[2][2]);

                SrkToolkit.Testing.Assert.AreEqual("", context.CsvData[3][0]);
                SrkToolkit.Testing.Assert.AreEqual("c2", context.CsvData[3][1]);
                SrkToolkit.Testing.Assert.AreEqual("c3", context.CsvData[3][2]);

                SrkToolkit.Testing.Assert.AreEqual("", context.CsvData[4][0]);
                SrkToolkit.Testing.Assert.AreEqual("", context.CsvData[4][1]);
                SrkToolkit.Testing.Assert.AreEqual("", context.CsvData[4][2]);

                SrkToolkit.Testing.Assert.AreEqual("", context.CsvData[5][0]);
                SrkToolkit.Testing.Assert.AreEqual("", context.CsvData[5][1]);
                SrkToolkit.Testing.Assert.AreEqual("", context.CsvData[5][2]);
            }
        }
    }

    public class TestCsvUserImport : CsvUserImport
    {
        public TestCsvUserImport()
        {
            this.Out = new StreamWriter(new MemoryStream());
            this.Error = new StreamWriter(new MemoryStream());
            this.In = new StringReader("");
        }

        public bool ExecutePrepareParser(Context context)
        {
            return this.PrepareParser(context);
        }

        public bool ExecuteParseCsv(Context context, TextReader reader)
        {
            return this.ParseCsv(context, reader);
        }
    }
}
