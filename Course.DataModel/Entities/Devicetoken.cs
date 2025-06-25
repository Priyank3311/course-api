using System;
using System.Collections.Generic;

namespace Course.DataModel.Entities;

public partial class Devicetoken
{
    public int Id { get; set; }

    public int? Userid { get; set; }

    public string? Token { get; set; }

    public DateTime? Createdat { get; set; }

    public virtual User? User { get; set; }
}
