using System;
using System.Collections.Generic;

namespace LoccarInfra.ORM.model;

public partial class Motorcycle
{
    public int IdVehicle { get; set; }

    public bool? TractionControl { get; set; }

    public bool? AbsBrakes { get; set; }

    public bool? CruiseControl { get; set; }

    public virtual Vehicle IdVehicleNavigation { get; set; } = null!;
}
