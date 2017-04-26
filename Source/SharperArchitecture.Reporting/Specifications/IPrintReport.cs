using System.Collections.Generic;
using System.IO;
using SharperArchitecture.Common.Reporting;

namespace SharperArchitecture.Reporting.Specifications
{
    public interface IPrintReport
    {
        Stream Print<TItem>(PrintParameters<TItem> parameters) where TItem : class;

        string Extension { get; }

        string MimeType { get; }
    }
}