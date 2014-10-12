using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Reporting
{
    public class ReportInfo
    {
        public string TemplatePath { get; set; }

        public string LanguageCode { get; set; }

        public bool Landscape { get; set; }

        public int FontSize { get; set; }
    }
}
