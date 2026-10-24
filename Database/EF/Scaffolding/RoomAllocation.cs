using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class RoomAllocation
{
    public int RoomAllocationId { get; set; }

    public int? RoomId { get; set; }

    public int? BedNumber { get; set; }

    public int? StudentId { get; set; }

    public virtual Room? Room { get; set; }

    public virtual Student? Student { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
