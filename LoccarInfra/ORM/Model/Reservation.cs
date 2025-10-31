using System;
using System.Collections.Generic;

namespace LoccarInfra.ORM.model;

public partial class Reservation
{
    public int Reservationnumber { get; set; }

    public int IdCustomer { get; set; }

    public int IdVehicle { get; set; }

    public DateTime RentalDate { get; set; }

    public DateTime ReturnDate { get; set; }

    public int? RentalDays { get; set; }

    public decimal? DailyRate { get; set; }

    public string? RateType { get; set; }

    public decimal? InsuranceVehicle { get; set; }

    public decimal? InsuranceThirdParty { get; set; }

    public decimal? TaxAmount { get; set; }

    public string? DamageDescription { get; set; }

    public virtual Customer IdCustomerNavigation { get; set; } = null!;

    public virtual Vehicle IdVehicleNavigation { get; set; } = null!;
}
