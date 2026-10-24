using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class WorkingDay
{
    public int WorkingDaysId { get; set; }

    public int? EmployeeId { get; set; }

    public bool? Monday { get; set; }

    public bool? Tuesday { get; set; }

    public bool? Wednesday { get; set; }

    public bool? Thursday { get; set; }

    public bool? Friday { get; set; }

    public bool? Sunday { get; set; }

    public bool? Saturday { get; set; }

    public virtual Employee? Employee { get; set; }
}
