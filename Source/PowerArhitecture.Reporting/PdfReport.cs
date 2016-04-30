using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Reporting;
using PowerArhitecture.Reporting.Specifications;
using PowerArhitecture.Reporting.TextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace PowerArhitecture.Reporting
{
    public class PdfReport : BaseReport<PdfReportSettings>, IPrintReport
    {
        protected static BaseFont BaseFont;
        protected readonly XMLWorkerHelper XMLWorkerHelper;

        static PdfReport()
        {
            var fontPath = String.Format("{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
            BaseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        }

        public PdfReport()
        {
            XMLWorkerHelper = XMLWorkerHelper.GetInstance();
        }

        public override string Type { get { return ReportType.Pdf; } }

        public override string Extension { get { return "pdf"; } }

        public override Stream CreateReport<TItem>(Parameters<TItem> parameters)
        {
            var pageSize = Settings.PageOrientation == PageOrientation.Portrait ? PageSize.A4 : PageSize.A4.Rotate();
            using (var document = new Document(pageSize))
            using (var mStream = new MemoryStream())
            using (var writer = PdfWriter.GetInstance(document, mStream))
            {
                try
                {

                    Settings.Header.CustomContent.Content = "Headdeerer teext {DateTime}";
                    Settings.Header.CustomContent.Align = Alignment.Right;
                    Settings.Header.Paging.Show = true;
                    Settings.Header.Paging.Align = Alignment.Left;
                    Settings.Header.ShowSeparatorLine = true;

                    Settings.Footer.CustomContent.Content = "Foooteeer teext {DateTime}";
                    Settings.Footer.CustomContent.Align = Alignment.Left;
                    Settings.Footer.Paging.Show = true;
                    Settings.Footer.Paging.Align = Alignment.Right;
                    Settings.Footer.ShowSeparatorLine = true;

                    Settings.StartContentHtml = "<h1>Start content {DateTime}</h1>";
                    Settings.EndContentHtml = "<h1>End content {Date}</h1>";

                    var headerFooterEvent = new PdfHeaderFooterPageEvent<TItem>(parameters, Settings, BaseFont)
                    {
                        HeaderHeight = 40,
                        FooterHeight = 50
                    };
                    writer.PageEvent = headerFooterEvent;
                    headerFooterEvent.SetMargins(document);
                    document.Open();

                    var printParameters = parameters as PrintParameters<TItem>;
                    if (printParameters != null && !printParameters.Preview)
                    {
                        writer.AddJavaScript("var pp = this.getPrintParams();this.print(pp);");
                    }
                        
                    AddHtmlContent(document, writer, parameters, Settings.StartContentHtml);

                    CreateAndFillTable(document, parameters.Items, parameters.ItemProperties);

                    AddHtmlContent(document, writer, parameters, Settings.EndContentHtml);
                }
                catch (DocumentException)
                {
                    throw;
                    //handle pdf document exception if any
                }
                catch (IOException)
                {
                    throw;
                    // handle IO exception
                }
                catch (Exception)
                {
                    throw;
                    // handle other exception if occurs
                }
                document.Close();
                writer.Flush();
                return mStream;
            }
        }

        protected void AddHtmlContent<TItem>(Document document, PdfWriter writer, Parameters<TItem> parameters, string html, Stream cssStream = null)
        {
            if (string.IsNullOrEmpty(html)) return;
            html = ReportShortCodes.Process(parameters, html);

            using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(html)))
            {
                XMLWorkerHelper.ParseXHtml(writer, document, mStream, cssStream);
            }
        }

        public Stream Print<TItem>(PrintParameters<TItem> parameters) 
            where TItem : class
        {
            Settings = parameters.Settings as PdfReportSettings;
            return CreateReport(parameters);
        }

        public override string MimeType
        {
            get { return "application/pdf"; }
        }

        protected Font GetFont(int type)
        {
            return new Font(BaseFont, Settings.FontSize, type, BaseColor.BLACK);
        }

        protected void CreateAndFillTable<T>(Document document, IEnumerable<T> items, IEnumerable<ReportItemProperty> itemProperties = null) where T : class
        {
            var type = typeof(T);
            var itemPropertiesCol = GetItemProperties(type, itemProperties);

            var table = new PdfPTable(itemPropertiesCol.Count)
                {
                    WidthPercentage = 100
                };
            if (Settings.RepeatTableHeader)
                table.HeaderRows = 1;
            var tableHeaderFont = GetFont(Font.BOLD);
            //Add headers
            foreach (var itemProperty in itemPropertiesCol)
            {
                table.AddCell(new PdfPCell(new Phrase(itemProperty.HeaderName, tableHeaderFont)));
            }

            var tableRowFont = GetFont(Font.NORMAL);
            
            var props = itemPropertiesCol.Select(o => type.GetProperty(o.PropertyName)).ToList();
            
            foreach (var item in items)
            {
                foreach (var prop in props)
                {
                    var value = prop.GetValue(item, null) ?? "";
                    table.AddCell(new Paragraph(value.ToString(), tableRowFont));
                }
            }
            document.Add(table);
        }
    }
}
