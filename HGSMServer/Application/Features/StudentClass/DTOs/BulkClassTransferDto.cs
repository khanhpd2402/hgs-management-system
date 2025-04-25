using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.StudentClass.DTOs
{
    public class BulkClassTransferDto
    {
        public int ClassId { get; set; } 
        public int? AcademicYearId { get; set; } 
        public int TargetClassId { get; set; } 
        public int? TargetAcademicYearId { get; set; } 
    }
}
