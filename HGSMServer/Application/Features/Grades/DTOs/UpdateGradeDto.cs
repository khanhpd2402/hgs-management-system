using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Grades.DTOs
{
    public class UpdateGradeDto
    {
        public int GradeID { get; set; }
        public string Score { get; set; }
        public string TeacherComment { get; set; }
    }
    public class UpdateMultipleGradesDto
    {
        public List<UpdateGradeDto> Grades { get; set; }
    }

}

