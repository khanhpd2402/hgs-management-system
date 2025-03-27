using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.GradeBatchs.DTOs
{
    public class GradeBatchDto
    {
        public int BatchId { get; set; }
        public string BatchName { get; set; } = string.Empty;
        public int SemesterId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
