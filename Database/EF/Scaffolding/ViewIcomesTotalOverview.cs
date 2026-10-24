using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class ViewIcomesTotalOverview
{
    public int IncomeId { get; set; }

    public int IncomeCategoryId { get; set; }

    public decimal IncomeAmount { get; set; }

    public DateOnly? IncomeDate { get; set; }

    public string CategoryName { get; set; } = null!;
}
