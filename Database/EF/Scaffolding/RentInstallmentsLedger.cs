using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class RentInstallmentsLedger
{
    public int RentInstallmentId { get; set; }

    public int ContractId { get; set; }

    public int InstallmentNumber { get; set; }

    public decimal InstallmentAmount { get; set; }

    public DateOnly DueDate { get; set; }

    public DateOnly? PaidDate { get; set; }

    public bool IsPaid { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? PaymentMethodId { get; set; }

    public virtual Contract Contract { get; set; } = null!;

    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
