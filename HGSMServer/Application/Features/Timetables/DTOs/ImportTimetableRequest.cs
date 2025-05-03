using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Timetables.DTOs
{
    public class ImportTimetableRequest
    {
        [Required(ErrorMessage = "Vui lòng cung cấp ID năm học.")]
        public int AcademicYearId { get; set; }

        [Required(ErrorMessage = "Vui lòng cung cấp ID học kỳ.")]
        public int SemesterId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn file Excel để import.")]
        public IFormFile File { get; set; } = null!;
    }
}