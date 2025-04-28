using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.GradeBatchs.DTOs
{
    public class UpdateGradeBatchDto
    {
        public string BatchName { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Status { get; set; }
        public int SemesterId { get; set; }
    }
}
