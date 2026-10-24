using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class LeaveType
{
    public int LeaveTypeId { get; set; }

    public string LeaveTypeName { get; set; } = null!;

    public virtual ICollection<EmployeeLeaf> EmployeeLeaves { get; set; } = new List<EmployeeLeaf>();
}
