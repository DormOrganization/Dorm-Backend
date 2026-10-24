using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class Building
{
    public int BuildingId { get; set; }

    public string BuildingName { get; set; } = null!;

    public virtual ICollection<EmployeesContract> EmployeesContracts { get; set; } = new List<EmployeesContract>();

    public virtual ICollection<Floor> Floors { get; set; } = new List<Floor>();
}
