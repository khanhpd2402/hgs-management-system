using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Students.Interfaces
{
    public interface IParentInfoDto
    {
        string? FullNameFather { get; set; }
        DateOnly? YearOfBirthFather { get; set; }
        string? OccupationFather { get; set; }
        string? PhoneNumberFather { get; set; }
        string? EmailFather { get; set; }
        string? IdcardNumberFather { get; set; }

        string? FullNameMother { get; set; }
        DateOnly? YearOfBirthMother { get; set; }
        string? OccupationMother { get; set; }
        string? PhoneNumberMother { get; set; }
        string? EmailMother { get; set; }
        string? IdcardNumberMother { get; set; }

        string? FullNameGuardian { get; set; }
        DateOnly? YearOfBirthGuardian { get; set; }
        string? OccupationGuardian { get; set; }
        string? PhoneNumberGuardian { get; set; }
        string? EmailGuardian { get; set; }
        string? IdcardNumberGuardian { get; set; }
    }
}
