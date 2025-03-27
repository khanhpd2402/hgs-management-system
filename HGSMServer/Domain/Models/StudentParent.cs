using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class StudentParent
{
    public int StudentId { get; set; }

    public int ParentId { get; set; }

    public string Relationship { get; set; } = null!;

    public virtual Parent Parent { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
