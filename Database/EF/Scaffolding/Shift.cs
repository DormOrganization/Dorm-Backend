using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class Shift
{
    public int ShiftId { get; set; }

    public string? ShiftName { get; set; }

    public virtual ICollection<EmployeesContract> EmployeesContracts { get; set; } = new List<EmployeesContract>();
}
