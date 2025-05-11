
namespace Application.Features.Grades.DTOs
{
    /// <summary>
    /// DTO chứa thông tin tổng kết điểm của cả lớp cho một học kỳ.
    /// </summary>
    public class ClassGradesSummaryDto
    {
        public int ClassId { get; set; }
        public string? ClassName { get; set; } // Ví dụ: "Lớp 6A"
        public int SemesterId { get; set; }
        public string? SemesterName { get; set; } // Ví dụ: "Học kỳ I"
        public string? AcademicYear { get; set; } // Ví dụ: "2024-2025" (Lấy từ Semester)
        public List<StudentOverallGradesDto> Students { get; set; } = new List<StudentOverallGradesDto>();
    }

    /// <summary>
    /// DTO chứa thông tin tổng kết điểm của một học sinh.
    /// </summary>
    public class StudentOverallGradesDto
    {
        public int StudentId { get; set; }
        public string? FullName { get; set; }    // Họ và tên học sinh
        public List<SubjectResultDto> SubjectResults { get; set; } = new List<SubjectResultDto>();
    }

    /// <summary>
    /// DTO chứa kết quả của một môn học cho học sinh.
    /// </summary>
    public class SubjectResultDto
    {
        public int SubjectId { get; set; }
        public string? SubjectName { get; set; } // Tên môn học
        /// <summary>
        /// Điểm tổng kết môn học (dạng chuỗi để hiển thị cả số và chữ "Đạt"/"Chưa Đạt").
        /// </summary>
        public string? FinalScore { get; set; }
        /// <summary>
        /// Loại hình đánh giá môn học ("Điểm số" hoặc "Đánh giá").
        /// </summary>
        public string? GradeType { get; set; }
    }
}
