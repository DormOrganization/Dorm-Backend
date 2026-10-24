using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class RelationshipType
{
    public int RelationshipTypeId { get; set; }

    public string RelationshipName { get; set; } = null!;

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
