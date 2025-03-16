using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime? CreateDate { get; set; }

    public bool? IsActive { get; set; }
}
