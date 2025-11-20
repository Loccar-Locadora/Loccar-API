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

        // Métodos para atualizar tipos específicos de veículos
        public async Task<CargoVehicle> UpdateCargoVehicle(CargoVehicle cargoVehicle)
        {
            var existing = await _dbContext.CargoVehicles.FirstOrDefaultAsync(c => c.IdVehicle == cargoVehicle.IdVehicle);
            if (existing == null)
            {
                // Se não existe, criar novo
                await _dbContext.CargoVehicles.AddAsync(cargoVehicle);
            }
            else
            {
                // Atualizar existente
                existing.CargoCapacity = cargoVehicle.CargoCapacity;
                existing.CargoCompartmentSize = cargoVehicle.CargoCompartmentSize;
                existing.CargoType = cargoVehicle.CargoType;
                existing.TareWeight = cargoVehicle.TareWeight;
            }

            await _dbContext.SaveChangesAsync();
            return cargoVehicle;
        }

        public async Task<Motorcycle> UpdateMotorcycleVehicle(Motorcycle motorcycle)
        {
            var existing = await _dbContext.Motorcycles.FirstOrDefaultAsync(m => m.IdVehicle == motorcycle.IdVehicle);
            if (existing == null)
            {
                // Se não existe, criar novo
                await _dbContext.Motorcycles.AddAsync(motorcycle);
            }
            else
            {
                // Atualizar existente
                existing.TractionControl = motorcycle.TractionControl;
                existing.AbsBrakes = motorcycle.AbsBrakes;
                existing.CruiseControl = motorcycle.CruiseControl;
            }

            await _dbContext.SaveChangesAsync();
            return motorcycle;
        }

        public async Task<LeisureVehicle> UpdateLeisureVehicle(LeisureVehicle leisureVehicle)
        {
            var existing = await _dbContext.LeisureVehicles.FirstOrDefaultAsync(l => l.IdVehicle == leisureVehicle.IdVehicle);
            if (existing == null)
            {
                // Se não existe, criar novo
                await _dbContext.LeisureVehicles.AddAsync(leisureVehicle);
            }
            else
            {
                // Atualizar existente
                existing.Automatic = leisureVehicle.Automatic;
                existing.PowerSteering = leisureVehicle.PowerSteering;
                existing.AirConditioning = leisureVehicle.AirConditioning;
                existing.Category = leisureVehicle.Category;
            }

            await _dbContext.SaveChangesAsync();
            return leisureVehicle;
        }

        public async Task<PassengerVehicle> UpdatePassengerVehicle(PassengerVehicle passengerVehicle)
        {
            var existing = await _dbContext.PassengerVehicles.FirstOrDefaultAsync(p => p.IdVehicle == passengerVehicle.IdVehicle);
            if (existing == null)
            {
                // Se não existe, criar novo
                await _dbContext.PassengerVehicles.AddAsync(passengerVehicle);
            }
            else
            {
                // Atualizar existente
                existing.PassengerCapacity = passengerVehicle.PassengerCapacity;
                existing.Tv = passengerVehicle.Tv;
                existing.AirConditioning = passengerVehicle.AirConditioning;
                existing.PowerSteering = passengerVehicle.PowerSteering;
            }

            await _dbContext.SaveChangesAsync();
            return passengerVehicle;
        }

        // Métodos para remover tipos específicos de veículos
        public async Task<bool> RemoveCargoVehicle(int vehicleId)
        {
            var cargoVehicle = await _dbContext.CargoVehicles.FirstOrDefaultAsync(c => c.IdVehicle == vehicleId);
            if (cargoVehicle == null)
            {
                return false;
            }

            _dbContext.CargoVehicles.Remove(cargoVehicle);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveMotorcycleVehicle(int vehicleId)
        {
            var motorcycle = await _dbContext.Motorcycles.FirstOrDefaultAsync(m => m.IdVehicle == vehicleId);
            if (motorcycle == null)
            {
                return false;
            }

            _dbContext.Motorcycles.Remove(motorcycle);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveLeisureVehicle(int vehicleId)
        {
            var leisureVehicle = await _dbContext.LeisureVehicles.FirstOrDefaultAsync(l => l.IdVehicle == vehicleId);
            if (leisureVehicle == null)
            {
                return false;
            }

            _dbContext.LeisureVehicles.Remove(leisureVehicle);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemovePassengerVehicle(int vehicleId)
        {
            var passengerVehicle = await _dbContext.PassengerVehicles.FirstOrDefaultAsync(p => p.IdVehicle == vehicleId);
            if (passengerVehicle == null)
            {
                return false;
            }

            _dbContext.PassengerVehicles.Remove(passengerVehicle);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Método para definir status de reserva
        public async Task<bool> SetVehicleReserved(int vehicleId, bool reserved)
        {
            var vehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.IdVehicle == vehicleId);
            if (vehicle == null)
            {
                return false;
            }

            vehicle.Reserved = reserved;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
