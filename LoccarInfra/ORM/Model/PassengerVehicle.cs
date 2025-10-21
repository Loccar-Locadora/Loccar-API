using System;
using System.Collections.Generic;

namespace LoccarInfra.ORM.model;

public partial class PassengerVehicle
{
    public int Idvehicle { get; set; }

    public int? PassengerCapacity { get; set; }

    public bool? Tv { get; set; }

    public bool? AirConditioning { get; set; }

    public bool? PowerSteering { get; set; }

    public virtual Vehicle IdvehicleNavigation { get; set; } = null!;
}
