using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Period
{
    public int PeriodId { get; set; }

    public string PeriodName { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public byte Shift { get; set; }

    public virtual ICollection<TimetableDetail> TimetableDetails { get; set; } = new List<TimetableDetail>();
}
