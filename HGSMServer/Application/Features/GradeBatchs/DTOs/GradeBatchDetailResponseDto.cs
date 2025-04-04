using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.GradeBatchs.DTOs
{
    public class GradeBatchDetailResponseDto
    {
        public int BatchId { get; set; }
        public string BatchName { get; set; }
        public int SemesterId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Status { get; set; } // Trạng thái đợt nhập điểm
        public List<int> ClassIds { get; set; } = new List<int>(); // Danh sách ID các lớp liên quan
        public List<SubjectGradeBatchDto> Subjects { get; set; } = new List<SubjectGradeBatchDto>();
        public List<AssessmentTypeGradeBatchDto> AssessmentTypes { get; set; } = new List<AssessmentTypeGradeBatchDto>();
    }

    public class SubjectGradeBatchDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
    }

    public class AssessmentTypeGradeBatchDto
    {
        public int SubjectId { get; set; } // ID môn học liên quan đến loại đánh giá
        public string AssessmentTypeName { get; set; }
        public int ClassId { get; set; } // ID lớp học liên quan
    }


}
