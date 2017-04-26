using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Common.Reporting
{
    public class PdfReportSettings : IReportSettings
    {
        public PdfReportSettings()
        {
            PageOrientation = PageOrientation.Portrait;
            Header = new HeaderFooterSettings();
            Footer = new HeaderFooterSettings();
            ShowWatermark = true;
            RepeatTableHeader = true;
            FontSize = ReportSettings.DefaultFontSize;
        }

        public int FontSize { get; set; }

        public PageOrientation PageOrientation { get; set; }

        public string StartContentHtml { get; set; }

        public string EndContentHtml { get; set; }

        public bool ShowWatermark { get; set; }

        public bool RepeatTableHeader { get; set; }

        public HeaderFooterSettings Header { get; set; }

        public HeaderFooterSettings Footer { get; set; }
    }
}
