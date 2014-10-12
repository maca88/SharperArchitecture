using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Globalization
{
    public static class DateTimeFormatInfoExtensions
    {
        public static string GetDefaultFormat(this DateTimeFormatInfo dateTimeFormatInfo)
        {
            return string.Format("{0} {1}", dateTimeFormatInfo.ShortDatePattern, dateTimeFormatInfo.LongTimePattern);
        }
    }
}
