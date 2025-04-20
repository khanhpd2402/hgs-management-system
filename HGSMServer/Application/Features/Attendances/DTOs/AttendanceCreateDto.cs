using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Attendances.DTOs
{
    public class AttendanceCreateDto
    {
        public int StudentId { get; set; }
        public string Session { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
        public DateTime Date { get; set; }
    }
}
