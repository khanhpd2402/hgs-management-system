using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
    public static class FormatUserName
    {
        public static string GenerateUsername(string fullName, int id)
        {
            if (string.IsNullOrEmpty(fullName)) return $"user{id}";

            string[] words = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0) return $"user{id}";

            string lastName = words[^1].ToLower(); // Tên (cuối cùng)
            string firstChar = words[0][0].ToString().ToLower(); // Chữ cái đầu họ
            string middleChars = words.Length > 2 ? new string(words[1].Take(2).ToArray()).ToLower() : ""; // 2 ký tự đầu họ đệm

            // Loại bỏ dấu tiếng Việt
            lastName = RemoveDiacritics(lastName);
            middleChars = RemoveDiacritics(middleChars);
            firstChar = RemoveDiacritics(firstChar);

            return $"{lastName}{firstChar}{middleChars}{id}";
        }

        /// <summary>
        /// Loại bỏ dấu tiếng Việt khỏi chuỗi.
        /// </summary>
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            string normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
