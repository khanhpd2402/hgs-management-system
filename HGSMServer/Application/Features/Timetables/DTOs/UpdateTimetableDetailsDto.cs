using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Timetables.DTOs
{
    public class UpdateTimetableDetailsDto
    {
        [Required(ErrorMessage = "TimetableId is required.")]
        public int TimetableId { get; set; }

        [Required(ErrorMessage = "Details is required.")]
        public List<TimetableDetailUpdateDto> Details { get; set; }
    }

    public class TimetableDetailUpdateDto
    {
        public int TimetableDetailId { get; set; }

        public int ClassId { get; set; }

        public int SubjectId { get; set; }

        public int TeacherId { get; set; }

        public string DayOfWeek { get; set; }

        public int PeriodId { get; set; }
    }
}
