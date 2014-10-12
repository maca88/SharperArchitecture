using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Reporting;

namespace PowerArhitecture.Reporting
{
    public class CsvReport : BaseReport<CsvReportSettings>
    {
        private char _delimiter = ',';

        public override string Extension
        {
            get { return "csv"; }
        }

        public override string MimeType
        {
            get { return "text/csv"; }
        }

        public override Stream CreateReport<TItem>(Parameters<TItem> parameters)
        {
            //Apply settings
            if (Settings.Delimiter != default(char))
                _delimiter = Settings.Delimiter;

            //Create report
            var type = typeof(TItem);
            var itemPropertiesCol = GetItemProperties(type, parameters.ItemProperties);
            var lastPropName = itemPropertiesCol.Last().PropertyName;

            using (var mStream = new MemoryStream())
            using (var writer = new StreamWriter(mStream))
            {
                // The header
                foreach (var field in itemPropertiesCol)
                {
                    writer.Write(MakeValueCsvFriendly(field.HeaderName));
                    if (lastPropName != field.PropertyName)
                        writer.Write(_delimiter);
                }
                writer.WriteLine();

                // The rows
                var props = itemPropertiesCol.Select(o => type.GetProperty(o.PropertyName)).ToList();
                foreach (var item in parameters.Items)
                {
                    foreach (var prop in props)
                    {
                        var value = prop.GetValue(item, null) ?? "";
                        writer.Write(MakeValueCsvFriendly(value));
                        if (lastPropName != prop.Name)
                            writer.Write(_delimiter);
                    }
                    writer.WriteLine();
                }
                return mStream;
            }
        }

        public override string Type
        {
            get { return ReportType.Csv; }
        }

        /// <summary>
        /// Converts a value to how it should output in a csv file
        /// If it has a comma, it needs surrounding with double quotes
        /// Eg Sydney, Australia -> "Sydney, Australia"
        /// Also if it contains any double quotes ("), then they need to be replaced with quad quotes[sic] ("")
        /// Eg "Dangerous Dan" McGrew -> """Dangerous Dan"" McGrew"
        /// </summary>
        private string MakeValueCsvFriendly(object value)
        {
            if (value == null) return "";
            if (value is INullable && ((INullable)value).IsNull) return "";
            if (value is DateTime)
            {
                if (((DateTime)value).TimeOfDay.TotalSeconds == 0)
                    return ((DateTime)value).ToString("yyyy-MM-dd");
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            var output = value.ToString();
            if (output.Contains(_delimiter) || output.Contains("\""))
                output = '"' + output.Replace("\"", "\"\"") + '"';
            return output;
        }
    }
}
