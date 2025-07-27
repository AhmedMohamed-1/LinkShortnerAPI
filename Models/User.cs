using System;
using System.Collections.Generic;

namespace LinkShorterAPI.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Apikey> Apikeys { get; set; } = new List<Apikey>();

    public virtual ICollection<ShortLink> ShortLinks { get; set; } = new List<ShortLink>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
