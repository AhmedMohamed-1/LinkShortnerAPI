using System;
using System.Collections.Generic;

namespace LinkShorterAPI.Models;

public partial class SystemLog
{
    public long Id { get; set; }

    public string? Source { get; set; }

    public string? Level { get; set; }

    public string? Message { get; set; }

    public string? StackTrace { get; set; }

    public DateTime CreatedAt { get; set; }
}
