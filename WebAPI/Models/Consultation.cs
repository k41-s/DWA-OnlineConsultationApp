using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Consultation
{
    public int Id { get; set; }

    public int MentorId { get; set; }

    public int UserId { get; set; }

    public DateTime RequestedAt { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual Mentor Mentor { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
