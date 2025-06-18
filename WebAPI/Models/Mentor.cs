using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Mentor
{
    public int Id { get; set; }

    public int TypeOfWorkId { get; set; }

    public virtual ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();

    public virtual User User { get; set; } = null!;

    public virtual TypeOfWork TypeOfWork { get; set; } = null!;

    public virtual ICollection<Area> Areas { get; set; } = new List<Area>();
}
