using System;
using System.Collections.Generic;

namespace Course.DataModel.Entities;

public partial class Enrollment
{
    public int Id { get; set; }

    public int Userid { get; set; }

    public int Courseid { get; set; }

    public bool? Iscompleted { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
