using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class ViewEmployee
{
    public int EmployeeId { get; set; }

    public int TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int PersonId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? SecondName { get; set; }

    public string? ThirdName { get; set; }

    public string LastName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public bool Gender { get; set; }

    public string PassportNumber { get; set; } = null!;

    public string IdentityNumber { get; set; } = null!;

    public int? NationalityId { get; set; }

    public int ContactId { get; set; }

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Address { get; set; }

    public string? EmergencyContactName { get; set; }

    public string? EmergencyContactPhone { get; set; }

    public int RelationshipTypeId { get; set; }

    public string RelationshipName { get; set; } = null!;
}
