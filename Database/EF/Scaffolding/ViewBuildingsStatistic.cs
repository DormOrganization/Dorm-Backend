using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class ViewBuildingsStatistic
{
    public int? TotalBuildings { get; set; }

    public int? TotalFloors { get; set; }

    public int? TotalRooms { get; set; }

    public int? TotalBeds { get; set; }

    public int? OccupiedBeds { get; set; }

    public int? AvailableBeds { get; set; }
}
