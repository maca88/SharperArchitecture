using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class ListExtensions
    {
        public static IList<T> Update<T>(this IList<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
            return enumerable;
        }

        public static bool Replace<T>(this IList<T> enumerable, T itemToReplace, T newItem)
        {
            var idx = enumerable.IndexOf(itemToReplace);
            if (idx < 0) return false;
            enumerable[idx] = newItem;
            return true;
        }
    }
}
