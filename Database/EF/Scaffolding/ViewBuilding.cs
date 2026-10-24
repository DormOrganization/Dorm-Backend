using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class ViewBuilding
{
    public int BuildingId { get; set; }

    public string BuildingName { get; set; } = null!;

    public int? FloorCount { get; set; }

    public int? RoomCount { get; set; }
}
