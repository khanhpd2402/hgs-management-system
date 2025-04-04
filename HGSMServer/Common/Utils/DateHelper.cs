using System.Globalization;

namespace Common.Utils
{
    public static class DateHelper
    {
        // Định dạng ngày mặc định
        private static readonly string[] SupportedDateFormats = new[]
        {
            "dd/MM/yyyy", // 01/09/1999
            "dd/M/yyyy",  // 01/9/1999
            "d/MM/yyyy",  // 1/09/1999
            "d/M/yyyy"    // 1/9/1999
        };

        public static DateOnly ParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return default; // Trả về 01/01/0001 nếu ngày trống
            }

            // Loại bỏ dấu nháy đơn ở đầu và khoảng trắng thừa
            string cleanedDateString = dateString.Trim().TrimStart('\'');

            // Thử các định dạng được hỗ trợ
            foreach (var format in SupportedDateFormats)
            {
                if (DateOnly.TryParseExact(cleanedDateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    return date;
                }
            }

            // Thử định dạng khác: yyyy-MM-dd HH:mm:ss (nếu có dấu gạch ngang)
            if (cleanedDateString.Contains("-") && DateTime.TryParse(cleanedDateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                return DateOnly.FromDateTime(dt);
            }

            // Nếu không parse được, thử phân tích linh hoạt với TryParse
            if (DateOnly.TryParse(cleanedDateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out var flexibleDate))
            {
                return flexibleDate;
            }

            // Nếu vẫn không được, ném lỗi với thông tin chi tiết
            throw new Exception($"Ngày '{dateString}' không đúng định dạng ({string.Join(", ", SupportedDateFormats)}) hoặc không thể phân tích.");
        }
    }
}