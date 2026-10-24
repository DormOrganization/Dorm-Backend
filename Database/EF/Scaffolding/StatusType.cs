using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class StatusType
{
    public int StatusTypeId { get; set; }

    public string StatusTypeName { get; set; } = null!;

    public virtual ICollection<EmployeeLeaf> EmployeeLeaves { get; set; } = new List<EmployeeLeaf>();
}
