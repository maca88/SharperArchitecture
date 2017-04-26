using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Common.Reporting;
using SharperArchitecture.Reporting.Extensions;
using SharperArchitecture.Reporting.Specifications;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;

namespace SharperArchitecture.Reporting
{
    public class XlsxReport : BaseReport<XlsxReportSettings>
    {
        public override Stream CreateReport<TItem>(Parameters<TItem> parameters)
        {
            using (var mStream = new MemoryStream())
            using (var document = new SLDocument())
            {
                document.SetPageSettings(new SLPageSettings
                {
                    Orientation = Settings.PageOrientation == PageOrientation.Landscape
                        ? OrientationValues.Landscape
                        : OrientationValues.Portrait
                });
                CreateAndFillTable(document, null, parameters.Items, parameters.ItemProperties, null);
                document.SaveAs(mStream);
                return mStream;
            }
        }

        public override string Type { get { return ReportType.Xlsx; } }

        public override string Extension { get { return "xlsx"; } }

        public override string MimeType
        {
            get { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }
        }

        protected void CreateAndFillTable<T>(SLDocument document, string sheetName, IEnumerable<T> items, string tableStart, SLTableStyleTypeValues tableStyle) where T : class
        {
            CreateAndFillTable(document, sheetName, items, null, tableStart);
        }

        protected void CreateAndFillTable<T>(SLDocument document, string sheetName, IEnumerable<T> items, IEnumerable<ReportItemProperty> itemProperties, string tableStart) where T : class
        {
            var type = typeof(T);
            var itemPropertiesCol = GetItemProperties(type, itemProperties);

            if(!string.IsNullOrEmpty(sheetName))
                document.SelectWorksheet(sheetName);

            if (string.IsNullOrEmpty(sheetName))
                tableStart = "A1";

            var rowIdx = tableStart.GetCellRowIndex() + 1;
            var colOffset = tableStart.GetExcelColumn().GetExcelColumnNumber() + 1;

            //Styling for cells
            var style = document.CreateStyle();
            style.Font.FontSize = Settings.FontSize;

            var colIdx = colOffset;
            foreach (var itemProp in itemPropertiesCol)
            {
                SetCellValue(document, rowIdx, colIdx, itemProp.HeaderName);
                document.SetCellStyle(rowIdx, colIdx, style);
                colIdx++;
            }
            rowIdx++;

            var props = itemPropertiesCol.Select(o => type.GetProperty(o.PropertyName)).ToList();
            foreach (var obj in items)
            {
                colIdx = colOffset;
                foreach (var prop in props)
                {
                    SetCellValue(document, rowIdx, colIdx, prop.GetValue(obj, null));
                    document.SetCellStyle(rowIdx, colIdx, style);
                    colIdx++;
                }
                rowIdx++;
            }

            colIdx = colOffset;
            if (Settings.AutoFitColumns)
            {
                for (var i = 0; i < itemPropertiesCol.Count; i++)
                {
                    document.AutoFitColumn(i + colIdx);
                }
            }
            
            var endCellRef = string.Format("{0}{1}", tableStart.GetExcelColumn(itemPropertiesCol.Count - 1), (rowIdx - 1));
            var tbl = document.CreateTable(tableStart, endCellRef);
            SLTableStyleTypeValues tableStyle;
            if(Enum.TryParse(Settings.TableStyle.ToString(), true, out tableStyle))
                tbl.SetTableStyle(tableStyle);
            document.InsertTable(tbl);
        }

        private static bool SetCellValue(SLDocument document, int rowIdx, int colIdx, object value)
        {
            if (value == null) return false;
            var str = value as string;
            if (str != null)
            {
                document.SetCellValue(rowIdx, colIdx, str);
                return true;
            }
            var num = value as int?;
            if (num.HasValue)
            {
                document.SetCellValue(rowIdx, colIdx, num.Value);
                return true;
            }
            var dec = value as decimal?;
            if (dec.HasValue)
            {
                document.SetCellValue(rowIdx, colIdx, dec.Value);
                return true;
            }
            var date = value as DateTime?;
            if (date.HasValue)
            {
                document.SetCellValue(rowIdx, colIdx, date.Value);
                return true;
            }
            var boolean = value as bool?;
            if (boolean.HasValue)
            {
                document.SetCellValue(rowIdx, colIdx, boolean.Value);
                return true;
            }
            var lng = value as long?;
            if (lng.HasValue)
            {
                document.SetCellValue(rowIdx, colIdx, lng.Value);
                return true;
            }
            var shrt = value as short?;
            if (shrt.HasValue)
            {
                document.SetCellValue(rowIdx, colIdx, shrt.Value);
                return true;
            }
            var dbl = value as double?;
            if (dbl.HasValue)
            {
                document.SetCellValue(rowIdx, colIdx, dbl.Value);
                return true;
            }
            var byt = value as byte?;
            if (byt.HasValue)
            {
                document.SetCellValue(rowIdx, colIdx, byt.Value);
                return true;
            }
            var timeSpan = value as TimeSpan?;
            if (timeSpan.HasValue)
            {
                document.SetCellValue(rowIdx, colIdx, timeSpan.Value.ToString());
                return true;
            }
            return false;
        }
    }
}
