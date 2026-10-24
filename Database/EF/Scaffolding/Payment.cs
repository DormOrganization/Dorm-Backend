using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? RentInstallmentId { get; set; }

    public int? PaymentMethodId { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public string? Notes { get; set; }

    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual RentInstallmentsLedger? RentInstallment { get; set; }
}
