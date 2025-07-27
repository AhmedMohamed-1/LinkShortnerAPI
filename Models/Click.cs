using System;
using System.Collections.Generic;

namespace LinkShorterAPI.Models;

public partial class Click
{
    public long Id { get; set; }

    public Guid ShortLinkId { get; set; }

    public DateTime ClickedAt { get; set; }

    public string? IpAddress { get; set; }

    public string? CountryCode { get; set; }

    public string? City { get; set; }

    public string? Device { get; set; }

    public string? Os { get; set; }

    public string? Browser { get; set; }

    public string? Referrer { get; set; }

    public bool? IsBot { get; set; }

    public string? UserAgent { get; set; }

    public virtual ICollection<ClickUtm> ClickUtms { get; set; } = new List<ClickUtm>();

    public virtual ShortLink ShortLink { get; set; } = null!;
}
