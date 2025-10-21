using System.Threading.Tasks;
using LoccarInfra.ORM.model;
using LoccarInfra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LoccarInfra.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly DataBaseContext _dbContext;

        public VehicleRepository(DataBaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Vehicle> RegisterVehicle(Vehicle vehicle)
        {
            await _dbContext.Vehicles.AddAsync(vehicle);
            await _dbContext.SaveChangesAsync();
            return vehicle;
        }

        public async Task<CargoVehicle> RegisterCargoVehicle(CargoVehicle cargoVehicle)
        {
            await _dbContext.CargoVehicles.AddAsync(cargoVehicle);
            await _dbContext.SaveChangesAsync();
            return cargoVehicle;
        }

        public async Task<Motorcycle> RegisterMotorcycleVehicle(Motorcycle motorcycle)
        {
            await _dbContext.Motorcycles.AddAsync(motorcycle);
            await _dbContext.SaveChangesAsync();
            return motorcycle;
        }

        public async Task<LeisureVehicle> RegisterLeisureVehicle(LeisureVehicle leisureVehicle)
        {
            await _dbContext.LeisureVehicles.AddAsync(leisureVehicle);
            await _dbContext.SaveChangesAsync();
            return leisureVehicle;
        }

        public async Task<PassengerVehicle> RegisterPassengerVehicle(PassengerVehicle passengerVehicle)
        {
            await _dbContext.PassengerVehicles.AddAsync(passengerVehicle);
            await _dbContext.SaveChangesAsync();
            return passengerVehicle;
        }

        public async Task<List<Vehicle>> ListAvailableVehicles()
        {
            return await _dbContext.Vehicles
                .Include(i => i.CargoVehicle)
                .Include(i => i.LeisureVehicle)
                .Include(i => i.PassengerVehicle)
                .Include(i => i.Motorcycle)
                .Where(n => n.Reserved == false).ToListAsync();
        }

        public async Task<bool> SetVehicleMaintenance(int vehicleId, bool inMaintenance)
        {
            var vehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.Idvehicle == vehicleId);
            if (vehicle == null) return false;

            vehicle.Reserved = inMaintenance; // ou criar coluna Maintenance
            await _dbContext.SaveChangesAsync();
            return true;
        }


    }
}
