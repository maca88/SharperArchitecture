using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Common.Reporting
{
    public class HtmlReportSettings : IReportSettings
    {
        public HtmlReportSettings()
        {
            FontSize = ReportSettings.DefaultFontSize;
        }

        public int FontSize { get; set; }
    }
}
