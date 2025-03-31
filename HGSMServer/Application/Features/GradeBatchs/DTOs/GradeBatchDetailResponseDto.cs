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
        public bool? IsActive { get; set; }
        public List<int> ClassIds { get; set; } // thêm
        public List<SubjectDto> Subjects { get; set; }
        public List<AssessmentTypeDto> AssessmentTypes { get; set; }
    }

    public class SubjectGradeBatchDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
    }

    public class AssessmentTypeGradeBatchDto
    {
        public int SubjectId { get; set; }
        public string AssessmentTypeName { get; set; }
        public int ClassId { get; set; }
    }


}
