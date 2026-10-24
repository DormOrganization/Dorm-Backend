using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class User
{
    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public int PersonId { get; set; }

    public int UserRoleId { get; set; }

    public virtual Person Person { get; set; } = null!;

    public virtual UserRole UserRole { get; set; } = null!;
}
