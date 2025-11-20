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

        // Novos métodos CRUD
        Task<Vehicle> GetVehicleById(int vehicleId);

        Task<List<Vehicle>> ListAllVehicles();

        Task<Vehicle> UpdateVehicle(Vehicle vehicle);

        Task<bool> DeleteVehicle(int vehicleId);

        // Métodos de estatísticas
        Task<int> GetTotalVehiclesCount();
        Task<int> GetAvailableVehiclesCount();

        // Métodos para atualizar tipos específicos de veículos
        Task<CargoVehicle> UpdateCargoVehicle(CargoVehicle cargoVehicle);
        Task<Motorcycle> UpdateMotorcycleVehicle(Motorcycle motorcycle);
        Task<LeisureVehicle> UpdateLeisureVehicle(LeisureVehicle leisureVehicle);
        Task<PassengerVehicle> UpdatePassengerVehicle(PassengerVehicle passengerVehicle);

        // Métodos para remover tipos específicos de veículos
        Task<bool> RemoveCargoVehicle(int vehicleId);
        Task<bool> RemoveMotorcycleVehicle(int vehicleId);
        Task<bool> RemoveLeisureVehicle(int vehicleId);
        Task<bool> RemovePassengerVehicle(int vehicleId);

        // Método para definir status de reserva
        Task<bool> SetVehicleReserved(int vehicleId, bool reserved);
    }
}
