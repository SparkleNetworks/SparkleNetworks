
namespace Sparkle.UI
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using Gettext.Cs;

    /// <summary>
    /// Recursive proxy to gettext. This thing loads .PO files and allows finding a translation trhough many files.
    /// </summary>
    public class Strings
    {
        private readonly IDictionary<CultureInfo, IDictionary<string, string>> sets = new Dictionary<CultureInfo, IDictionary<string, string>>();

        private CultureInfo defaultCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;

        public Strings()
        {
            this.sets = new Dictionary<CultureInfo, IDictionary<string, string>>();
            this.defaultCulture = CultureInfo.CurrentCulture;
        }

        private Strings(IDictionary<CultureInfo, IDictionary<string, string>> sets, CultureInfo defaultCulture)
        {
            this.sets = sets;
            this.defaultCulture = defaultCulture;
        }

        protected Strings(Strings original)
            : this(original.sets, original.defaultCulture)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.StartsWith(System.String)")]
        public static Strings LoadFromConfiguration()
        {
            var dir = ConfigurationManager.AppSettings["GettextRootDir"];
            var app = ConfigurationManager.AppSettings["GettextApplication"];
            var dcu = ConfigurationManager.AppSettings["GettextDefaultCulture"];
            var appf = ConfigurationManager.AppSettings["GettextFallbackApplication"];
            var dcuf = ConfigurationManager.AppSettings["GettextFallbackDefaultCulture"];

            if (string.IsNullOrEmpty(dir))
                throw new InvalidOperationException("The value of 'GettextRootDir' cannot be empty");
            if (string.IsNullOrEmpty(app))
                throw new InvalidOperationException("The value of 'GettextApplication' cannot be empty");
            if (string.IsNullOrEmpty(dcu))
                throw new InvalidOperationException("The value of 'GettextDefaultCulture' cannot be empty");

            if (dir.StartsWith("~"))
                dir = HttpContext.Current.Server.MapPath(dir);

            var main = Load(dir, app, dcu);

            if (!string.IsNullOrEmpty(appf) && !string.IsNullOrEmpty(dcuf))
                main.Fallback = Load(dir, appf, dcuf);

            return main;
        }

        public static Strings Load(string directory, string application, string defaultCulture)
        {
            if (!Directory.Exists(directory))
                throw new InvalidOperationException("The directory '"+directory+"' for Gettext translations does not exists");

            var defaultCultureInfo = new CultureInfo(defaultCulture);
            var sets = new Dictionary<CultureInfo, IDictionary<string, string>>();
            var parser = new PoParser();
            var cultureRegex = new Regex(@"\.[a-z]{2}(-[a-z]{2}(-[a-z]{2})?)?$", RegexOptions.IgnoreCase);

            var files = Directory.GetFiles(directory, application + "*.po", SearchOption.TopDirectoryOnly).ToList();
            foreach (var file in files)
            {
                var filename = Path.GetFileNameWithoutExtension(file); // something like "App1.fr-fr" or "App1"
                string culture = string.Empty;
                CultureInfo info = null;
                if (!filename.StartsWith(application))
                    continue;

                if (filename.Contains('.') && cultureRegex.IsMatch(filename))
                {
                    culture = filename.Substring(filename.LastIndexOf('.') + 1); // remove left side of the filename
                    try
                    {
                        info = new CultureInfo(culture);
                    }
                    catch (ArgumentException)
                    {
                        Trace.TraceError("Strings: file '" + filename + "': unknown culture '" + culture + "'");
                        continue;
                    }
                }
                else
                {
                    info = defaultCultureInfo;
                }

                if (info == null)
                {
                    Trace.TraceError("Strings: file '" + filename + "': unknown culture '" + culture + "'");
                    continue;
                }

                var set = new DictionaryGettextParserRequestor();
                FileStream stream = null;
                try
                {
                    stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    var reader = new StreamReader(stream);
                    try
                    {
                        parser.Parse(reader, set);
                        sets.Add(info, set);
                        ////Trace.TraceInformation("Strings: read file '" + filename + "' c='" + culture + "'");
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                catch (IOException ex)
                {
                    Trace.TraceError("Strings: file '" + filename + "':  '" + ex.Message + "'");
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }

            return new Strings(sets, defaultCultureInfo);
        }

        public static Strings Empty
        {
            get { return new Strings(); }
        }

        public Strings Fallback { get; set; }

        public IList<CultureInfo> AvailableCultures
        {
            get { return this.sets.Keys.ToList(); }
        }

        /// <summary>
        /// Returns a translated string.
        /// If the translation is not for the specified culture, it will be searched in the other available cultures.
        /// If the translation does not exists, the specified value is directly returned.
        /// </summary>
        /// <param name="value">The sentence to translate.</param>
        /// <returns>The translated sentence</returns>
        public string T(string value)
        {
            return this.T(null, value);
        }

        /// <summary>
        /// Returns a translated string.
        /// If the translation is not for the specified culture, it will be searched in the other available cultures.
        /// If the translation does not exists, the specified value is directly returned.
        /// </summary>
        /// <param name="info">The desired culture for translation.</param>
        /// <param name="value">The sentence to translate.</param>
        /// <returns>The translated sentence</returns>
        public string T(CultureInfo info, string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            CultureInfo culture = this.GetBestCulture(info);

            string @string = this.GetStringSafely(value, culture);
            
            if (@string == null && this.Fallback != null) {
                @string = this.Fallback.T(info, value);
            }

            if (@string != null) {
                return @string;
            }

            return value;
        }

        /// <summary>
        /// Returns a translated formatted string.
        /// If the translation is not for the specified culture, it will be searched in the other available cultures.
        /// If the translation does not exists, the specified value is directly returned.
        /// </summary>
        /// <param name="value">The sentence to translate.</param>
        /// <returns>The translated sentence</returns>
        public string T(string value, params object[] parameters)
        {
            return this.T(null, value, parameters);
        }

        /// <summary>
        /// Returns a translated formatted string.
        /// If the translation is not for the specified culture, it will be searched in the other available cultures.
        /// If the translation does not exists, the specified value is directly returned.
        /// </summary>
        /// <param name="info">The desired culture for translation.</param>
        /// <param name="value">The sentence to translate.</param>
        /// <returns>The translated sentence</returns>
        public string T(CultureInfo info, string value, params object[] parameters)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            CultureInfo culture = this.GetBestCulture(info);

            string @string = this.GetStringSafely(value, culture);
            
            if (@string == null && this.Fallback != null) {
                @string = this.Fallback.T(info, value);
            }

            if (@string != null)
            {
                return string.Format(culture, @string, parameters);
            }

            return string.Format(culture, value, parameters);
        }

        /// <summary>
        /// Returns a translated string.
        /// If the translation is not for the specified culture, it will be searched in the other available cultures.
        /// If the translation does not exists, the specified value is directly returned.
        /// </summary>
        /// <param name="value">The sentence to translate.</param>
        /// <returns>The translated sentence</returns>
        public string M(string singularValue, string pluralValue, int count)
        {
            return this.M(null, singularValue, pluralValue, count);
        }

        /// <summary>
        /// Returns a translated string.
        /// If the translation is not for the specified culture, it will be searched in the other available cultures.
        /// If the translation does not exists, the specified value is directly returned.
        /// </summary>
        /// <param name="info">The desired culture for translation.</param>
        /// <param name="value">The sentence to translate.</param>
        /// <returns>The translated sentence</returns>
        public string M(CultureInfo info, string singularValue, string pluralValue, int count)
        {
            if (string.IsNullOrEmpty(singularValue) && count == 1)
                return singularValue;
            if (string.IsNullOrEmpty(pluralValue) && count != 1)
                return pluralValue;

            CultureInfo culture = this.GetBestCulture(info);

            string @string = count == 1 ? this.GetStringSafely(singularValue, culture) : this.GetStringSafely(pluralValue, culture);

            if (@string == null && this.Fallback != null)
            {
                @string = this.Fallback.M(info, singularValue, pluralValue, count);
            }

            if (@string != null)
            {
                return string.Format(culture, @string, count);
            }

            return string.Format(culture, count == 1 ? singularValue : pluralValue, count);
        }

        /// <summary>
        /// Returns a translated formatted string.
        /// If the translation is not for the specified culture, it will be searched in the other available cultures.
        /// If the translation does not exists, the specified value is directly returned.
        /// </summary>
        /// <param name="value">The sentence to translate.</param>
        /// <returns>The translated sentence</returns>
        public string M(string singularValue, string pluralValue, int count, params object[] parameters)
        {
            return this.M(null, singularValue, pluralValue, count, parameters);
        }

        /// <summary>
        /// Returns a translated formatted string.
        /// If the translation is not for the specified culture, it will be searched in the other available cultures.
        /// If the translation does not exists, the specified value is directly returned.
        /// </summary>
        /// <param name="info">The desired culture for translation.</param>
        /// <param name="value">The sentence to translate.</param>
        /// <returns>The translated sentence</returns>
        public string M(CultureInfo info, string singularValue, string pluralValue, int count, params object[] parameters)
        {
            if (string.IsNullOrEmpty(singularValue) && count == 1)
                return singularValue;
            if (string.IsNullOrEmpty(pluralValue) && count != 1)
                return pluralValue;
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            CultureInfo culture = this.GetBestCulture(info);

            var realParams = new object[parameters.Length + 1];
            realParams[0] = count;
            for (int i = 0; i < parameters.Length; i++)
                realParams[i + 1] = parameters[i];

            string @string = count == 1 ? this.GetStringSafely(singularValue, culture) : this.GetStringSafely(pluralValue, culture);

            if (@string == null && this.Fallback != null)
            {
                @string = this.Fallback.M(info, singularValue, pluralValue, count);
            }

            if (@string != null)
            {
                return string.Format(culture, @string, realParams);
            }

            return string.Format(culture, count == 1 ? singularValue : pluralValue, realParams);
        }

        protected CultureInfo GetBestCulture(CultureInfo info)
        {
            CultureInfo culture = null;
            if (info != null)
            {
                if (sets.ContainsKey(info))
                    culture = info;
                else if (info.Parent != null && sets.ContainsKey(info.Parent))
                    culture = info.Parent;
            }
            var threadCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            if (culture == null) {
                if (sets.ContainsKey(threadCulture))
                    culture = threadCulture;
            }
            if (culture == null && threadCulture.Parent != null) {
                if (sets.ContainsKey(threadCulture.Parent))
                    culture = threadCulture.Parent;
            }
            if (culture == null)
            {
                culture = defaultCulture;
            }

            return culture;
        }

        private string GetStringSafely(string value, CultureInfo cultureInfo)
        {
            if (!this.sets.ContainsKey(cultureInfo))
            {
                ////Trace.TraceWarning("Strings: no set '" + cultureInfo.Name + "' for value '" + value + "'");
                return null;
            }

            var set = this.sets[cultureInfo];

            if (!set.ContainsKey(value))
                return null;

            var val = set[value];
            if (val == null)
                Trace.TraceWarning("Strings: no value '" + cultureInfo.Name + "' for value '" + value + "'");
            return val;
        }
    }
}
