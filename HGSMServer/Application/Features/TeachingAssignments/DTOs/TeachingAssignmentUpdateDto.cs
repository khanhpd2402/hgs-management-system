using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.TeachingAssignments.DTOs
{
    public class TeachingAssignmentUpdateDto
    {
        public int SubjectId { get; set; }
        public List<ClassAssignmentDto> ClassAssignments { get; set; }
    }
}