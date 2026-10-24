using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class Faculty
{
    public int FacultyId { get; set; }

    public string FacultyName { get; set; } = null!;

    public string? FacultyCode { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
