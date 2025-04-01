using System.ComponentModel.DataAnnotations;

namespace Application.Features.Users.DTOs
{
    public class CreateUserDTO
    {
        [Required(ErrorMessage = "RoleId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "RoleId must be a positive integer.")]
        public int RoleId { get; set; }

        // Trường chung cho tất cả role
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        // Thông tin cha (cho Parent)
        public string? FullNameFather { get; set; }
        public DateTime? YearOfBirthFather { get; set; }
        public string? OccupationFather { get; set; }
        public string? PhoneNumberFather { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? EmailFather { get; set; }
        public string? IdcardNumberFather { get; set; }

        // Thông tin mẹ (cho Parent)
        public string? FullNameMother { get; set; }
        public DateTime? YearOfBirthMother { get; set; }
        public string? OccupationMother { get; set; }
        public string? PhoneNumberMother { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? EmailMother { get; set; }
        public string? IdcardNumberMother { get; set; }

        // Thông tin người bảo hộ (cho Parent)
        public string? FullNameGuardian { get; set; }
        public DateTime? YearOfBirthGuardian { get; set; }
        public string? OccupationGuardian { get; set; }
        public string? PhoneNumberGuardian { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? EmailGuardian { get; set; }
        public string? IdcardNumberGuardian { get; set; }

        // Thông tin cho Teacher (và các role khác ngoài Parent)
        [Required(ErrorMessage = "FullName is required for non-Parent roles.", AllowEmptyStrings = false)]
        public string? FullName { get; set; }
        [Required(ErrorMessage = "DOB is required for non-Parent roles.")]
        public DateTime? DOB { get; set; }
        [Required(ErrorMessage = "Gender is required for non-Parent roles.", AllowEmptyStrings = false)]
        public string? Gender { get; set; }
        [Required(ErrorMessage = "SchoolJoinDate is required for non-Parent roles.")]
        public DateTime? SchoolJoinDate { get; set; }
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
        public DateTime? HiringDate { get; set; }
        public DateTime? PermanentEmploymentDate { get; set; }
        public string? PermanentAddress { get; set; }
        public string? Hometown { get; set; }
    }
}