using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<IGrouping<TKey, TSource>> Duplicates<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.GroupBy(keySelector).Where(o => o.Count() > 1);
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                action(item);
            }
        }
    }
}
