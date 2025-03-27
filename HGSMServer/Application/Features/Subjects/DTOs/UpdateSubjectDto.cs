using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Subjects.DTOs
{
    public class UpdateSubjectDto
    {
        public string? SubjectName { get; set; }
        public string? SubjectCategory { get; set; }
        public string? TypeOfGrade { get; set; }
    }
}
