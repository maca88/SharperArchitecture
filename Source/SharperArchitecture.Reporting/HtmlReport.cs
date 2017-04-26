using System;
using System.IO;
using System.Linq;
using System.Reflection;
using SharperArchitecture.Common.Reporting;
using SharperArchitecture.Reporting.Templates;
using RazorEngine;

namespace SharperArchitecture.Reporting
{
    public class HtmlReport : BaseReport<HtmlReportSettings>
    {
        static HtmlReport()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SharperArchitecture.Reporting.Templates.TableReport.cshtml"))
            using (var reader = new StreamReader(stream))
            {
                var template = reader.ReadToEnd();
                template = string.Join(Environment.NewLine, template.Split(Environment.NewLine.ToCharArray()).Skip(1));
                Razor.Compile(template, typeof(TableReportViewModel), "TableReport");
            }
        }

        public override string Extension
        {
            get { return "html"; }
        }

        public override string MimeType
        {
            get { return "text/html"; }
        }

        public override Stream CreateReport<TItem>(Parameters<TItem> parameters)
        {
            var type = typeof(TItem);
            var itemPropertiesCol = GetItemProperties(type, parameters.ItemProperties);
            var props = itemPropertiesCol.Select(o => type.GetProperty(o.PropertyName)).ToList();

            var viewModel = new TableReportViewModel
            {
                Headers = itemPropertiesCol.Select(o => o.HeaderName).ToList(),
                Properties = props,
                Items = parameters.Items,
                Settings = Settings
            };
            using (var stream = Razor.Run("TableReport", viewModel).ToStream())
            {
                return stream;
            }
        }

        public override string Type
        {
            get { return ReportType.Html; }
        }
    }
}
