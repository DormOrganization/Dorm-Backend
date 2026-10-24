using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class UserRole
{
    public int UserRoleId { get; set; }

    public string UserRoleName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
