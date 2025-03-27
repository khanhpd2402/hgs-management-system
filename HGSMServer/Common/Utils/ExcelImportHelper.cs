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
            var worksheet = workbook.Worksheet(1); // Lấy sheet đầu tiên

            var rowCount = worksheet.LastRowUsed().RowNumber();
            var colCount = worksheet.LastColumnUsed().ColumnNumber();

            // Đọc tiêu đề cột
            var headers = new List<string>();
            for (int col = 1; col <= colCount; col++)
            {
                headers.Add(worksheet.Cell(1, col).Value.ToString());
            }

            var dataList = new List<Dictionary<string, string>>();

            // Đọc dữ liệu từ hàng thứ 2
            for (int row = 2; row <= rowCount; row++)
            {
                var rowData = new Dictionary<string, string>();
                for (int col = 1; col <= colCount; col++)
                {
                    rowData[headers[col - 1]] = worksheet.Cell(row, col).Value.ToString();
                }
                dataList.Add(rowData);
            }

            return dataList;
        }
    }

}
