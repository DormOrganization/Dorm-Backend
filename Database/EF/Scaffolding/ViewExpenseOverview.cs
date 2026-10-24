using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class ViewExpenseOverview
{
    public int ExpenseId { get; set; }

    public int ExpenseGategoryId { get; set; }

    public string? CategoryName { get; set; }

    public decimal ExpenseAmount { get; set; }

    public DateOnly ExpenseDate { get; set; }

    public DateTime? CreatedAt { get; set; }
}
