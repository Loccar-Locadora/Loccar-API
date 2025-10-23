namespace LoccarDomain.Vehicle.Models
{
    public class LeisureVehicle : Vehicle
    {
        public int? IdVehicle { get; set; }

        public bool? Automatic { get; set; }

        public bool? PowerSteering { get; set; }

        public bool? AirConditioning { get; set; }

        public string? Category { get; set; }
    }
}
