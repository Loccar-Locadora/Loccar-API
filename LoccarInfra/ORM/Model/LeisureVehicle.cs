using System;
using System.Collections.Generic;

namespace LoccarInfra.ORM.model;

public partial class LeisureVehicle
{
    public int Idvehicle { get; set; }

    public bool? Automatic { get; set; }

    public bool? PowerSteering { get; set; }

    public bool? AirConditioning { get; set; }

    public string? Category { get; set; }

    public virtual Vehicle IdvehicleNavigation { get; set; } = null!;
}
