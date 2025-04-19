using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.TeachingAssignments.DTOs
{
    public class TeachingAssignmentUpdateDto
    {
        public int TeacherId { get; set; }
        public int SubjectId { get; set; }
        public string SubjectCategory { get; set; }
        public List<ClassAssignmentDto> ClassAssignments { get; set; }
        public int SemesterId { get; set; }
    }
}