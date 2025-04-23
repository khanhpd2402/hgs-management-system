using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Attendances.DTOs
{
    public class AttendanceUpdateDto
    {
        public int AttendanceId { get; set; }
        public string Status { get; set; } = default!;
        public string? Note { get; set; }
    }
}
