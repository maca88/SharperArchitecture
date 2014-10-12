using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Ninject;
using Ninject.Syntax;

namespace PowerArhitecture.Common.Reporting
{
    public class ReportSettingsProvider : IReportSettingsProvider
    {
        private readonly IResolutionRoot _resolutionRoot;

        public ReportSettingsProvider(IResolutionRoot resolutionRoot)
        {
            _resolutionRoot = resolutionRoot;
        }

        public IReportSettings GetSettings(string reportType)
        {
            return _resolutionRoot.Get<IReportSettings>(reportType);
        }
    }
}
