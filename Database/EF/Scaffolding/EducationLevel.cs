using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class EducationLevel
{
    public int LevelId { get; set; }

    public string? LevelName { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
