using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class StudentDeposit
{
    public int DepositId { get; set; }

    public int StudentId { get; set; }

    public decimal DepositAmount { get; set; }

    public DateOnly DepositDate { get; set; }

    public bool? IsRefunded { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Student Student { get; set; } = null!;
}
