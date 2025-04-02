using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;

namespace Common.Utils
{
    public static class ExcelImportHelper
    {
        public static List<Dictionary<string, string>> ReadExcelData(IFormFile file)
        {
            using var stream = new MemoryStream();
            file.CopyTo(stream);
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);

            var rowCount = worksheet.LastRowUsed().RowNumber();
            var colCount = worksheet.LastColumnUsed().ColumnNumber();

            var headers = new List<string>();
            for (int col = 1; col <= colCount; col++)
            {
                headers.Add(worksheet.Cell(1, col).Value.ToString());
            }

            var dataList = new List<Dictionary<string, string>>();
            var dateFormat = "dd/MM/yyyy";

            for (int row = 2; row <= rowCount; row++)
            {
                var rowData = new Dictionary<string, string>();
                for (int col = 1; col <= colCount; col++)
                {
                    var cell = worksheet.Cell(row, col);
                    string cellValue;

                    if (cell.DataType == XLDataType.DateTime)
                    {
                        var dateValue = cell.GetDateTime();
                        cellValue = dateValue.ToString(dateFormat);
                    }
                    else
                    {
                        cellValue = cell.Value.ToString();
                        if (cellValue.StartsWith("'"))
                        {
                            cellValue = cellValue.Substring(1);
                        }
                    }

                    rowData[headers[col - 1]] = cellValue;
                }
                dataList.Add(rowData);
            }

            return dataList;
        }
    }
}