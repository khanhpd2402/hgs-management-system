using Common.Constants;
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
        [Required(ErrorMessage = "TimetableId is required.")]
        public int TimetableId { get; set; }

        [Required(ErrorMessage = "SemesterId is required.")]
        public int SemesterId { get; set; }

        public DateOnly EffectiveDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string Status { get; set; } = AppConstants.Status.PENDING;
    }
}
