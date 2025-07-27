using System;
using System.Collections.Generic;

namespace LinkShorterAPI.Models;

public partial class SecurityLog
{
    public long Id { get; set; }

    public Guid? UserId { get; set; }

    public string? EventType { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
}
