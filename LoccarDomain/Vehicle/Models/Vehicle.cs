namespace LoccarDomain.Vehicle.Models
{
    public class Vehicle
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

        public VehicleType Type { get; set; }

        public CargoVehicle? CargoVehicle { get; set; }
        public Motorcycle? Motorcycle { get; set; }
        public PassengerVehicle? PassengerVehicle { get; set; }
        public LeisureVehicle? LeisureVehicle { get; set; }
    }

    public enum VehicleType
    {
        Cargo = 0,
        Motorcycle = 1,
        Passenger = 2,
        Leisure = 3
    }
}
