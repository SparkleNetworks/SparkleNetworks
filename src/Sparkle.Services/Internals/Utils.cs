
namespace Sparkle.Services.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class Utils
    {
        /// <summary>
        ///   Converts an array of 8-bit unsigned integers to its equivalent string representation
        ///   that is encoded with base-64 digits.
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static string ToUrlBase64(this byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var chars = Convert.ToBase64String(data).Replace("-", string.Empty).ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '+')
                {
                    chars[i] = '-';
                }
                else if (chars[i] == '/')
                {
                    chars[i] = '_';
                }
                else if (chars[i] == '=')
                {
                    chars[i] = ',';
                }
            }

            return new string(chars);
        }

        /// <summary>
        ///   Converts a string, which encodes binary data
        ///   as base-64 digits, to an equivalent 8-bit unsigned integer array.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <returns></returns>
        public static byte[] FromUrlBase64(this string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var chars = value.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '-')
                {
                    chars[i] = '+';
                }
                else if (value[i] == '_')
                {
                    chars[i] = '/';
                }
                else if (value[i] == ',')
                {
                    chars[i] = '=';
                }
            }

            var data = Convert.FromBase64CharArray(chars, 0, chars.Length);

            return data;
        }
    }
}
