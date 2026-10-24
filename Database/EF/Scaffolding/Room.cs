using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class Room
{
    public int RoomId { get; set; }

    public string RoomNumber { get; set; } = null!;

    public int FloorId { get; set; }

    public int Capacity { get; set; }

    public int AvailableBeds { get; set; }

    public int? RoomStatusId { get; set; }

    public virtual Floor Floor { get; set; } = null!;

    public virtual ICollection<RoomAllocation> RoomAllocations { get; set; } = new List<RoomAllocation>();

    public virtual RoomStatus? RoomStatus { get; set; }
}
