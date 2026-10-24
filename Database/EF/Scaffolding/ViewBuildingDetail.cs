using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class ViewBuildingDetail
{
    public int BuildingId { get; set; }

    public string BuildingName { get; set; } = null!;

    public int? TotalFloors { get; set; }

    public int? TotalRooms { get; set; }

    public int? TotalCapacity { get; set; }

    public int? Available { get; set; }

    public int? Occupied { get; set; }
}
