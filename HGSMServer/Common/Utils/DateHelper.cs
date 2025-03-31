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

        public static DateOnly ParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return default; // Trả về 01/01/0001 nếu ngày trống
            }

            if (!DateOnly.TryParseExact(dateString.Trim(), AppConstants.DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                throw new Exception($"Ngày '{dateString}' không đúng định dạng {AppConstants.DATE_FORMAT}.");
            }

            return date;
        }


    }

}
