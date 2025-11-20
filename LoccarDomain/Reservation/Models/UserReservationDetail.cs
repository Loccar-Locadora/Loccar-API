namespace LoccarDomain.Reservation.Models;

public class UserReservationDetail
{
    public int Reservationnumber { get; set; }
    public int IdVehicle { get; set; }
    public string VehicleBrand { get; set; } = string.Empty;
    public string VehicleModel { get; set; } = string.Empty;
    public DateTime RentalDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public int? RentalDays { get; set; }
    public decimal? DailyRate { get; set; }
    public string? RateType { get; set; }
    public decimal? InsuranceVehicle { get; set; }
    public decimal? InsuranceThirdParty { get; set; }
    public decimal? TaxAmount { get; set; }
    public string? DamageDescription { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalCost { get; set; }
    public string? ImgUrl { get; set; }
}
