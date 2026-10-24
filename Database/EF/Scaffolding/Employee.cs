using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public int EmployeeTypeId { get; set; }

    public int PersonId { get; set; }

    public virtual ICollection<EmployeeLeaf> EmployeeLeaves { get; set; } = new List<EmployeeLeaf>();

    public virtual EmployeeType EmployeeType { get; set; } = null!;

    public virtual ICollection<EmployeesContract> EmployeesContracts { get; set; } = new List<EmployeesContract>();

    public virtual Person Person { get; set; } = null!;

    public virtual ICollection<WorkingDay> WorkingDays { get; set; } = new List<WorkingDay>();
}
