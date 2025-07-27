using System;
using System.Collections.Generic;

namespace LinkShorterAPI.Models;

public partial class AuditLog
{
    public long Id { get; set; }

    public Guid? PerformedBy { get; set; }

    public string? EntityType { get; set; }

    public string? EntityId { get; set; }

    public string? Action { get; set; }

    public string? Changes { get; set; }

    public DateTime CreatedAt { get; set; }
}
