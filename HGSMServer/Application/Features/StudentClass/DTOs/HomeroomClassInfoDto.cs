using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.StudentClass.DTOs
{
    public class HomeroomClassInfoDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int SemesterId { get; set; }
        public string SemesterName { get; set; }
        public int TotalStudents { get; set; }
    }
}
