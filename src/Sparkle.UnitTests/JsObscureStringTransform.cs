
namespace Sparkle.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sparkle.WebBase;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestClass]
    public class JsObscureStringTransformTests
    {
        [TestMethod]
        public void SingleQuoteUrl()
        {
            var target = GetTarget();
            var input = @"// this is a js file
var url = '/Ajax/Do';
echo(url);
";
            var expected = @"// this is a js file
var url = '\x2FAjax\x2FDo';
echo(url);
";
            var result = target.Process(string.Empty, input);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DoubleQuoteUrl()
        {
            var target = GetTarget();
            var input = @"// this is a js file
var url = ""/Ajax/Do"";
echo(url);
";
            var expected = @"// this is a js file
var url = ""\x2FAjax\x2FDo"";
echo(url);
";
            var result = target.Process(string.Empty, input);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TwoInOneLine()
        {
            var target = GetTarget();
            var input = @"// this is a js file
var url = '/Ajax/Do'; var url2 = '/Ajax/Foo';
echo(url);
";
            var expected = @"// this is a js file
var url = '\x2FAjax\x2FDo'; var url2 = '\x2FAjax\x2FFoo';
echo(url);
";
            var result = target.Process(string.Empty, input);
            Assert.AreEqual(expected, result);
        }

        private static JsObscureStringTransform GetTarget()
        {
            return new JsObscureStringTransform();
        }
    }
}
