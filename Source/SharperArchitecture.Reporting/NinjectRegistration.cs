using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Common.Reporting;
using SharperArchitecture.Reporting.Specifications;
using Ninject.Modules;

namespace SharperArchitecture.Reporting
{
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            Bind<IReporter>().To<Reporter>().InSingletonScope();
            Bind<IReportSettingsProvider>().To<ReportSettingsProvider>().InSingletonScope();
            Bind<IPrintReport>().To<PdfReport>();
            Bind<IReport>().To<PdfReport>().Named(ReportType.Pdf);
            Bind<IReport>().To<XlsxReport>().Named(ReportType.Xlsx);
            Bind<IReport>().To<CsvReport>().Named(ReportType.Csv);
            Bind<IReport>().To<HtmlReport>().Named(ReportType.Html);

            Bind<IReportSettings>().To<CsvReportSettings>().Named(ReportType.Csv);
            Bind<IReportSettings>().To<HtmlReportSettings>().Named(ReportType.Html);
            Bind<IReportSettings>().To<PdfReportSettings>().Named(ReportType.Pdf);
            Bind<IReportSettings>().To<XlsxReportSettings>().Named(ReportType.Xlsx);
        }
    }
}
