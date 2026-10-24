using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class Student
{
    public int StudentId { get; set; }

    public string StudentNumber { get; set; } = null!;

    public DateOnly StartedDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int YearOfStudy { get; set; }

    public int PersonId { get; set; }

    public int UniversityId { get; set; }

    public int FacultyId { get; set; }

    public int DepartmentId { get; set; }

    public int? EducationLevelId { get; set; }

    public int? RoomAllocationId { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<DocumentsPath> DocumentsPaths { get; set; } = new List<DocumentsPath>();

    public virtual EducationLevel? EducationLevel { get; set; }

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual Person Person { get; set; } = null!;

    public virtual RoomAllocation? RoomAllocation { get; set; }

    public virtual ICollection<RoomAllocation> RoomAllocations { get; set; } = new List<RoomAllocation>();

    public virtual ICollection<StudentDeposit> StudentDeposits { get; set; } = new List<StudentDeposit>();

    public virtual University University { get; set; } = null!;
}
