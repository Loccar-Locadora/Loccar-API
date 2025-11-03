using LoccarDomain.Vehicle.Models;

namespace LoccarDomain.Vehicle.Models
{
    public class VehicleListResponse
    {
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public int TotalVehicles { get; set; }
        public int AvailableVehicles { get; set; }
        public int ReservedVehicles { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
