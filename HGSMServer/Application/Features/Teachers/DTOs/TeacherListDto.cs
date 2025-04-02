using System;

namespace Application.Features.Teachers.DTOs
{
    public class TeacherListDto
    {
        public int TeacherId { get; set; }
        public string FullName { get; set; } = null!;
        public DateOnly Dob { get; set; }  // Ngày sinh
        public string Gender { get; set; } = null!;
        public string? Ethnicity { get; set; }
        public string? Religion { get; set; }
        public string? MaritalStatus { get; set; }
        public string? IdcardNumber { get; set; }
        public string? InsuranceNumber { get; set; }
        public string? EmploymentType { get; set; }
        public string? Position { get; set; }
        public string? Department { get; set; }
        public string? AdditionalDuties { get; set; }
        public bool? IsHeadOfDepartment { get; set; }
        public string? EmploymentStatus { get; set; }
        public string? RecruitmentAgency { get; set; }
        public DateOnly? HiringDate { get; set; }  // Ngày tuyển dụng
        public DateOnly? PermanentEmploymentDate { get; set; }  // Ngày biên chế chính thức
        public DateOnly SchoolJoinDate { get; set; }  // Ngày vào trường
        public string? PermanentAddress { get; set; }
        public string? Hometown { get; set; }

        // Thông tin từ bảng Users
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? TeachingSubject { get; set; }
    }
}