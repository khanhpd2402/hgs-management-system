using Common.Constants;
using System.Globalization;

namespace Common.Utils
{
    public static class DateHelper
    {
        public static DateOnly ParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                throw new Exception("Ngày không được để trống.");
            }

            // Loại bỏ dấu nháy đơn ở đầu và khoảng trắng thừa
            string cleanedDateString = dateString.Trim().TrimStart('\'');

            // Thử định dạng chính: dd/MM/yyyy
            if (DateOnly.TryParseExact(cleanedDateString, AppConstants.DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date;
            }

            // Thử định dạng khác: yyyy-MM-dd HH:mm:ss (như của Trần Thị Mai Lụa)
            if (cleanedDateString.Contains("-") && DateTime.TryParse(cleanedDateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                return DateOnly.FromDateTime(dt);
            }

            // Nếu không parse được, ném lỗi với thông tin chi tiết
            throw new Exception($"Ngày '{dateString}' không đúng định dạng {AppConstants.DATE_FORMAT} hoặc không thể phân tích.");
        }
    }
}