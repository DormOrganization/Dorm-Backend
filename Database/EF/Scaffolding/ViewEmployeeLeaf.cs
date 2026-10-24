using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class ViewEmployeeLeaf
{
    public int LeaveId { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = null!;

    public int LeaveTypeId { get; set; }

    public string LeaveTypeName { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? Reason { get; set; }

    public int? StatusId { get; set; }

    public string StatusTypeName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
