using System;

namespace Application.Features.Students.DTOs
{
    public class UpdateStudentDto
    {
        public int StudentId { get; set; } // Bắt buộc cần ID khi update
        public string FullName { get; set; } = null!;
        public DateOnly Dob { get; set; }
        public string Gender { get; set; } = null!;
        public int ClassId { get; set; }
        public DateOnly AdmissionDate { get; set; }
        public string? EnrollmentType { get; set; }
        public string? Ethnicity { get; set; }
        public string? PermanentAddress { get; set; }
        public string? BirthPlace { get; set; }
        public string? Religion { get; set; }
        public bool? RepeatingYear { get; set; }
        public string? IdcardNumber { get; set; }
        public string? Status { get; set; }

        // Thông tin cha
        public string? FullNameFather { get; set; }
        public DateOnly? YearOfBirthFather { get; set; }
        public string? OccupationFather { get; set; }
        public string? PhoneNumberFather { get; set; }
        public string? EmailFather { get; set; }
        public string? IdcardNumberFather { get; set; }

        // Thông tin mẹ
        public string? FullNameMother { get; set; }
        public DateOnly? YearOfBirthMother { get; set; }
        public string? OccupationMother { get; set; }
        public string? PhoneNumberMother { get; set; }
        public string? EmailMother { get; set; }
        public string? IdcardNumberMother { get; set; }

        // Thông tin người bảo hộ (nếu có)
        public string? FullNameGuardian { get; set; }
        public DateOnly? YearOfBirthGuardian { get; set; }
        public string? OccupationGuardian { get; set; }
        public string? PhoneNumberGuardian { get; set; }
        public string? EmailGuardian { get; set; }
        public string? IdcardNumberGuardian { get; set; }
    }
}