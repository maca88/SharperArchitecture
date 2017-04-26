using System.Collections.Generic;

namespace SharperArchitecture.Common.Reporting
{
    public class ReportType
    {
        public const string Pdf = "Pdf";
        public const string Xlsx = "Xlsx";
        public const string Html = "Html";
        public const string Csv = "Csv";

        public static IEnumerable<string> GetAll()
        {
            return new []{Pdf, Xlsx, Csv};
        } 
    }
}
