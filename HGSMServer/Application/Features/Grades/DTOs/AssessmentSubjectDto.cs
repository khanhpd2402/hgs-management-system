using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Grades.DTOs
{
    public class AssessmentSubjectDto
    {
        public int BatchID { get; set; }
        public string AssessmentsTypeName { get; set; } = null!;
        public int SubjectID { get; set; }
    }
}
