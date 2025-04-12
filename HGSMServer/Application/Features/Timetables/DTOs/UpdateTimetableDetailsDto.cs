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
        [Required]
        public int TimetableId { get; set; }

        [Required]
        public List<TimetableDetailUpdateDto>? Details { get; set; }
    }

    public class TimetableDetailUpdateDto
    {
        [Required]
        public int TimetableDetailId { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public DateOnly Date { get; set; }
        public int PeriodId { get; set; }
    }
}
