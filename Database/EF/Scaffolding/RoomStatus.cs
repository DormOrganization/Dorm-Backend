using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class RoomStatus
{
    public int RoomStatusId { get; set; }

    public string? RoomStatus1 { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
