using System;
using System.Collections.Generic;

namespace LinkShorterAPI.Models;

public partial class ActivityLog
{
    public long Id { get; set; }

    public Guid? UserId { get; set; }

    public Guid? TeamId { get; set; }

    public string? Action { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
}
