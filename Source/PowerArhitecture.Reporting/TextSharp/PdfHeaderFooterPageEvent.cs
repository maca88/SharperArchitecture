using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using PowerArhitecture.Common.Reporting;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Font = iTextSharp.text.Font;
using Image = iTextSharp.text.Image;
using Rectangle = iTextSharp.text.Rectangle;

namespace PowerArhitecture.Reporting.TextSharp
{
    public class PdfHeaderFooterPageEvent<TItem> : PdfPageEventHelper
    {
        PdfTemplate _total;
        private readonly BaseFont _baseFont;
        private int _headerOffset = 20;
        private int _footerOffset = 20;

        protected Parameters<TItem> ReportParameters { get; set; }

        protected PdfReportSettings ReportSettings { get; set; }

        public PdfHeaderFooterPageEvent(Parameters<TItem> parameters, PdfReportSettings settings, BaseFont baseFont)
        {
            ReportParameters = parameters;
            ReportSettings = settings;
            _baseFont = baseFont;
        }

        public int FooterHeight { get; set; }

        public int HeaderHeight { get; set; }

        public void SetMargins(Document document)
        {
            document.SetMargins(document.LeftMargin, document.RightMargin, HeaderHeight + _headerOffset, FooterHeight + _footerOffset);
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            _total = writer.DirectContent.CreateTemplate(15, ReportSettings.FontSize);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            CreateHeaderFooter(writer, document, false, ReportSettings.Footer, FooterHeight);
            CreateHeaderFooter(writer, document, true, ReportSettings.Header, 15);
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            SetMargins(document);
        }

        private void CreateHeaderFooter(PdfWriter writer, Document document, bool header, HeaderFooterSettings settings, float margin)
        {
            var border = header ? Rectangle.BOTTOM_BORDER : Rectangle.TOP_BORDER;
            if (!settings.ShowSeparatorLine)
                border = Rectangle.NO_BORDER;
            
            var contentAlign = new Dictionary<Alignment, List<PdfPCell>>();
            if (settings.Paging.Show)
            {
                var align = settings.Paging.Align;
                if(!contentAlign.ContainsKey(align))
                    contentAlign.Add(align, new List<PdfPCell>());
                contentAlign[align].Add(CreatePaging(writer, border, align));
            }
            if (!string.IsNullOrEmpty(settings.CustomContent.Content))
            {
                var align = settings.CustomContent.Align;
                if(!contentAlign.ContainsKey(align))
                    contentAlign.Add(align, new List<PdfPCell>());
                contentAlign[align].Add(CreateCustomContent(settings.CustomContent, border, align));
            }
            
            var widths = new List<int>(contentAlign.Count);
            widths.AddRange(contentAlign.Select(width => 20));
            var watermarkBorder = false;
            if (!widths.Any())
            {
                if(!ReportSettings.ShowWatermark) return;
                widths.Add(20);
                if(!header)
                    margin -= 20;
                watermarkBorder = true;
            }

            if(contentAlign.ContainsKey(Alignment.Center) && widths.Count % 2 == 0)
                widths.Add(20);

            var table = new PdfPTable(widths.Count);
            table.SetWidths(widths.ToArray());
            table.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            table.DefaultCell.Border = border;
            table.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            
            if(header)
                AddWatermark(table, true, watermarkBorder);

            AddHeaderFooterContent(Alignment.Left, table, contentAlign);
            AddHeaderFooterContent(Alignment.Center, table, contentAlign);
            AddHeaderFooterContent(Alignment.Right, table, contentAlign);
            table.CompleteRow();

            if (!header)
                AddWatermark(table, false, watermarkBorder);

            var yPos = header ? document.PageSize.GetTop(margin) : document.PageSize.GetBottom(margin);
            table.WriteSelectedRows(0, -1, document.LeftMargin, yPos, writer.DirectContent);
        }


        private void AddWatermark(PdfPTable table, bool header, bool border)
        {
            if(!ReportSettings.ShowWatermark) return;

            var watermarkCell = new PdfPCell(new Phrase("Watermark 2014", new Font(_baseFont, ReportSettings.FontSize, Font.NORMAL, new BaseColor(Color.DarkGray))))
            {
                Colspan = table.NumberOfColumns,
                Rowspan = 1,
                Border = border 
                    ? (header ? Rectangle.BOTTOM_BORDER : Rectangle.TOP_BORDER)
                    : Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            table.AddCell(watermarkCell);
        }

        private void AddHeaderFooterContent(Alignment alignment, PdfPTable table, Dictionary<Alignment, List<PdfPCell>> contentAlign)
        {
            if (!contentAlign.ContainsKey(alignment) || !contentAlign[alignment].Any()) return;
            if (contentAlign[alignment].Count > 1)
            {
                var nestedTable = new PdfPTable(contentAlign[alignment].Count);
                foreach (var ntable in contentAlign[alignment])
                {
                    nestedTable. AddCell(ntable);
                }
                table.AddCell(nestedTable);
            }
            else
            {
                table.AddCell(contentAlign[alignment][0]);
            }
        }

        private PdfPCell CreateCustomContent(CustomContent customContent, int border, Alignment alignment)
        {
            var cell = new PdfPCell(new Phrase(ReportShortCodes.Process(ReportParameters, customContent.Content)))
            {
                Border = border,
                HorizontalAlignment = GetElementAlignment(alignment)
            };
            return cell;
        }

        private int GetElementAlignment(Alignment alignment)
        {
            switch (alignment)
            {
                case Alignment.Left:
                    return Element.ALIGN_LEFT;
                case Alignment.Center:
                    return Element.ALIGN_CENTER;
                case Alignment.Right:
                    return Element.ALIGN_RIGHT;
                default:
                    throw new InvalidEnumArgumentException("alignment");
            }
        }

        private PdfPCell CreatePaging(PdfWriter writer, int border, Alignment alignment)
        {
            var totalImage = Image.GetInstance(_total);
            var text = new Phrase(new Chunk(String.Format("Page {0} of ", writer.PageNumber)));
            text.Add(new Chunk(totalImage, 0, 0));
            var cell = new PdfPCell(text)
            {
                Border = border,
                HorizontalAlignment = GetElementAlignment(alignment)
            };
            return cell;
        }
        
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            ColumnText.ShowTextAligned(_total, Element.ALIGN_LEFT, new Phrase((writer.PageNumber - 1).ToString(CultureInfo.InvariantCulture), 
                new Font(_baseFont, ReportSettings.FontSize)), 0, 0, 0);
        }
    } 
}
