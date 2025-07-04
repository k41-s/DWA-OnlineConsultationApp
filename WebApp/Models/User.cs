﻿using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Surname { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
}
