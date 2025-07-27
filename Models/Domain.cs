using System;
using System.Collections.Generic;

namespace LinkShorterAPI.Models;

public partial class Domain
{
    public Guid Id { get; set; }

    public Guid? TeamId { get; set; }

    public string DomainName { get; set; } = null!;

    public bool? IsVerified { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ShortLink> ShortLinks { get; set; } = new List<ShortLink>();

    public virtual Team? Team { get; set; }
}
