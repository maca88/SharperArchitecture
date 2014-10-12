using System.Collections.Generic;
using System.IO;
using PowerArhitecture.Common.Reporting;

namespace PowerArhitecture.Reporting.Specifications
{
    public interface IPrintReport
    {
        Stream Print<TItem>(PrintParameters<TItem> parameters) where TItem : class;

        string Extension { get; }

        string MimeType { get; }
    }
}