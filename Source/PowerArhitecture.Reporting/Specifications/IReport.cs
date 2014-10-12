﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using PowerArhitecture.Common.Reporting;

namespace PowerArhitecture.Reporting.Specifications
{
    public interface IReport<TIn> : IReport
    {
        void FillReport(string finalReportPath, ReportInfo reportInfo, TIn parameter);
    }

    public interface IReport
    {
        string Type { get; }

        string Extension { get; }

        string MimeType { get; }

        Stream Create<TItem>(ReportParameters<TItem> parameters) where TItem : class;
    }
}