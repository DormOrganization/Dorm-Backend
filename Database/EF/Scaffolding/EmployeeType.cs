using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class EmployeeType
{
    public int TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
