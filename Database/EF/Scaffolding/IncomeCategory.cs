using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class IncomeCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();
}
