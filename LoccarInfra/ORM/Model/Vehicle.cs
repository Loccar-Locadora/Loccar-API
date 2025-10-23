using System;
using System.Collections.Generic;

namespace LoccarInfra.ORM.model;

public partial class Vehicle
{
    public int Idvehicle { get; set; }

    public string? Brand { get; set; }

    public string? Model { get; set; }

    public int? ManufacturingYear { get; set; }

    public int? ModelYear { get; set; }

    public string? Vin { get; set; }

    public int? FuelTankCapacity { get; set; }

    public decimal? DailyRate { get; set; }

    public decimal? ReducedDailyRate { get; set; }

    public decimal? MonthlyRate { get; set; }

    public decimal? CompanyDailyRate { get; set; }

    public bool? Reserved { get; set; }

    public virtual CargoVehicle? CargoVehicle { get; set; }

    public virtual LeisureVehicle? LeisureVehicle { get; set; }

    public virtual Motorcycle? Motorcycle { get; set; }

    public virtual PassengerVehicle? PassengerVehicle { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
