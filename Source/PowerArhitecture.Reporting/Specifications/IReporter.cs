using System.Collections.Generic;
using System.IO;
using PowerArhitecture.Common.Reporting;

namespace PowerArhitecture.Reporting.Specifications
{
    public interface IReporter
    {
        ReportResult CreateReport<TItem>(ReportParameters<TItem> parameters)
            where TItem : class;

        ReportResult Print<TItem>(PrintParameters<TItem> parameters)
            where TItem : class;
    }
}