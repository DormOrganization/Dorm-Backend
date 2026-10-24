using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class ViewPaymentsIcomeOverview
{
    public int PaymentId { get; set; }

    public int IncomeCategoryId { get; set; }

    public decimal InstallmentAmount { get; set; }

    public DateOnly? IncomeDate { get; set; }
}
