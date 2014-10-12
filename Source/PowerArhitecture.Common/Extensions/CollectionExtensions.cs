using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class CollectionExtensions
    {
        public static void AddIfNotExist<T>(this ICollection<T> collection, T item)
        {
            if (!collection.Contains(item))
                collection.Add(item);
        }

        public static void RemoveIfExist<T>(this ICollection<T> collection, T item)
        {
            if (collection.Contains(item))
                collection.Remove(item);
        }
    }
}
