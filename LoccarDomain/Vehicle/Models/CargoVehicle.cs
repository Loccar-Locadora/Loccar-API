namespace LoccarDomain.Vehicle.Models
{
    public class CargoVehicle : Vehicle
    {
        public decimal? CargoCapacity { get; set; }

        public string? CargoType { get; set; }

        public decimal? TareWeight { get; set; }

        public string? CargoCompartmentSize { get; set; }
        public int? IdVehicle { get; set; }
    }
}
