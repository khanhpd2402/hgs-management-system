using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Grades.DTOs
{
    public class ImportGradesResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Số lượng bản ghi điểm (Grade) đã được cập nhật thành công.
        /// (Vì bạn chỉ cập nhật, nên không có ImportedRecords)
        /// </summary>
        public int UpdatedRecords { get; set; }

        /// <summary>
        /// Danh sách các lỗi hoặc cảnh báo chi tiết xảy ra trong quá trình xử lý.
        /// Ví dụ: "Học sinh với mã 'XYZ' không tìm thấy.", "Lỗi dữ liệu: Không tìm thấy bản ghi điểm có sẵn..."
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// DTO đại diện cho thông tin điểm của một học sinh được đọc từ file Excel.
    /// Đây là DTO trung gian, sử dụng nội bộ trong GradeService.
    /// </summary>
    public class StudentGradeImportDto
    {
        public int StudentId { get; set; }

        /// <summary>
        /// Danh sách các mục điểm cụ thể cho học sinh này (ví dụ: điểm TX1, GK, CK).
        /// </summary>
        public List<GradeEntryDto> GradeEntries { get; set; } = new List<GradeEntryDto>();

        /// <summary>
        /// Nhận xét chung của giáo viên cho học sinh này đối với môn học, được lấy từ cột "Nhận xét" trong file Excel.
        /// </summary>
        //public string? TeacherComment { get; set; }
    }

    /// <summary>
    /// DTO đại diện cho một mục điểm cụ thể (ví dụ: một cột điểm thành phần) của học sinh từ file Excel.
    /// </summary>
    public class GradeEntryDto
    {
        /// <summary>
        /// Tên loại đánh giá/kiểm tra (ví dụ: "ĐĐG TX 1", "ĐĐG GK", "ĐĐG CK", "ĐĐG TX (KTtx M1)").
        /// Tên này phải khớp với `AssessmentsTypeName` trong entity `Grade`.
        /// </summary>
        public string AssessmentsTypeName { get; set; } = string.Empty;

        /// <summary>
        /// Điểm số (dưới dạng chuỗi để có thể lưu cả số và chữ như "Đ", "CĐ").
        /// </summary>
        public string? Score { get; set; }

        // Ghi chú: Nếu trong file Excel có cột nhận xét riêng cho từng mục điểm (ngoài nhận xét chung)
        // thì có thể thêm thuộc tính IndividualComment vào đây.
        // public string? IndividualComment { get; set; }
    }
}
