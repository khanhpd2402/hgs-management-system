using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.TeachingAssignment.DTOs
{
    public class TeachingAssignmentResponseDto
    {
        public int AssignmentId { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int SemesterId { get; set; }
        public string SemesterName { get; set; }
        public int ActualPeriodsPerWeekHK1 { get; set; } // Số tiết/tuần thực tế HK1
        public int ActualPeriodsPerWeekHK2 { get; set; } // Số tiết/tuần thực tế HK2
    }
}
