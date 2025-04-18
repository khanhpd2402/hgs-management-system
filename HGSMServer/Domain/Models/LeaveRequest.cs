﻿using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class LeaveRequest
{
    public int RequestId { get; set; }

    public int TeacherId { get; set; }

    public DateOnly RequestDate { get; set; }

    public DateOnly LeaveFromDate { get; set; }

    public DateOnly LeaveToDate { get; set; }

    public string? Reason { get; set; }

    public string? Comment { get; set; }

    public string? Status { get; set; }

    public virtual Teacher Teacher { get; set; } = null!;
}
