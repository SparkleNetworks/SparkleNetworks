
namespace Sparkle.Commands.Main.Import
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class ImportedIdHelper
    {
        /// <summary>
        /// Parse a ImportedId into a list.
        /// </summary>
        /// <remarks>
        /// Example field: ;OldId=123456;Company=Stuff SAS;NoPassword;
        /// </remarks>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            var parts = value.Split(new char[] { ';', }, StringSplitOptions.RemoveEmptyEntries);
            var list = new List<KeyValuePair<string, string>>(parts.Length);
            foreach (var item in parts)
            {
                var equalSign = item.IndexOf('=');
                if (equalSign > 0)
                {
                    list.Add(new KeyValuePair<string, string>(item.Substring(0, equalSign), item.Substring(equalSign + 1)));
                }
                else
                {
                    list.Add(new KeyValuePair<string, string>(item, string.Empty));
                }
            }

            return list;
        }

        /// <summary>
        /// Creates an ImportedId from a list of values.
        /// </summary>
        /// <remarks>
        /// Example field: ;OldId=123456;Company=Stuff SAS;NoPassword;
        /// </remarks>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Compute(IEnumerable<KeyValuePair<string, string>> values)
        {
            if (values == null)
                return null;

            var sb = new StringBuilder();
            sb.Append(';');
            int count = 0;
            foreach (var item in values)
            {
                count++;
                sb.Append(item.Key);
                sb.Append('=');
                sb.Append(item.Value);
                sb.Append(';');
            }

            if (count > 0)
            {
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
    }
}
