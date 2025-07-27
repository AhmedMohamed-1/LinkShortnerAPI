using System;
using System.Collections.Generic;

namespace LinkShorterAPI.Models;

public partial class Apikey
{
    public Guid Id { get; set; }

    public Guid TeamId { get; set; }

    public Guid CreatedBy { get; set; }

    public string KeyHash { get; set; } = null!;

    public string? Name { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Team Team { get; set; } = null!;
}
