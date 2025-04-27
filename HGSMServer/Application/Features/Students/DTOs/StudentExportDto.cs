using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Students.DTOs
{
    public class StudentExportDto
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Dob { get; set; }  // Định dạng ngày tháng xuất ra Excel
        public string Gender { get; set; } = null!;
        public string ClassName { get; set; } = null!; // Lấy tên lớp thay vì ID
        public string AdmissionDate { get; set; }
        public string? EnrollmentType { get; set; }
        public string? Ethnicity { get; set; }
        public string? PermanentAddress { get; set; }
        public string? BirthPlace { get; set; }
        public string? Religion { get; set; }
        public string? IdcardNumber { get; set; }
        public string? Status { get; set; }
    }

}
