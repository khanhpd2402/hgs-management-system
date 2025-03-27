using System.Collections.Generic;

namespace Application.Features.Teachers.DTOs
{
    public class TeacherListResponseDto
    {
        public IEnumerable<TeacherListDto> Teachers { get; set; } = new List<TeacherListDto>();
        public int TotalCount { get; set; }
    }
}