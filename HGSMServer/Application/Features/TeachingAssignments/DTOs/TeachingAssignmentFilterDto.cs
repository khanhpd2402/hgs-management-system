using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.TeachingAssignments.DTOs
{
    public class TeachingAssignmentFilterDto
    {
        public int? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public int? SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public int? ClassId { get; set; }
        public string? ClassName { get; set; }
        public int? SemesterId { get; set; }
    }
}
