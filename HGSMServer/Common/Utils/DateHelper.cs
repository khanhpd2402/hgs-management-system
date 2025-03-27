using Common.Constants;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
    public static class DateHelper
    {

        // Chuyển chuỗi thành DateOnly
        public static DateOnly ParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                throw new Exception("Ngày không được để trống.");
            }

            if (!DateOnly.TryParseExact(dateString.Trim(), AppConstants.DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                throw new Exception($"Ngày '{dateString}' không đúng định dạng {AppConstants.DATE_FORMAT}.");
            }

            return date;
        }
    }

}
