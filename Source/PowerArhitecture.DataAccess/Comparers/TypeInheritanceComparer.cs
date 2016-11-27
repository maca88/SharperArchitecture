using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.DataAccess.Comparers
{
    internal class TypeInheritanceComparer : IComparer<Type>
    {
        public int Compare(Type x, Type y)
        {
            if (x == y)
            {
                return 0;
            }
            if (x.IsAssignableToGenericType(y))
            {
                return 1;
            }
            return -1;
        }
    }
}
