using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class ViewEmployeesContract
{
    public int EmployeeContractId { get; set; }

    public int EmployeeId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? ShiftId { get; set; }

    public double? Salary { get; set; }

    public double? WorkingHours { get; set; }

    public int? WorkingDaysId { get; set; }

    public bool? Monday { get; set; }

    public bool? Tuesday { get; set; }

    public bool? Wednesday { get; set; }

    public bool? Thursday { get; set; }

    public bool? Friday { get; set; }

    public bool? Saturday { get; set; }

    public bool? Sunday { get; set; }

    public int? AssignedBuildingId { get; set; }
}
