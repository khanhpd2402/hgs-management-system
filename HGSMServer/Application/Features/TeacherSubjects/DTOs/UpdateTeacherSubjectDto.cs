using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.TeacherSubjects.DTOs
{
    public class UpdateTeacherSubjectDto
    {
        public int TeacherId { get; set; }
        public List<SubjectUpdateInfo> Subjects { get; set; }
    }

    public class SubjectUpdateInfo
    {
        public int SubjectId { get; set; }
        public bool IsMainSubject { get; set; }
    }
}
