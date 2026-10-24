using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class Person
{
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

    public int? ContactId { get; set; }

    public virtual Contact? Contact { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual Nationality? Nationality { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
