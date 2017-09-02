
namespace Sparkle.UnitTests.Mocks
{
    using Sparkle.UI;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    internal class TestStrings : Strings
    {
        internal TestStrings(Strings original)
            : base(original)
        {
        }

        internal static new TestStrings Load(string dir, string app, string defaultCulture)
        {
            return new TestStrings(Strings.Load(dir, app, defaultCulture));
        }

        internal new CultureInfo GetBestCulture(CultureInfo info)
        {
            return base.GetBestCulture(info);
        }
    }
}
