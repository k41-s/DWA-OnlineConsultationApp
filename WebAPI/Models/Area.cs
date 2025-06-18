using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Area
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Mentor> Mentors { get; set; } = new List<Mentor>();
}
