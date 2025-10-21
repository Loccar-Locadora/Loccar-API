using System.Threading.Tasks;
using LoccarInfra.ORM.model;

namespace LoccarInfra.Repositories.Interfaces
{
    public interface IVehicleRepository
    {
        Task<Vehicle> RegisterVehicle(Vehicle vehicle);
        Task<CargoVehicle> RegisterCargoVehicle(CargoVehicle cargoVehicle);
        Task<Motorcycle> RegisterMotorcycleVehicle(Motorcycle motorcycle);
        Task<LeisureVehicle> RegisterLeisureVehicle(LeisureVehicle leisureVehicle);
        Task<PassengerVehicle> RegisterPassengerVehicle(PassengerVehicle passengerVehicle);
        Task<List<Vehicle>> ListAvailableVehicles();
        Task<bool> SetVehicleMaintenance(int vehicleId, bool inMaintenance);
    }
}
