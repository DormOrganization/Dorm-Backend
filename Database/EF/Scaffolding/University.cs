using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class University
{
    public int UniversityId { get; set; }

    public string UniversityName { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
