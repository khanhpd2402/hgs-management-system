using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exams.DTOs
{
    public class StatusUpdateDto
    {
        public string Status { get; set; }
        public string? Comment { get; set; }
    }
}
