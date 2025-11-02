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
            var vehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.IdVehicle == vehicleId);
            if (vehicle == null)
            {
                return false;
            }

            vehicle.Reserved = inMaintenance; // ou criar coluna Maintenance
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Novos métodos CRUD
        public async Task<Vehicle> GetVehicleById(int vehicleId)
        {
            return await _dbContext.Vehicles
                .Include(i => i.CargoVehicle)
                .Include(i => i.LeisureVehicle)
                .Include(i => i.PassengerVehicle)
                .Include(i => i.Motorcycle)
                .FirstOrDefaultAsync(v => v.IdVehicle == vehicleId);
        }

        public async Task<List<Vehicle>> ListAllVehicles()
        {
            return await _dbContext.Vehicles
                .Include(i => i.CargoVehicle)
                .Include(i => i.LeisureVehicle)
                .Include(i => i.PassengerVehicle)
                .Include(i => i.Motorcycle)
                .ToListAsync();
        }

        public async Task<Vehicle> UpdateVehicle(Vehicle vehicle)
        {
            var existingVehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.IdVehicle == vehicle.IdVehicle);
            if (existingVehicle == null)
            {
                return null;
            }

            existingVehicle.Brand = vehicle.Brand;
            existingVehicle.Model = vehicle.Model;
            existingVehicle.ManufacturingYear = vehicle.ManufacturingYear;
            existingVehicle.ModelYear = vehicle.ModelYear;
            existingVehicle.DailyRate = vehicle.DailyRate;
            existingVehicle.MonthlyRate = vehicle.MonthlyRate;
            existingVehicle.CompanyDailyRate = vehicle.CompanyDailyRate;
            existingVehicle.ReducedDailyRate = vehicle.ReducedDailyRate;
            existingVehicle.FuelTankCapacity = vehicle.FuelTankCapacity;
            existingVehicle.Vin = vehicle.Vin;
            existingVehicle.Reserved = vehicle.Reserved;

            await _dbContext.SaveChangesAsync();
            return existingVehicle;
        }

        public async Task<bool> DeleteVehicle(int vehicleId)
        {
            var vehicle = await _dbContext.Vehicles
                .Include(v => v.CargoVehicle)
                .Include(v => v.LeisureVehicle)
                .Include(v => v.PassengerVehicle)
                .Include(v => v.Motorcycle)
                .FirstOrDefaultAsync(v => v.IdVehicle == vehicleId);

            if (vehicle == null)
            {
                return false;
            }

            // Remove veículos específicos primeiro (devido às chaves estrangeiras)
            if (vehicle.CargoVehicle != null)
            {
                _dbContext.CargoVehicles.Remove(vehicle.CargoVehicle);
            }

            if (vehicle.LeisureVehicle != null)
            {
                _dbContext.LeisureVehicles.Remove(vehicle.LeisureVehicle);
            }

            if (vehicle.PassengerVehicle != null)
            {
                _dbContext.PassengerVehicles.Remove(vehicle.PassengerVehicle);
            }

            if (vehicle.Motorcycle != null)
            {
                _dbContext.Motorcycles.Remove(vehicle.Motorcycle);
            }

            _dbContext.Vehicles.Remove(vehicle);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Métodos de estatísticas
        public async Task<int> GetTotalVehiclesCount()
        {
            return await _dbContext.Vehicles.CountAsync();
        }

        public async Task<int> GetAvailableVehiclesCount()
        {
            return await _dbContext.Vehicles.CountAsync(v => v.Reserved == false);
        }
    }
}
