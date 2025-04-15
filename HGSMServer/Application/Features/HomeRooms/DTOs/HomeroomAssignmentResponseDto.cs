using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HomeRooms.DTOs
{
    public class HomeroomAssignmentResponseDto
    {
        public int HomeroomAssignmentId { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int SemesterId { get; set; }
        public string SemesterName { get; set; }
        public string Status { get; set; }
    }
}
