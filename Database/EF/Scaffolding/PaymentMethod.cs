using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string? PaymentMethodName { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<RentInstallmentsLedger> RentInstallmentsLedgers { get; set; } = new List<RentInstallmentsLedger>();
}
