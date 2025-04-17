using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SubstituteTeachings.DTOs
{
    public class SubstituteTeachingUpdateDto
    {
        public int SubstituteId { get; set; }

        public int TimetableDetailId { get; set; }

        public int OriginalTeacherId { get; set; }

        public int SubstituteTeacherId { get; set; }

        public DateOnly Date { get; set; }

        public string? Note { get; set; }
    }
}
