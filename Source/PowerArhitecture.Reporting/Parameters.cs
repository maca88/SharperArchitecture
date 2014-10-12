using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Common.Reporting
{
    public class Parameters
    {
        public string Culture { get; set; }

        public IReportSettings Settings { get; set; }

        public string ReportName { get; set; }

        public IEnumerable<ReportItemProperty> ItemProperties { get; set; }
    }

    public class Parameters<T> : Parameters
    {
        public IEnumerable<T> Items { get; set; }
    }
}
