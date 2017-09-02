
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public interface II18NService
    {
        string ReplaceVariables<T>(string value, T model, IDictionary<string, Func<T, string, string>> variables);
        ////IList<CultureInfo> GetAvailableCultures();
        ////CultureInfo GetAvailableCulture(string name);
        ////CultureInfo GetAnyAvailableCulture();
    }
}
