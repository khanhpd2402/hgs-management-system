using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Attendances.DTOs
{
    public class AttendanceDto
    {
        public int AttendanceId { get; set; }
        public int StudentClassId { get; set; }
        public DateOnly Date { get; set; }
        public string Session { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string? Note { get; set; }
        public bool CanEdit { get; set; } // Dùng cho UI: chỉ cho sửa trong cùng ngày
    }
}
