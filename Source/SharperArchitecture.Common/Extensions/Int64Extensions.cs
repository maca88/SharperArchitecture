using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Common.Extensions
{
    public static class Int64Extensions
    {
        /// <summary>
        /// Treat long as timestamp
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long l)
        {
            return new DateTime(1970, 01, 01).AddMilliseconds(l);
        }
    }
}
