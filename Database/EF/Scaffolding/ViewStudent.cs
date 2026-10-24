using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class ViewStudent
{
    public int StudentId { get; set; }

    public string StudentNumber { get; set; } = null!;

    public DateOnly StartedDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int YearOfStudy { get; set; }

    public int UniversityId { get; set; }

    public string UniversityName { get; set; } = null!;

    public int FacultyId { get; set; }

    public string FacultyName { get; set; } = null!;

    public int DepartmentId { get; set; }

    public string? DepartmentName { get; set; }

    public int? EducationLevelId { get; set; }

    public string? LevelName { get; set; }

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

    public string Nationality { get; set; } = null!;

    public int ContactId { get; set; }

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Address { get; set; }

    public string? EmergencyContactName { get; set; }

    public string? EmergencyContactPhone { get; set; }

    public int? RelationshipTypeId { get; set; }

    public string RelationshipName { get; set; } = null!;
}
