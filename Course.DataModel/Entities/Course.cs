using System;
using System.Collections.Generic;

namespace Course.DataModel.Entities;

public partial class Course
{
    public int Id { get; set; }

    public string Coursename { get; set; } = null!;

    public string? Content { get; set; }

    public int Credits { get; set; }

    public string Department { get; set; } = null!;

    public string? ImagePath { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
