using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Common.Reporting;

namespace SharperArchitecture.Reporting.Templates
{
    public class TableReportViewModel
    {
        public TableReportViewModel()
        {
            Headers = new List<string>();
            Properties = new List<PropertyInfo>();
        }

        public List<string> Headers { get; set; }

        public List<PropertyInfo> Properties { get; set; }

        public IEnumerable Items { get; set; }

        public HtmlReportSettings Settings { get; set; }
    }
}
