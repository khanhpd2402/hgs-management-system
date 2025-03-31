using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Timetable
{
    public int TimetableId { get; set; }

    public int SemesterId { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public virtual Semester Semester { get; set; } = null!;

    public virtual ICollection<TimetableDetail> TimetableDetails { get; set; } = new List<TimetableDetail>();
}
