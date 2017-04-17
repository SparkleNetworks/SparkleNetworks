
#if SSC
namespace SparkleSystems.Configuration.Internals
#else
namespace System.Collections.Generic
#endif
{
    using System;
    using System.Linq;
#if SSC
    using System.Collections.Generic;
#endif

    /// <summary>
    /// Extension methods for arrays.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Compares 2 array.
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="first">the first array</param>
        /// <param name="second">the second array</param>
        /// <returns>a boolean</returns>
        public static bool ArraySequenceEquals<T>(this T[] first, T[] second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (first == null || second == null)
            {
                return false;
            }

            if (first.Length != second.Length)
            {
                return false;
            }

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < first.Length; i++)
            {
                if (!comparer.Equals(first[i], second[i]))
                { 
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a string from an array of bytes using a hexadecimal form.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>a string from an array of bytes using a hexadecimal form</returns>
        public static string ToHexadecimalForm(this IEnumerable<byte> source)
        {
            return string.Concat(Array.ConvertAll(source.ToArray(), x => x.ToString("X2")));
        }
    }
}
