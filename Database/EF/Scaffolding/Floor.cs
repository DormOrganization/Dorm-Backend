using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class Floor
{
    public int FloorId { get; set; }

    public int FloorNumber { get; set; }

    public int BuildingId { get; set; }

    public virtual Building Building { get; set; } = null!;

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
