using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Parent
{
    public int ParentId { get; set; }

    public int? UserId { get; set; }

    public string? FullNameFather { get; set; }

    public DateOnly? YearOfBirthFather { get; set; }

    public string? OccupationFather { get; set; }

    public string? PhoneNumberFather { get; set; }

    public string? EmailFather { get; set; }

    public string? IdcardNumberFather { get; set; }

    public string? FullNameMother { get; set; }

    public DateOnly? YearOfBirthMother { get; set; }

    public string? OccupationMother { get; set; }

    public string? PhoneNumberMother { get; set; }

    public string? EmailMother { get; set; }

    public string? IdcardNumberMother { get; set; }

    public string? FullNameGuardian { get; set; }

    public DateOnly? YearOfBirthGuardian { get; set; }

    public string? OccupationGuardian { get; set; }

    public string? PhoneNumberGuardian { get; set; }

    public string? EmailGuardian { get; set; }

    public string? IdcardNumberGuardian { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual User? User { get; set; }
}
