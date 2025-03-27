using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Grades.DTOs
{
    public class GradeRespondDto
    {
        public int GradeId { get; set; }
        public int StudentID { get; set; }
        public string FullName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Score { get; set; }
        public string AssessmentsTypeName { get; set; }
        public string TeacherComment { get; set; }
    }
}
