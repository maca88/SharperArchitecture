using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Common.Reporting
{
    public class XlsxReportSettings : IReportSettings
    {
        public XlsxReportSettings()
        {
            FontSize = ReportSettings.DefaultFontSize;
            PageOrientation = PageOrientation.Portrait;
            AutoFitColumns = true;
        }

        public int FontSize { get; set; }

        public PageOrientation PageOrientation { get; set; }

        public XlsxTableStyle TableStyle { get; set; }

        public bool AutoFitColumns { get; set; }
    }
}
