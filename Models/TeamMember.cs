using System;
using System.Collections.Generic;

namespace LinkShorterAPI.Models;

public partial class TeamMember
{
    public Guid Id { get; set; }

    public Guid TeamId { get; set; }

    public Guid UserId { get; set; }

    public string Role { get; set; } = null!;

    public DateTime JoinedAt { get; set; }

    public virtual Team Team { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
