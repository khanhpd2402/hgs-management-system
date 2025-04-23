using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.StudentClass.DTOs
{
    public class StudentFilterDto
    {
        public int StudentClassId { get; set; }
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Status { get; set; }
    }
}
