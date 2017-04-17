
namespace System.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class IEnumerableExtensions
    {
        private static readonly Random random = new Random();

        public static IEnumerable<T> ForEach<T>(IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
                yield return item;
            }
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            // copied from http://stackoverflow.com/a/1287572
            T[] elements = source.ToArray();
            for (int i = elements.Length - 1; i >= 0; i--)
            {
                // Swap element "i" with a random earlier element it (or itself)
                // ... except we don't really need to swap it fully, as we can
                // return it immediately, and afterwards it's irrelevant.
                int swapIndex = random.Next(i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }
        }

        public static IList<T> InsertOrUpdate<T>(this IList<T> collection, T item, Func<T, bool> find, Action<T> update)
        {
            return collection.InsertOrUpdate(item, find, (l,i) => l.Add(i), update);
        }

        public static IList<T> InsertOrUpdate<T>(this IList<T> collection, T item, Func<T, bool> find, Action<IList<T>, T> insert, Action<T> update)
        {
            var exists = collection.SingleOrDefault(find);
            if (exists != null)
            {
                update(exists);
            }
            else
            {
                insert(collection, item);
            }

            return collection;
        }
    }
}
