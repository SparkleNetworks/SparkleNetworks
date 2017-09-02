
namespace Sparkle.UI
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Diagnostics.CodeAnalysis;
    using System;

    /// <summary>
    /// Quick sparkle language tools accessor.
    /// Prefer using this.Services.Lang instead of these static methods.
    /// </summary>
    public sealed class Lang
    {
        private Lang()
        {
        }

        public static IList<CultureInfo> AvailableCultures { get; set; }

        /// <summary>
        /// Gets or sets the source sitnrgs.
        /// </summary>
        /// <value>
        /// The source sitnrgs.
        /// </value>
        public static Strings Source { get; set; }

        /// <summary>
        /// Returns a translated string.
        /// If the translation is not for the specified culture, it will be searched in the other available cultures.
        /// If the translation does not exists, the specified value is directly returned.
        /// </summary>
        /// <param name="value">The sentence to translate.</param>
        /// <returns>The translated sentence</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "Sparkle.UI.Strings.T(System.String)")]
        public static string T(string value)
        {
            var source = Source;
            if (source == null)
                throw new InvalidOperationException("Lang.Source is not set");
            return source.T(value);
        }

        /// <summary>
        /// Returns a translated formatted string.
        /// If the translation is not for the specified culture, it will be searched in the other available cultures.
        /// If the translation does not exists, the specified value is directly returned.
        /// </summary>
        /// <param name="value">The sentence to translate.</param>
        /// <returns>The translated sentence</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "Sparkle.UI.Strings.T(System.String,System.Object[])")]
        public static string T(string value, params object[] parameters)
        {
            var source = Source;
            if (source == null)
                throw new InvalidOperationException("Lang.Source is not set");
            return source.T(value, parameters);
        }

        public static string T(CultureInfo lang, string value)
        {
            var source = Source;
            if (source == null)
                throw new InvalidOperationException("Lang.Source is not set");
            return source.T(lang, value);
        }

        /// <summary>
        /// Returns a translated string.
        /// If the translation is not for the specified culture, it will be searched in the other available cultures.
        /// If the translation does not exists, the specified value is directly returned.
        /// </summary>
        /// <param name="value">The sentence to translate.</param>
        /// <returns>The translated sentence</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "Sparkle.UI.Strings.M(System.String,System.String,System.Int32)")]
        public static string M(string singularValue, string pluralValue, int count)
        {
            var source = Source;
            if (source == null)
                throw new InvalidOperationException("Lang.Source is not set");
            return source.M(singularValue, pluralValue, count);
        }
    }
}
