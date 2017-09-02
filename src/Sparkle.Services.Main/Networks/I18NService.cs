
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Sparkle.Data.Networks;
    using Sparkle.Services.Networks;

    public class I18NService : ServiceBase, II18NService
    {
        private List<CultureInfo> availableCulturesCache;

        private readonly System.Text.RegularExpressions.Regex replaceRegex =
            new System.Text.RegularExpressions.Regex("{([a-zA-Z]+)(?: ([ /,\"a-zA-Z]+))?}", System.Text.RegularExpressions.RegexOptions.Compiled);

        [DebuggerStepThrough]
        internal I18NService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public CultureInfo GetAnyAvailableCulture()
        {
            return this.GetAvailableCultures().First();
        }

        public CultureInfo GetAvailableCulture(string name)
        {
            foreach (var culture in this.GetAvailableCultures())
            {
                if (culture.Name == name)
                    return culture;
            }

            foreach (var culture in this.GetAvailableCultures())
            {
                if (culture.Parent != null && culture.Parent.Name == name)
                    return culture;
            }

            return this.GetAnyAvailableCulture();
        }

        public IList<CultureInfo> GetAvailableCultures()
        {
            if (this.availableCulturesCache != null)
            {
                return this.availableCulturesCache.ToList();
            }

            var list = new List<CultureInfo>();
            this.availableCulturesCache = list;
            if (string.IsNullOrEmpty(this.Services.AppConfiguration.Tree.Features.I18N.AvailableCultures))
            {
                try
                {
                    list.Add(new CultureInfo(this.Services.AppConfiguration.Tree.DefaultCulture ?? "fr-FR"));
                    return list;
                }
                catch (CultureNotFoundException)
                {
                }
            }

            var available = this.Services.AppConfiguration.Tree.Features.I18N.AvailableCultures.Split(';');
            foreach (var name in available)
            {
                try
                {
                    list.Add(new CultureInfo(name));
                }
                catch (CultureNotFoundException)
                {
                }
            }

            return list;
        }

        public string ReplaceVariables<T>(string value, T model, IDictionary<string, Func<T, string, string>> variables)
        {
            return replaceRegex.Replace(value, x =>
            {
                if (x.Groups.Count > 1)
                {
                    var key = x.Groups[1].Value;
                    var param = x.Groups.Count > 2 ? x.Groups[2].Value : "";
                    if (key == "L")
                        return this.Services.Lang.T(param);
                    else if (variables.ContainsKey(key))
                        return variables[key](model, param);
                }

                return x.Groups[0].Value;
            });
        }
    }
}
