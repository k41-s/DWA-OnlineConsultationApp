using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public virtual ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();

    public virtual Mentor? Mentor { get; set; }
}
