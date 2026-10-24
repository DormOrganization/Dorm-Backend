using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class Nationality
{
    public int NationalityId { get; set; }

    public string Country { get; set; } = null!;

    public string Nationality1 { get; set; } = null!;

    public virtual ICollection<Person> People { get; set; } = new List<Person>();
}
