using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class Contact
{
    public int ContactId { get; set; }

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Address { get; set; }

    public string? EmergencyContactName { get; set; }

    public string? EmergencyContactPhone { get; set; }

    public int? RelationshipTypeId { get; set; }

    public virtual ICollection<Person> People { get; set; } = new List<Person>();

    public virtual RelationshipType? RelationshipType { get; set; }
}
