using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Timetables.DTOs
{
    // DTO cho thêm/sửa thời khóa biểu thủ công
    public class ManualTimetableDto
    {
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public string DayOfWeek { get; set; } = null!;
        public string Shift { get; set; } = null!; // Morning/Afternoon
        public int Period { get; set; }
        public string SchoolYear { get; set; } = null!;
        public int Semester { get; set; }
        public DateOnly EffectiveDate { get; set; }
    }

    // DTO cho hiển thị thời khóa biểu theo vai trò
    public class RoleBasedTimetableDto
    {
        public string ClassName { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public string TeacherName { get; set; } = null!;
        public string DayOfWeek { get; set; } = null!;
        public string Shift { get; set; } = null!;
        public int Period { get; set; }
    }
    public class TimetableDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = null!;
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = null!;
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = null!;
        public string DayOfWeek { get; set; } = null!;
        public int Period { get; set; }
    }

}
