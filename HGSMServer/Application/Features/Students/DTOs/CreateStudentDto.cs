using Application.Features.Students.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Students.DTOs
{
    public class CreateStudentDto : IParentInfoDto
    {
        // Thông tin học sinh
        [Required(ErrorMessage = "FullName is required.")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Dob is required.")]
        public DateOnly Dob { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; } = null!;

        [Required(ErrorMessage = "ClassId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ClassId must be greater than 0.")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "AdmissionDate is required.")]
        public DateOnly AdmissionDate { get; set; }

        public string? EnrollmentType { get; set; }
        public string? Ethnicity { get; set; }
        public string? PermanentAddress { get; set; }
        public string? BirthPlace { get; set; }
        public string? Religion { get; set; }

        [Required(ErrorMessage = "RepeatingYear is required.")]
        public bool? RepeatingYear { get; set; }

        public string? IdcardNumber { get; set; }
        public string? Status { get; set; }
        public int? ParentId { get; set; }

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