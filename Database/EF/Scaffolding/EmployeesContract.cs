using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class EmployeesContract
{
    public int EmployeeContractId { get; set; }

    public int? EmployeeId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? ShiftId { get; set; }

    public double? Salary { get; set; }

    public double? WorkingHours { get; set; }

    public int? WorkingDays { get; set; }

    public int? AssignedBuildingId { get; set; }

    public virtual Building? AssignedBuilding { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Shift? Shift { get; set; }
}
