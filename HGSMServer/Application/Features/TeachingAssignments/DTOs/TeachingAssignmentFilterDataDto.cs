using Application.Features.Classes.DTOs;
using Application.Features.Subjects.DTOs;
using Application.Features.Teachers.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.TeachingAssignments.DTOs
{
    public class TeachingAssignmentFilterDataDto
    {
        public List<TeacherListDto> Teachers { get; set; }
        public List<SubjectDto> Subjects { get; set; }
        public List<ClassDto> Classes { get; set; }
    }
}
