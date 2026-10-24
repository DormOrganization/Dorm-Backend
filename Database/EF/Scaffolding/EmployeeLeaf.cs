using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class EmployeeLeaf
{
    public int LeaveId { get; set; }

    public int EmployeeId { get; set; }

    public int LeaveTypeId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? Reason { get; set; }

    public int? StatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual LeaveType LeaveType { get; set; } = null!;

    public virtual StatusType? Status { get; set; }
}
