using System;
using System.Collections.Generic;

namespace LinkShorterAPI.Models;

public partial class ShortLink
{
    public Guid Id { get; set; }

    public Guid? TeamId { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid DomainId { get; set; }

    public string Slug { get; set; } = null!;

    public string DestinationUrl { get; set; } = null!;

    public string? Title { get; set; }

    public bool? IsActive { get; set; }

    public int? ClickLimit { get; set; }

    public int? ClickCount { get; set; }

    public DateTime? ExpireAt { get; set; }

    public string? PasswordHash { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastAccessedAt { get; set; }

    public virtual ICollection<Click> Clicks { get; set; } = new List<Click>();

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Domain Domain { get; set; } = null!;

    public virtual Team? Team { get; set; }
}
