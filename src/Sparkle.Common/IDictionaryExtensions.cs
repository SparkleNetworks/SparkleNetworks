
namespace System.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Extensions for the IDictionary type.
    /// </summary>
    public static class IDictionaryExtensions
    {
        public static IDictionary<TKey, TValue> AddFluent<TKey, TValue>(this IDictionary<TKey, TValue> collection, TKey key, TValue value)
        {
            collection.Add(key, value);
            return collection;
        }

        public static IDictionary<TKey, TValue> SetFluent<TKey, TValue>(this IDictionary<TKey, TValue> collection, TKey key, TValue value)
        {
            collection[key] = value;
            return collection;
        }

        public static void FromHttpHeaderText<TKey, TValue>(this IDictionary<TKey, TValue> collection, string data)
        {
            var lines = data.Split(new string[] { "\r\n", }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var pos = line.IndexOf(':');
                if (pos > 0)
                {
                    var key = line.Substring(0, pos);
                    var value = line.Substring(pos + 1);
                    collection.Add(
                        (TKey)Convert.ChangeType(key, typeof(TKey)),
                        (TValue)Convert.ChangeType(value, typeof(TValue)));
                }
                else
                {
                    throw new InvalidOperationException("HttpHeaderText is not valid: line " + i + " has no colon delimiter");
                }
            }
        }

        public static string ToHttpHeaderText<TKey, TValue>(this IDictionary<TKey, TValue> collection)
        {
            var sb = new StringBuilder();
            foreach(var key in collection.Keys)
            {
                sb.Append(key);
                sb.Append(":");
                sb.AppendLine(collection[key].ToString());
            }

            return sb.ToString();
        }

        public static string ToQueryString<TKey, TValue>(this IDictionary<TKey, TValue> collection)
        {
            var sb = new StringBuilder();
            var sep = "";
            foreach(var key in collection.Keys)
            {
                sb.Append(sep);
                sb.Append(Uri.EscapeDataString(key.ToString()));
                sb.Append("=");
                if (collection[key] != null)
                    sb.Append(Uri.EscapeDataString(collection[key].ToString()));
                sep = "&";
            }

            return sb.ToString();
        }
    }
}
