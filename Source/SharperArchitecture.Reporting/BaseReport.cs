using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SharperArchitecture.Common.Reporting;
using SharperArchitecture.Reporting.Specifications;

namespace SharperArchitecture.Reporting
{
    public abstract class BaseReport<TSettings> : IReport
        where TSettings : class, IReportSettings, new()
    {
        protected BaseReport()
        {
            Settings = new TSettings();
        }

        protected TSettings Settings { get; set; }

        public abstract string MimeType { get; }

        public Stream Create<TItem>(ReportParameters<TItem> parameters) where TItem : class
        {
            Settings = parameters.Settings as TSettings;
            return CreateReport(parameters);
        }

        public abstract Stream CreateReport<TItem>(Parameters<TItem> parameters) where TItem : class;
        
        public abstract string Type { get; }
        public abstract string Extension { get; }

        protected ICollection<ReportItemProperty> GetItemProperties(Type type, IEnumerable<ReportItemProperty> itemProperties = null)
        {
            var itemPropertiesCol = itemProperties as ICollection<ReportItemProperty> ?? (itemProperties != null ? itemProperties.ToList() : null);

            if (itemPropertiesCol == null || !itemPropertiesCol.Any())
                itemPropertiesCol = type.GetProperties()
                    .Select(o => new ReportItemProperty
                    {
                        HeaderName = o.Name,
                        PropertyName = o.Name
                    })
                    .ToList();
            return itemPropertiesCol;
        }

    }
}
