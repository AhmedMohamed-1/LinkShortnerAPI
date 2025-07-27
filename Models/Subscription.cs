using System;
using System.Collections.Generic;

namespace LinkShorterAPI.Models;

public partial class Subscription
{
    public Guid Id { get; set; }

    public Guid TeamId { get; set; }

    public string SubscriptionPlan { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime StartedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public string? BillingEmail { get; set; }

    public virtual Team Team { get; set; } = null!;
}
