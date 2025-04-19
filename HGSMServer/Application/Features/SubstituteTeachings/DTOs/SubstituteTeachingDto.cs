using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SubstituteTeachings.DTOs
{
    public class SubstituteTeachingDto
    {
        public int SubstituteId { get; set; }
        public int TimetableDetailId { get; set; }
        public int OriginalTeacherId { get; set; }
        public string OriginalTeacherName { get; set; }
        public int SubstituteTeacherId { get; set; }
        public string SubstituteTeacherName { get; set; }
        public DateOnly Date { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }

        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string DayOfWeek { get; set; }
        public int PeriodId { get; set; }
        public string PeriodName { get; set; }
    }
}
