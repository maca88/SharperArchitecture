using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Common.Reporting
{
    public class HeaderFooterSettings
    {
        public HeaderFooterSettings()
        {
            Paging = new PagingSettings();
            CustomContent = new CustomContent();
        }

        public bool ShowSeparatorLine { get; set; }

        public PagingSettings Paging { get; set; }

        public CustomContent CustomContent { get; set; }
    }
}
