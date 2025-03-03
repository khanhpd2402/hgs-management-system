using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Parent
{
    public int ParentId { get; set; }

    public int? UserId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly? Dob { get; set; }

    public string? Occupation { get; set; }

    public string Relationship { get; set; } = null!;

    public virtual User? User { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
