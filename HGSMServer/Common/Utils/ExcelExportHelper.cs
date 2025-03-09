using ClosedXML.Excel;
namespace Common.Utils
{
    public static class ExcelExporter
    {
        public static byte[] ExportToExcel<T>(List<T> data, Dictionary<string, string> columnMappings, string reportTitle, string academicYear, List<string>? selectedColumns, bool isReport) // Thêm tham số này
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Data");
            int currentRow = 1;
            if (isReport)
            {
                // Tiêu đề báo cáo
                AddReportHeader(worksheet, ref currentRow, columnMappings.Count, reportTitle, academicYear);
            }
            // Lọc các cột hợp lệ dựa vào selectedColumns
            var validColumns = selectedColumns == null || selectedColumns.Count == 0
                ? columnMappings.Keys.ToList()
                : columnMappings.Keys.Where(c => selectedColumns.Contains(c)).ToList();

            if (!validColumns.Any())
            {
                throw new Exception("Không có cột hợp lệ để xuất dữ liệu.");
            }

            // Ghi tiêu đề cột
            int headerRow = currentRow;
            int colIndex = 1;
            foreach (var column in validColumns)
            {
                worksheet.Cell(headerRow, colIndex).Value = columnMappings[column];
                worksheet.Cell(headerRow, colIndex).Style.Font.SetBold();
                colIndex++;
            }

            // Ghi dữ liệu
            int rowIndex = headerRow + 1;
            foreach (var item in data)
            {
                colIndex = 1;
                foreach (var column in validColumns)
                {
                    var property = typeof(T).GetProperty(column);
                    worksheet.Cell(rowIndex, colIndex).Value = property?.GetValue(item)?.ToString();
                    colIndex++;
                }
                rowIndex++;
            }
            worksheet.Columns().AdjustToContents(); // Tự động căn chỉnh độ rộng cột
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }


        private static void AddReportHeader(IXLWorksheet worksheet, ref int currentRow, int columnCount, string reportTitle, string academicYear)
        {
            var headers = new List<string>
        {
            "BỘ GIÁO DỤC VÀ ĐÀO TẠO",
            "TRƯỜNG THCS HẢI GIANG",
            "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM",
            "Độc Lập - Tự Do - Hạnh Phúc"
        };
            var fontSizes = new[] { 14, 13, 12, 11 };

            for (int i = 0; i < headers.Count; i++)
            {
                worksheet.Cell(currentRow, 1).Value = headers[i];
                worksheet.Range(currentRow, 1, currentRow, columnCount).Merge().Style.Font.SetBold().Font.FontSize = fontSizes[i];
                worksheet.Row(currentRow++).Height = 18;
            }

            worksheet.Cell(currentRow, columnCount).Value = $"Ngày {DateTime.Now:dd} tháng {DateTime.Now:MM} năm {DateTime.Now:yyyy}";
            worksheet.Range(currentRow, columnCount, currentRow, columnCount).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = reportTitle;
            worksheet.Range(currentRow, 1, currentRow, columnCount).Merge().Style.Font.SetBold().Font.FontSize = 14;
            worksheet.Row(currentRow++).Height = 22;

            worksheet.Cell(currentRow, 1).Value = academicYear;
            worksheet.Range(currentRow, 1, currentRow, columnCount).Merge().Style.Font.SetBold().Font.FontSize = 12;
            worksheet.Row(currentRow++).Height = 18;
        }
    }

}

