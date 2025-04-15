using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Timetables.DTOs
{
    public class TimetableListDto
    {
        public int TimetableId { get; set; }
        public int SemesterId { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string Status { get; set; }
    }
}
