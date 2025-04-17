using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Conduct
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int SemesterId { get; set; }

    public string ConductType { get; set; } = null!;

    public string? Note { get; set; }

    public virtual Semester Semester { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
