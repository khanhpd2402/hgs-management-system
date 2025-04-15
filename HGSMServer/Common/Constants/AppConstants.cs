using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Constants
{
    public static class AppConstants
    {
        public const int MAX_TOP_ODATA = 100;
        public const string DATE_FORMAT = "dd/MM/yyyy";
        // Trạng thái chung
        public static class Status
        {
            public const string ACTIVE = "Hoạt động";
            public const string INACTIVE = "Không hoạt động";
            public const string PENDING = "Chờ Duyệt";
            public static readonly string[] All = { ACTIVE, INACTIVE, PENDING };
        }
        public static class StudentStatus
        {
            public const string STUDYING = "Đang học";
            public const string RESERVED = "Bảo lưu";
            public const string DROPPED_OUT = "Nghỉ học";
            public const string GRADUATED = "Tốt nghiệp";
            public const string TRANSFERRED = "Chuyển trường";
            public static readonly string[] All = { STUDYING, RESERVED, DROPPED_OUT, GRADUATED, TRANSFERRED };
        }
        // Độ khó câu hỏi (Questions.Difficulty)
        public static class Difficulty
        {
            public const string EASY = "Dễ";
            public const string MEDIUM = "Trung bình";
            public const string HARD = "Khó";
            public static readonly string[] All = { EASY, MEDIUM, HARD };
        }

        // Loại câu hỏi (Questions.QuestionType)
        public static class QuestionType
        {
            public const string MULTIPLE_CHOICE = "Trắc nghiệm";
            public const string ESSAY = "Tự luận";
            public static readonly string[] All = { MULTIPLE_CHOICE, ESSAY };
        }

        // Trạng thái điểm danh (Attendances.Status)
        public static class AttendanceStatus
        {
            public const string PRESENT = "C"; // Có mặt
            public const string PERMISSION = "P"; // Nghỉ có phép
            public const string ABSENT = "K"; // Nghỉ không phép
            public const string LATE = "X"; // Đi muộn
            public static readonly string[] All = { PRESENT, PERMISSION, ABSENT, LATE };
        }

        // Loại điểm (Subjects.TypeOfGrade)
        public static class GradeType
        {
            public const string NUMERIC = "Tính điểm";
            public const string COMMENT = "Nhận xét";
            public static readonly string[] All = { NUMERIC, COMMENT };
        }

        // Ca học (Periods.Shift)
        public static class Shift
        {
            public const byte MORNING = 1;
            public const byte AFTERNOON = 2;
            public static readonly byte[] All = { MORNING, AFTERNOON };
        }
        public static class DayOfWeek
        {
            public const string MONDAY = "Thứ Hai";
            public const string TUESDAY = "Thứ Ba";
            public const string WEDNESDAY = "Thứ Tư";
            public const string THURSDAY = "Thứ Năm";
            public const string FRIDAY = "Thứ Sáu";
            public const string SATURDAY = "Thứ Bảy";
            public const string SUNDAY = "Chủ Nhật";

            public static readonly string[] All = { MONDAY, TUESDAY, WEDNESDAY, THURSDAY, FRIDAY, SATURDAY, SUNDAY };
        }
    }
}
