using System;
using System.Collections.Generic;

namespace LinkShorterAPI.Models;

public partial class Team
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid OwnerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Apikey> Apikeys { get; set; } = new List<Apikey>();

    public virtual ICollection<Domain> Domains { get; set; } = new List<Domain>();

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<ShortLink> ShortLinks { get; set; } = new List<ShortLink>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
