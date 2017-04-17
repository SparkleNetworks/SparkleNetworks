
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Infrastructure.Data;

    /// <summary>
    /// Auto-generated configuration tree.
    /// </summary>
    partial class SparkleNetworksApplicationConfigurationTree
    {
        /// <summary>
        /// Gets a value with the specified key (fallbacks to the default value) and converted to the specified type.
        /// </summary>
        /// <typeparam name="T">The desired type</typeparam>
        /// <param name="values">The values source.</param>
        /// <param name="key">The key.</param>
        /// <returns>the value, the default value or null</returns>
        internal static T GetValue<T>(IDictionary<string, AppConfigurationEntry> values, string key)
        {
            if (values.ContainsKey(key))
            {
                var entry = values[key];
                var val = entry.RawValue;
                if (val != null)
                    return (T)Convert.ChangeType(val, typeof(T));
                else if (entry.DefaultRawValue != null)
                    return (T)Convert.ChangeType(entry.DefaultRawValue, typeof(T));
                return default(T);
            }

            return default(T);
        }

        /// <summary>
        /// Gets a value with the specified key (fallbacks to the default value) and converted to the specified type.
        /// </summary>
        /// <typeparam name="T">The desired type</typeparam>
        /// <param name="values">The values source.</param>
        /// <param name="key">The key.</param>
        /// <returns>the value, the default value or null</returns>
        internal static Nullable<T> GetNullableValue<T>(IDictionary<string, AppConfigurationEntry> values, string key)
            where T : struct
        {
            if (values.ContainsKey(key))
            {
                var entry = values[key];
                var val = entry.RawValue;
                if (val != null)
                    return (T?)Convert.ChangeType(val, typeof(T));
                else if (entry.DefaultRawValue != null)
                    return (T?)Convert.ChangeType(entry.DefaultRawValue, typeof(T));
                return default(T?);
            }

            return default(T?);
        }

        /// <summary>
        /// Gets a values with the specified key (no fallback to the default value) and converted to the specified type.
        /// </summary>
        /// <typeparam name="T">The desired type</typeparam>
        /// <param name="values">The values source.</param>
        /// <param name="key">The key.</param>
        /// <returns>the values</returns>
        internal static IList<T> GetValues<T>(IDictionary<string, AppConfigurationEntry> values, string key)
        {
            if (values.ContainsKey(key))
            {
                var entry = values[key];
                var vals = entry.RawValues;
                if (vals != null)
                    return vals.Where(x => x != null).Select(x => (T)Convert.ChangeType(x, typeof(T))).ToList();
                return new List<T>();
            }

            return new List<T>();
        }
    }
}
