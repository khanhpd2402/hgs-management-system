using Application.Features.Subjects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Teachers.DTOs
{
    public class TeacherDetailDto
    {
        public int TeacherId { get; set; }
        //public int? UserId { get; set; }
        public string FullName { get; set; } = null!;
        public DateOnly Dob { get; set; }
        public string Gender { get; set; } = null!;
        public string? Ethnicity { get; set; }
        public string? Religion { get; set; }
        public string? MaritalStatus { get; set; }
        public string? IdcardNumber { get; set; }
        public string? InsuranceNumber { get; set; }
        public string? EmploymentType { get; set; }
        public string? Position { get; set; }
        public string? Department { get; set; }
        public bool? IsHeadOfDepartment { get; set; }
        public string? EmploymentStatus { get; set; }
        public string? RecruitmentAgency { get; set; }
        public DateOnly? HiringDate { get; set; }
        public DateOnly? PermanentEmploymentDate { get; set; }
        public DateOnly SchoolJoinDate { get; set; }
        public string? PermanentAddress { get; set; }
        public string? Hometown { get; set; }
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public List<SubjectTeacherDto> Subjects { get; set; } = new List<SubjectTeacherDto>();
    }

}