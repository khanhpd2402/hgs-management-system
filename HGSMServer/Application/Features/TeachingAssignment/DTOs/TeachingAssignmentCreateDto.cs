using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.TeachingAssignment.DTOs
{
    public class TeachingAssignmentCreateDto
    {
        public int TeacherId { get; set; } // Giáo viên được chọn
        public int SubjectId { get; set; } // Môn học được chọn
        public string SubjectCategory { get; set; } // Phân môn (Hình học, Đại số)
        public List<ClassAssignmentDto> ClassAssignments { get; set; } // Danh sách lớp
        public int SemesterId { get; set; } // Học kỳ (SemesterID)
    }
}
