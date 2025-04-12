using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Timetables.DTOs
{
    public class UpdateTimetableInfoDto
    {
        [Required]
        public int TimetableId { get; set; }

        [Required]
        public int SemesterId { get; set; }

        [Required]
        public DateOnly EffectiveDate { get; set; }

        [Required]
        public string Status { get; set; }
    }
}
