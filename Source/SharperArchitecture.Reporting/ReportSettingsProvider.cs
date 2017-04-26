using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SimpleInjector;
using SimpleInjector.Extensions;

namespace SharperArchitecture.Common.Reporting
{
    public class ReportSettingsProvider : IReportSettingsProvider
    {
        private readonly Container _resolutionRoot;

        public ReportSettingsProvider(Container resolutionRoot)
        {
            _resolutionRoot = resolutionRoot;
        }

        public IReportSettings GetSettings(string reportType)
        {
            return _resolutionRoot.GetInstance<IReportSettings>(/*reportType*/);
        }
    }
}
