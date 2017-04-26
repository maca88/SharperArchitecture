using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Common.Reporting
{
    public class ReportInfo
    {
        public ReportInfo()
        {
            Settings =new Dictionary<string, IReportSettings>();
        }

        public string TableId { get; set; }

        public string ReportName { get; set; }

        public Dictionary<string, IReportSettings> Settings { get; set; }

        public IReportSettings GetSettings(string reportType)
        {
            if (!Settings.ContainsKey(reportType)) return null;
            return Settings[reportType];
        }

        public T GetSettings<T>(string reportType) where T : class, IReportSettings, new()
        {
            if (!Settings.ContainsKey(reportType)) return new T();
            return Settings[reportType] as T ?? new T();
        }
    }
}
