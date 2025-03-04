using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Reward
{
    public int RewardId { get; set; }

    public int TeacherId { get; set; }

    public string? RewardType { get; set; }

    public DateOnly? DateReceived { get; set; }

    public virtual Teacher Teacher { get; set; } = null!;
}
