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

    public virtual ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();

    public virtual User? User { get; set; }
}
