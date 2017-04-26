using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharperArchitecture.Reporting.Extensions
{
    public static class StringExtensions
    {
        public static int GetCellRowIndex(this string cell)
        {
            var match = Regex.Match(cell, @"([A-Z]+)([\d]*)");
            if (!match.Success) return -1;
            return int.Parse(match.Groups[2].Value) - 1;
        }

        public static string GetExcelColumn(this string cell, int offset = 0)
        {
            var match = Regex.Match(cell, @"([A-Z]+)([\d]*)");
            if (!match.Success) return null;
            var column = match.Groups[1].Value;
            var cNum = GetExcelColumnNumber(column);
            return GetExcelColumnName(cNum + offset);
        }

        public static int GetExcelColumnNumber(this string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");
            var characters = columnName.ToUpperInvariant().ToCharArray();
            var sum = 0;
            foreach (var t in characters)
            {
                sum *= 26;
                sum += (t - 'A' + 1);
            }
            return (sum - 1);
        }

        public static string GetExcelColumnName(this int columnNumber)
        {
            columnNumber++;
            var dividend = columnNumber;
            var columnName = String.Empty;

            while (dividend > 0)
            {
                var modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString(CultureInfo.InvariantCulture) + columnName;
                dividend = ((dividend - modulo) / 26);
            }
            return columnName;
        }
    }
}
