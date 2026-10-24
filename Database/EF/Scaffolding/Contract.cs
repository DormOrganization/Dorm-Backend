using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class Contract
{
    public int ContractId { get; set; }

    public string ContractName { get; set; } = null!;

    public DateOnly ContractStartDate { get; set; }

    public DateOnly ContractEndDate { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal AdditionalFees { get; set; }

    public decimal Discount { get; set; }

    public decimal MonthlyAmount { get; set; }

    public int NumberOfInstallments { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public int StudentId { get; set; }

    public virtual ICollection<RentInstallmentsLedger> RentInstallmentsLedgers { get; set; } = new List<RentInstallmentsLedger>();

    public virtual Student Student { get; set; } = null!;
}
