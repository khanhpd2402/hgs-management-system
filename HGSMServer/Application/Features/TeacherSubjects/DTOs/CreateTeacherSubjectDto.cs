using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.TeacherSubjects.DTOs
{
    public class CreateTeacherSubjectDto
    {
        public int TeacherId { get; set; }
        public int SubjectId { get; set; }
        public bool? IsMainSubject { get; set; }
    }
}
