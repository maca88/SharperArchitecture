using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Common.Reporting
{
    public class ReportShortCodes
    {
        private static readonly Dictionary<string, Func<Parameters, string>> ShortCodeProcessors;

        static ReportShortCodes()
        {
            ShortCodeProcessors = new Dictionary<string, Func<Parameters, string>>
            {
                {"{ReportName}", settings => settings.ReportName},
                {"{Date}", settings => DateTime.Now.Date.ToShortDateString()},
                {"{DateTime}", settings => DateTime.Now.ToString(CultureInfo.CurrentUICulture)},
            };
        }

        public static string Process<TItem>(Parameters<TItem> reportParamters, string content)
        {
            return string.IsNullOrEmpty(content)
                ? content
                : ShortCodeProcessors.Aggregate(content, (current, pair) => current.Replace(pair.Key, pair.Value(reportParamters)));
        }
    }
}
