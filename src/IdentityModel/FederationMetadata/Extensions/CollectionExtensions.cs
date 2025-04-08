using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic
{
    internal static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            if (collection is List<T> list)
            {
                list.AddRange(range);
            }
            else
            {
                foreach (var item in range)
                    collection.Add(item);
            }
        }
    }
}
