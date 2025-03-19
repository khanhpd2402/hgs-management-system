using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Subjects.DTOs
{
    public class SubjectDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = null!;
        public string SubjectCategory { get; set; } = null!;
        public string TypeOfGrade { get; set; } = null!;
    }
}
