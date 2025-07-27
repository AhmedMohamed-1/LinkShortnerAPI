using System;
using System.Collections.Generic;

namespace LinkShorterAPI.Models;

public partial class ClickUtm
{
    public long Id { get; set; }

    public long ClickId { get; set; }

    public string? UtmSource { get; set; }

    public string? UtmMedium { get; set; }

    public string? UtmCampaign { get; set; }

    public string? UtmTerm { get; set; }

    public string? UtmContent { get; set; }

    public virtual Click Click { get; set; } = null!;
}
