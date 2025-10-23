using System;
using System.Collections.Generic;

namespace LoccarWebapi.ORM.model;

public partial class Reservation
{
    public int Reservationnumber { get; set; }

    public int Idcustomer { get; set; }

    public int Idvehicle { get; set; }

    public DateTime RentalDate { get; set; }

    public DateTime ReturnDate { get; set; }

    public int? RentalDays { get; set; }

    public decimal? DailyRate { get; set; }

    public string? RateType { get; set; }

    public decimal? InsuranceVehicle { get; set; }

    public decimal? InsuranceThirdParty { get; set; }

    public decimal? TaxAmount { get; set; }

    public virtual Customer IdcustomerNavigation { get; set; } = null!;

    public virtual Vehicle IdvehicleNavigation { get; set; } = null!;
}
