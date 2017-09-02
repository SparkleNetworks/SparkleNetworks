
namespace System.Collections.Generic
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extensions for the ObservableCollection type.
    /// </summary>
    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// Adds the elements of the specified collection to the end of the collection.
        /// </summary>
        /// <typeparam name="T">the type of item for the collection</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="items">The items.</param>
        public static void AddRange<T>(this IList<T> source, IEnumerable<T> items)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (items == null)
                throw new ArgumentNullException("items");

            foreach (var item in items)
            {
                source.Add(item);
            }
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the collection with an item transformation.
        /// </summary>
        /// <typeparam name="TSource">destination type</typeparam>
        /// <typeparam name="TTransform">item type to be transform into a T</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="items">The items.</param>
        /// <param name="selector">The type transformation.</param>
        public static void AddRange<TSource, TTransform>(this IList<TSource> source, IEnumerable<TTransform> items, Func<TTransform, TSource> selector)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (items == null)
                throw new ArgumentNullException("items");
            if (selector == null)
                throw new ArgumentNullException("selector");

            foreach (var item in items)
            {
                source.Add(selector(item));
            }
        }
    }
}
