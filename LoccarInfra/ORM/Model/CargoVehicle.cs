using System;
using System.Collections.Generic;

namespace LoccarInfra.ORM.model;

public partial class CargoVehicle
{
    public int IdVehicle { get; set; }

    public decimal? CargoCapacity { get; set; }

    public string? CargoType { get; set; }

    public decimal? TareWeight { get; set; }

    public string? CargoCompartmentSize { get; set; }

    public virtual Vehicle IdVehicleNavigation { get; set; } = null!;
}
