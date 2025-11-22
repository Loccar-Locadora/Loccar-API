using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoccarApplication.Interfaces;
using LoccarDomain;
using LoccarDomain.Customer.Models;
using LoccarDomain.LoggedUser.Models;
using LoccarDomain.Vehicle.Models;
using LoccarInfra.Repositories.Interfaces;

namespace LoccarApplication
{
    public class VehicleApplication : IVehicleApplication
    {
        private readonly IAuthApplication _authApplication;
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleApplication(IAuthApplication authApplication, IVehicleRepository vehicleRepository)
        {
            _authApplication = authApplication;
            _vehicleRepository = vehicleRepository;
        }

        public async Task<BaseReturn<Vehicle>> RegisterVehicle(Vehicle vehicle)
        {
            BaseReturn<Vehicle> baseReturn = new BaseReturn<Vehicle>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();

                // Fixed authorization logic - user must have ADMIN or EMPLOYEE role
                if (loggedUser?.Roles == null || 
                    (!loggedUser.Roles.Contains("CLIENT_ADMIN") && !loggedUser.Roles.Contains("CLIENT_EMPLOYEE")))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    return baseReturn;
                }

                LoccarInfra.ORM.model.Vehicle tbVehicle = new LoccarInfra.ORM.model.Vehicle()
                {
                    Brand = vehicle.Brand,
                    Model = vehicle.Model,
                    ManufacturingYear = vehicle.ManufacturingYear,
                    ModelYear = vehicle.ModelYear,
                    DailyRate = vehicle.DailyRate,
                    MonthlyRate = vehicle.MonthlyRate,
                    CompanyDailyRate = vehicle.CompanyDailyRate,
                    ReducedDailyRate = vehicle.ReducedDailyRate,
                    FuelTankCapacity = vehicle.FuelTankCapacity,
                    Vin = vehicle.Vin,
                    Reserved = false,
                    ImgUrl = vehicle.ImgUrl
                };

                await _vehicleRepository.RegisterVehicle(tbVehicle);

                // Fixed vehicle type verification logic
                Vehicle registeredVehicle = null;
                switch (vehicle.Type)
                {
                    case VehicleType.Cargo:
                        if (vehicle.CargoVehicle != null)
                        {
                            vehicle.CargoVehicle.IdVehicle = tbVehicle.IdVehicle;
                            var cargoResult = await RegisterCargoVehicle(vehicle.CargoVehicle);
                            if (cargoResult.Code == "201")
                            {
                                registeredVehicle = vehicle;
                            }
                        }

                        break;
                    case VehicleType.Motorcycle:
                        if (vehicle.Motorcycle != null)
                        {
                            vehicle.Motorcycle.IdVehicle = tbVehicle.IdVehicle;
                            var motorcycleResult = await RegisterMotorcycleVehicle(vehicle.Motorcycle);
                            if (motorcycleResult.Code == "201")
                            {
                                registeredVehicle = vehicle;
                            }
                        }

                        break;
                    case VehicleType.Leisure:
                        if (vehicle.LeisureVehicle != null)
                        {
                            vehicle.LeisureVehicle.IdVehicle = tbVehicle.IdVehicle;
                            var leisureResult = await RegisterLeisureVehicle(vehicle.LeisureVehicle);
                            if (leisureResult.Code == "201")
                            {
                                registeredVehicle = vehicle;
                            }
                        }

                        break;
                    case VehicleType.Passenger:
                        if (vehicle.PassengerVehicle != null)
                        {
                            vehicle.PassengerVehicle.IdVehicle = tbVehicle.IdVehicle;
                            var passengerResult = await RegisterPassengerVehicle(vehicle.PassengerVehicle);
                            if (passengerResult.Code == "201")
                            {
                                registeredVehicle = vehicle;
                            }
                        }

                        break;
                }

                if (registeredVehicle == null)
                {
                    baseReturn.Code = "400";
                    baseReturn.Message = "Unable to register the vehicle";
                    return baseReturn;
                }

                baseReturn.Code = "201";
                baseReturn.Data = registeredVehicle;
                baseReturn.Message = "Vehicle registered successfully";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        private async Task<BaseReturn<CargoVehicle>> RegisterCargoVehicle(CargoVehicle cargoVehicle)
        {
            BaseReturn<CargoVehicle> baseReturn = new BaseReturn<CargoVehicle>();
            try
            {
                LoccarInfra.ORM.model.CargoVehicle tbCargoVehicle = new LoccarInfra.ORM.model.CargoVehicle()
                {
                    CargoCapacity = cargoVehicle.CargoCapacity,
                    CargoCompartmentSize = cargoVehicle.CargoCompartmentSize,
                    CargoType = cargoVehicle.CargoType,
                    TareWeight = cargoVehicle.TareWeight,
                    IdVehicle = cargoVehicle.IdVehicle,
                };

                await _vehicleRepository.RegisterCargoVehicle(tbCargoVehicle);

                baseReturn.Code = "201";
                baseReturn.Message = "Cargo vehicle registered.";
                baseReturn.Data = cargoVehicle;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        private async Task<BaseReturn<LeisureVehicle>> RegisterLeisureVehicle(LeisureVehicle leisureVehicle)
        {
            BaseReturn<LeisureVehicle> baseReturn = new BaseReturn<LeisureVehicle>();
            try
            {
                LoccarInfra.ORM.model.LeisureVehicle tbLeisureVehicle = new LoccarInfra.ORM.model.LeisureVehicle()
                {
                    Automatic = leisureVehicle.Automatic,
                    PowerSteering = leisureVehicle.PowerSteering,
                    AirConditioning = leisureVehicle.AirConditioning,
                    Category = leisureVehicle.Category,
                    IdVehicle = leisureVehicle.IdVehicle,
                };

                await _vehicleRepository.RegisterLeisureVehicle(tbLeisureVehicle);

                baseReturn.Code = "201";
                baseReturn.Message = "Leisure vehicle registered.";
                baseReturn.Data = leisureVehicle;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        private async Task<BaseReturn<Motorcycle>> RegisterMotorcycleVehicle(Motorcycle motorcycle)
        {
            BaseReturn<Motorcycle> baseReturn = new BaseReturn<Motorcycle>();
            try
            {
                LoccarInfra.ORM.model.Motorcycle tbMotorcycle = new LoccarInfra.ORM.model.Motorcycle()
                {
                    TractionControl = motorcycle.TractionControl,
                    AbsBrakes = motorcycle.AbsBrakes,
                    CruiseControl = motorcycle.CruiseControl,
                    IdVehicle = motorcycle.IdVehicle,
                };

                await _vehicleRepository.RegisterMotorcycleVehicle(tbMotorcycle);

                baseReturn.Code = "201";
                baseReturn.Message = "Motorcycle registered.";
                baseReturn.Data = motorcycle;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        private async Task<BaseReturn<PassengerVehicle>> RegisterPassengerVehicle(PassengerVehicle passengerVehicle)
        {
            BaseReturn<PassengerVehicle> baseReturn = new BaseReturn<PassengerVehicle>();
            try
            {
                LoccarInfra.ORM.model.PassengerVehicle tbPassengerVehicle = new LoccarInfra.ORM.model.PassengerVehicle()
                {
                    PassengerCapacity = passengerVehicle.PassengerCapacity,
                    Tv = passengerVehicle.Tv,
                    AirConditioning = passengerVehicle.AirConditioning,
                    PowerSteering = passengerVehicle.PowerSteering,
                    IdVehicle = passengerVehicle.IdVehicle,
                };

                await _vehicleRepository.RegisterPassengerVehicle(tbPassengerVehicle);

                baseReturn.Code = "201";
                baseReturn.Message = "Passenger vehicle registered.";
                baseReturn.Data = passengerVehicle;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<List<Vehicle>>> ListAvailableVehicles()
        {
            BaseReturn<List<Vehicle>> baseReturn = new BaseReturn<List<Vehicle>>();

            try
            {
                LoggedUser user = _authApplication.GetLoggedUser();

                if (user == null || user.Roles == null || !user.Roles.Any())
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized";
                    return baseReturn;
                }

                List<LoccarInfra.ORM.model.Vehicle> tbVehicles = await _vehicleRepository.ListAvailableVehicles();

                if (tbVehicles == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "No available vehicles found.";
                    return baseReturn;
                }

                List<Vehicle> vehicles = new List<Vehicle>();
                foreach (LoccarInfra.ORM.model.Vehicle tbVehicle in tbVehicles)
                {
                    Vehicle vehicle = new Vehicle()
                    {
                        IdVehicle = tbVehicle.IdVehicle,
                        Brand = tbVehicle.Brand,
                        Model = tbVehicle.Model,
                        ManufacturingYear = tbVehicle.ManufacturingYear,
                        ModelYear = tbVehicle.ModelYear,
                        DailyRate = tbVehicle.DailyRate,
                        MonthlyRate = tbVehicle.MonthlyRate,
                        CompanyDailyRate = tbVehicle.CompanyDailyRate,
                        ReducedDailyRate = tbVehicle.ReducedDailyRate,
                        FuelTankCapacity = tbVehicle.FuelTankCapacity,
                        Vin = tbVehicle.Vin,
                        Reserved = tbVehicle.Reserved,
                        ImgUrl = tbVehicle.ImgUrl,
                        Type = DetermineVehicleType(tbVehicle),

                        // CargoVehicle
                        CargoVehicle = tbVehicle.CargoVehicle != null ? new CargoVehicle()
                        {
                            IdVehicle = tbVehicle.CargoVehicle.IdVehicle,
                            CargoCapacity = tbVehicle.CargoVehicle.CargoCapacity,
                            CargoType = tbVehicle.CargoVehicle.CargoType,
                            TareWeight = tbVehicle.CargoVehicle.TareWeight,
                            CargoCompartmentSize = tbVehicle.CargoVehicle.CargoCompartmentSize,
                        }
                        : null,

                        // PassengerVehicle
                        PassengerVehicle = tbVehicle.PassengerVehicle != null ? new PassengerVehicle()
                        {
                            IdVehicle = tbVehicle.PassengerVehicle.IdVehicle,
                            PassengerCapacity = tbVehicle.PassengerVehicle.PassengerCapacity,
                            Tv = tbVehicle.PassengerVehicle.Tv,
                            AirConditioning = tbVehicle.PassengerVehicle.AirConditioning,
                            PowerSteering = tbVehicle.PassengerVehicle.PowerSteering,
                        }
                        : null,

                        // LeisureVehicle
                        LeisureVehicle = tbVehicle.LeisureVehicle != null ? new LeisureVehicle()
                        {
                            IdVehicle = tbVehicle.LeisureVehicle.IdVehicle,
                            Automatic = tbVehicle.LeisureVehicle.Automatic,
                            PowerSteering = tbVehicle.LeisureVehicle.PowerSteering,
                            AirConditioning = tbVehicle.LeisureVehicle.AirConditioning,
                            Category = tbVehicle.LeisureVehicle.Category,
                        }
                        : null,

                        // Motorcycle
                        Motorcycle = tbVehicle.Motorcycle != null ? new Motorcycle()
                        {
                            IdVehicle = tbVehicle.Motorcycle.IdVehicle,
                            TractionControl = tbVehicle.Motorcycle.TractionControl,
                            AbsBrakes = tbVehicle.Motorcycle.AbsBrakes,
                            CruiseControl = tbVehicle.Motorcycle.CruiseControl,
                        }
                        : null,
                    };

                    vehicles.Add(vehicle);
                }

                baseReturn.Code = "200";
                baseReturn.Message = "Available vehicles list:";
                baseReturn.Data = vehicles;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<bool>> SetVehicleMaintenance(int vehicleId, bool inMaintenance)
        {
            BaseReturn<bool> baseReturn = new BaseReturn<bool>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();

                // Fixed to allow ADMIN and EMPLOYEE
                if (loggedUser?.Roles == null || 
                    (!loggedUser.Roles.Contains("CLIENT_ADMIN") && !loggedUser.Roles.Contains("CLIENT_EMPLOYEE")))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    baseReturn.Data = false;
                    return baseReturn;
                }

                bool success = await _vehicleRepository.SetVehicleMaintenance(vehicleId, inMaintenance);

                baseReturn.Code = success ? "200" : "404";
                baseReturn.Data = success;
                baseReturn.Message = success ? "Vehicle updated successfully." : "Vehicle not found.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = ex.Message;
                baseReturn.Data = false;
            }

            return baseReturn;
        }

        // New CRUD methods
        public async Task<BaseReturn<Vehicle>> GetVehicleById(int vehicleId)
        {
            BaseReturn<Vehicle> baseReturn = new BaseReturn<Vehicle>();

            try
            {
                LoggedUser user = _authApplication.GetLoggedUser();

                if (user == null || user.Roles == null || !user.Roles.Any())
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized";
                    return baseReturn;
                }

                LoccarInfra.ORM.model.Vehicle tbVehicle = await _vehicleRepository.GetVehicleById(vehicleId);

                if (tbVehicle == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "Vehicle not found.";
                    return baseReturn;
                }

                Vehicle vehicle = new Vehicle()
                {
                    IdVehicle = tbVehicle.IdVehicle,
                    Brand = tbVehicle.Brand,
                    Model = tbVehicle.Model,
                    ManufacturingYear = tbVehicle.ManufacturingYear,
                    ModelYear = tbVehicle.ModelYear,
                    DailyRate = tbVehicle.DailyRate,
                    MonthlyRate = tbVehicle.MonthlyRate,
                    CompanyDailyRate = tbVehicle.CompanyDailyRate,
                    ReducedDailyRate = tbVehicle.ReducedDailyRate,
                    FuelTankCapacity = tbVehicle.FuelTankCapacity,
                    Vin = tbVehicle.Vin,
                    Reserved = tbVehicle.Reserved,
                    ImgUrl = tbVehicle.ImgUrl,
                    Type = DetermineVehicleType(tbVehicle),

                    // CargoVehicle
                    CargoVehicle = tbVehicle.CargoVehicle != null ? new CargoVehicle()
                    {
                        IdVehicle = tbVehicle.CargoVehicle.IdVehicle,
                        CargoCapacity = tbVehicle.CargoVehicle.CargoCapacity,
                        CargoType = tbVehicle.CargoVehicle.CargoType,
                        TareWeight = tbVehicle.CargoVehicle.TareWeight,
                        CargoCompartmentSize = tbVehicle.CargoVehicle.CargoCompartmentSize,
                    }
                    : null,

                    // PassengerVehicle
                    PassengerVehicle = tbVehicle.PassengerVehicle != null ? new PassengerVehicle()
                    {
                        IdVehicle = tbVehicle.PassengerVehicle.IdVehicle,
                        PassengerCapacity = tbVehicle.PassengerVehicle.PassengerCapacity,
                        Tv = tbVehicle.PassengerVehicle.Tv,
                        AirConditioning = tbVehicle.PassengerVehicle.AirConditioning,
                        PowerSteering = tbVehicle.PassengerVehicle.PowerSteering,
                    }
                    : null,

                    // LeisureVehicle
                    LeisureVehicle = tbVehicle.LeisureVehicle != null ? new LeisureVehicle()
                    {
                        IdVehicle = tbVehicle.LeisureVehicle.IdVehicle,
                        Automatic = tbVehicle.LeisureVehicle.Automatic,
                        PowerSteering = tbVehicle.LeisureVehicle.PowerSteering,
                        AirConditioning = tbVehicle.LeisureVehicle.AirConditioning,
                        Category = tbVehicle.LeisureVehicle.Category,
                    }
                    : null,

                    // Motorcycle
                    Motorcycle = tbVehicle.Motorcycle != null ? new Motorcycle()
                    {
                        IdVehicle = tbVehicle.Motorcycle.IdVehicle,
                        TractionControl = tbVehicle.Motorcycle.TractionControl,
                        AbsBrakes = tbVehicle.Motorcycle.AbsBrakes,
                        CruiseControl = tbVehicle.Motorcycle.CruiseControl,
                    }
                    : null,
                };

                baseReturn.Code = "200";
                baseReturn.Message = "Vehicle found successfully.";
                baseReturn.Data = vehicle;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<List<Vehicle>>> ListAllVehicles()
        {
            BaseReturn<List<Vehicle>> baseReturn = new BaseReturn<List<Vehicle>>();

            try
            {
                LoggedUser user = _authApplication.GetLoggedUser();

                if (user == null)
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized";
                    return baseReturn;
                }

                List<LoccarInfra.ORM.model.Vehicle> tbVehicles = await _vehicleRepository.ListAllVehicles();

                if (tbVehicles == null || !tbVehicles.Any())
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "No vehicles found.";
                    return baseReturn;
                }

                List<Vehicle> vehicles = new List<Vehicle>();
                foreach (LoccarInfra.ORM.model.Vehicle tbVehicle in tbVehicles)
                {
                    Vehicle vehicle = new Vehicle()
                    {
                        IdVehicle = tbVehicle.IdVehicle,
                        Brand = tbVehicle.Brand,
                        Model = tbVehicle.Model,
                        ManufacturingYear = tbVehicle.ManufacturingYear,
                        ModelYear = tbVehicle.ModelYear,
                        DailyRate = tbVehicle.DailyRate,
                        MonthlyRate = tbVehicle.MonthlyRate,
                        CompanyDailyRate = tbVehicle.CompanyDailyRate,
                        ReducedDailyRate = tbVehicle.ReducedDailyRate,
                        FuelTankCapacity = tbVehicle.FuelTankCapacity,
                        Vin = tbVehicle.Vin,
                        Reserved = tbVehicle.Reserved,
                        ImgUrl = tbVehicle.ImgUrl,
                        Type = DetermineVehicleType(tbVehicle),
                        
                        // CargoVehicle
                        CargoVehicle = tbVehicle.CargoVehicle != null ? new CargoVehicle()
                        {
                            IdVehicle = tbVehicle.CargoVehicle.IdVehicle,
                            CargoCapacity = tbVehicle.CargoVehicle.CargoCapacity,
                            CargoType = tbVehicle.CargoVehicle.CargoType,
                            TareWeight = tbVehicle.CargoVehicle.TareWeight,
                            CargoCompartmentSize = tbVehicle.CargoVehicle.CargoCompartmentSize,
                        }
                        : null,

                        // PassengerVehicle
                        PassengerVehicle = tbVehicle.PassengerVehicle != null ? new PassengerVehicle()
                        {
                            IdVehicle = tbVehicle.PassengerVehicle.IdVehicle,
                            PassengerCapacity = tbVehicle.PassengerVehicle.PassengerCapacity,
                            Tv = tbVehicle.PassengerVehicle.Tv,
                            AirConditioning = tbVehicle.PassengerVehicle.AirConditioning,
                            PowerSteering = tbVehicle.PassengerVehicle.PowerSteering,
                        }
                        : null,

                        // LeisureVehicle
                        LeisureVehicle = tbVehicle.LeisureVehicle != null ? new LeisureVehicle()
                        {
                            IdVehicle = tbVehicle.LeisureVehicle.IdVehicle,
                            Automatic = tbVehicle.LeisureVehicle.Automatic,
                            PowerSteering = tbVehicle.LeisureVehicle.PowerSteering,
                            AirConditioning = tbVehicle.LeisureVehicle.AirConditioning,
                            Category = tbVehicle.LeisureVehicle.Category,
                        }
                        : null,

                        // Motorcycle
                        Motorcycle = tbVehicle.Motorcycle != null ? new Motorcycle()
                        {
                            IdVehicle = tbVehicle.Motorcycle.IdVehicle,
                            TractionControl = tbVehicle.Motorcycle.TractionControl,
                            AbsBrakes = tbVehicle.Motorcycle.AbsBrakes,
                            CruiseControl = tbVehicle.Motorcycle.CruiseControl,
                        }
                        : null,
                    };

                    vehicles.Add(vehicle);
                }

                baseReturn.Code = "200";
                baseReturn.Message = "All vehicles list:";
                baseReturn.Data = vehicles;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<VehicleListResponse>> ListAllVehiclesWithCounts()
        {
            BaseReturn<VehicleListResponse> baseReturn = new BaseReturn<VehicleListResponse>();

            try
            {
                LoggedUser user = _authApplication.GetLoggedUser();

                if (user?.Roles == null || 
                    (!user.Roles.Contains("CLIENT_ADMIN") && !user.Roles.Contains("CLIENT_EMPLOYEE")))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized";
                    return baseReturn;
                }

                // Search data in parallel for better performance
                var vehiclesTask = _vehicleRepository.ListAllVehicles();
                var totalCountTask = _vehicleRepository.GetTotalVehiclesCount();
                var availableCountTask = _vehicleRepository.GetAvailableVehiclesCount();

                await Task.WhenAll(vehiclesTask, totalCountTask, availableCountTask);

                List<LoccarInfra.ORM.model.Vehicle> tbVehicles = await vehiclesTask;
                int totalVehicles = await totalCountTask;
                int availableVehicles = await availableCountTask;
                int reservedVehicles = totalVehicles - availableVehicles;

                if (tbVehicles == null || !tbVehicles.Any())
                {
                    var emptyResponse = new VehicleListResponse
                    {
                        Vehicles = new List<Vehicle>(),
                        TotalVehicles = 0,
                        AvailableVehicles = 0,
                        ReservedVehicles = 0,
                        GeneratedAt = DateTime.Now
                    };

                    baseReturn.Code = "200";
                    baseReturn.Message = "No vehicles found.";
                    baseReturn.Data = emptyResponse;
                    return baseReturn;
                }

                List<Vehicle> vehicles = new List<Vehicle>();
                foreach (LoccarInfra.ORM.model.Vehicle tbVehicle in tbVehicles)
                {
                    Vehicle vehicle = new Vehicle()
                    {
                        IdVehicle = tbVehicle.IdVehicle,
                        Brand = tbVehicle.Brand,
                        Model = tbVehicle.Model,
                        ManufacturingYear = tbVehicle.ManufacturingYear,
                        ModelYear = tbVehicle.ModelYear,
                        DailyRate = tbVehicle.DailyRate,
                        MonthlyRate = tbVehicle.MonthlyRate,
                        CompanyDailyRate = tbVehicle.CompanyDailyRate,
                        ReducedDailyRate = tbVehicle.ReducedDailyRate,
                        FuelTankCapacity = tbVehicle.FuelTankCapacity,
                        Vin = tbVehicle.Vin,
                        Reserved = tbVehicle.Reserved,
                        // Vehicle type added here
                        Type = DetermineVehicleType(tbVehicle),

                        // CargoVehicle
                        CargoVehicle = tbVehicle.CargoVehicle != null ? new CargoVehicle()
                        {
                            IdVehicle = tbVehicle.CargoVehicle.IdVehicle,
                            CargoCapacity = tbVehicle.CargoVehicle.CargoCapacity,
                            CargoType = tbVehicle.CargoVehicle.CargoType,
                            TareWeight = tbVehicle.CargoVehicle.TareWeight,
                            CargoCompartmentSize = tbVehicle.CargoVehicle.CargoCompartmentSize,
                        }
                        : null,

                        // PassengerVehicle
                        PassengerVehicle = tbVehicle.PassengerVehicle != null ? new PassengerVehicle()
                        {
                            IdVehicle = tbVehicle.PassengerVehicle.IdVehicle,
                            PassengerCapacity = tbVehicle.PassengerVehicle.PassengerCapacity,
                            Tv = tbVehicle.PassengerVehicle.Tv,
                            AirConditioning = tbVehicle.PassengerVehicle.AirConditioning,
                            PowerSteering = tbVehicle.PassengerVehicle.PowerSteering,
                        }
                        : null,

                        // LeisureVehicle
                        LeisureVehicle = tbVehicle.LeisureVehicle != null ? new LeisureVehicle()
                        {
                            IdVehicle = tbVehicle.LeisureVehicle.IdVehicle,
                            Automatic = tbVehicle.LeisureVehicle.Automatic,
                            PowerSteering = tbVehicle.LeisureVehicle.PowerSteering,
                            AirConditioning = tbVehicle.LeisureVehicle.AirConditioning,
                            Category = tbVehicle.LeisureVehicle.Category,
                        }
                        : null,

                        // Motorcycle
                        Motorcycle = tbVehicle.Motorcycle != null ? new Motorcycle()
                        {
                            IdVehicle = tbVehicle.Motorcycle.IdVehicle,
                            TractionControl = tbVehicle.Motorcycle.TractionControl,
                            AbsBrakes = tbVehicle.Motorcycle.AbsBrakes,
                            CruiseControl = tbVehicle.Motorcycle.CruiseControl,
                        }
                        : null,
                    };

                    vehicles.Add(vehicle);
                }

                var response = new VehicleListResponse
                {
                    Vehicles = vehicles,
                    TotalVehicles = totalVehicles,
                    AvailableVehicles = availableVehicles,
                    ReservedVehicles = reservedVehicles,
                    GeneratedAt = DateTime.Now
                };

                baseReturn.Code = "200";
                baseReturn.Message = $"Vehicle list with counts retrieved successfully. Total: {totalVehicles}, Available: {availableVehicles}, Reserved: {reservedVehicles}";
                baseReturn.Data = response;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<Vehicle>> UpdateVehicle(Vehicle vehicle)
        {
            BaseReturn<Vehicle> baseReturn = new BaseReturn<Vehicle>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();

                // Fixed authorization logic
                if (loggedUser?.Roles == null || 
                    (!loggedUser.Roles.Contains("CLIENT_ADMIN") && !loggedUser.Roles.Contains("CLIENT_EMPLOYEE")))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    return baseReturn;
                }

                // Search existing vehicle to verify current type
                var existingVehicle = await _vehicleRepository.GetVehicleById(vehicle.IdVehicle);
                if (existingVehicle == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "Vehicle not found.";
                    return baseReturn;
                }

                // Determine current vehicle type
                VehicleType currentType = DetermineVehicleType(existingVehicle);

                // Update basic vehicle properties
                LoccarInfra.ORM.model.Vehicle tbVehicle = new LoccarInfra.ORM.model.Vehicle()
                {
                    IdVehicle = vehicle.IdVehicle,
                    Brand = vehicle.Brand,
                    Model = vehicle.Model,
                    ManufacturingYear = vehicle.ManufacturingYear,
                    ModelYear = vehicle.ModelYear,
                    DailyRate = vehicle.DailyRate,
                    MonthlyRate = vehicle.MonthlyRate,
                    CompanyDailyRate = vehicle.CompanyDailyRate,
                    ReducedDailyRate = vehicle.ReducedDailyRate,
                    FuelTankCapacity = vehicle.FuelTankCapacity,
                    Vin = vehicle.Vin,
                    Reserved = vehicle.Reserved,
                    ImgUrl = vehicle.ImgUrl
                };

                var updatedVehicle = await _vehicleRepository.UpdateVehicle(tbVehicle);

                if (updatedVehicle == null)
                {
                    baseReturn.Code = "500";
                    baseReturn.Message = "Failed to update vehicle basic information.";
                    return baseReturn;
                }

                // If vehicle type changed, remove old type
                if (currentType != vehicle.Type)
                {
                    await RemoveCurrentVehicleType(vehicle.IdVehicle, currentType);
                }

                // Update or create new specific vehicle type
                bool typeUpdateSuccess = false;
                switch (vehicle.Type)
                {
                    case VehicleType.Cargo:
                        if (vehicle.CargoVehicle != null)
                        {
                            vehicle.CargoVehicle.IdVehicle = vehicle.IdVehicle;
                            var cargoResult = await UpdateCargoVehicle(vehicle.CargoVehicle);
                            typeUpdateSuccess = cargoResult.Code == "200";
                        }
                        break;

                    case VehicleType.Motorcycle:
                        if (vehicle.Motorcycle != null)
                        {
                            vehicle.Motorcycle.IdVehicle = vehicle.IdVehicle;
                            var motorcycleResult = await UpdateMotorcycleVehicle(vehicle.Motorcycle);
                            typeUpdateSuccess = motorcycleResult.Code == "200";
                        }
                        break;

                    case VehicleType.Leisure:
                        if (vehicle.LeisureVehicle != null)
                        {
                            vehicle.LeisureVehicle.IdVehicle = vehicle.IdVehicle;
                            var leisureResult = await UpdateLeisureVehicle(vehicle.LeisureVehicle);
                            typeUpdateSuccess = leisureResult.Code == "200";
                        }
                        break;

                    case VehicleType.Passenger:
                        if (vehicle.PassengerVehicle != null)
                        {
                            vehicle.PassengerVehicle.IdVehicle = vehicle.IdVehicle;
                            var passengerResult = await UpdatePassengerVehicle(vehicle.PassengerVehicle);
                            typeUpdateSuccess = passengerResult.Code == "200";
                        }
                        break;
                }

                if (!typeUpdateSuccess)
                {
                    baseReturn.Code = "400";
                    baseReturn.Message = "Failed to update vehicle type-specific information.";
                    return baseReturn;
                }

                baseReturn.Code = "200";
                baseReturn.Data = vehicle;
                baseReturn.Message = "Vehicle updated successfully";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        private static VehicleType DetermineVehicleType(LoccarInfra.ORM.model.Vehicle vehicle)
        {
            if (vehicle.CargoVehicle != null)
                return VehicleType.Cargo;
            if (vehicle.Motorcycle != null)
                return VehicleType.Motorcycle;
            if (vehicle.PassengerVehicle != null)
                return VehicleType.Passenger;
            if (vehicle.LeisureVehicle != null)
                return VehicleType.Leisure;
            
            // Default value if no specific type found
            return VehicleType.Passenger;
        }

        private async Task RemoveCurrentVehicleType(int vehicleId, VehicleType currentType)
        {
            switch (currentType)
            {
                case VehicleType.Cargo:
                    await _vehicleRepository.RemoveCargoVehicle(vehicleId);
                    break;
                case VehicleType.Motorcycle:
                    await _vehicleRepository.RemoveMotorcycleVehicle(vehicleId);
                    break;
                case VehicleType.Passenger:
                    await _vehicleRepository.RemovePassengerVehicle(vehicleId);
                    break;
                case VehicleType.Leisure:
                    await _vehicleRepository.RemoveLeisureVehicle(vehicleId);
                    break;
            }
        }

        private async Task<BaseReturn<CargoVehicle>> UpdateCargoVehicle(CargoVehicle cargoVehicle)
        {
            BaseReturn<CargoVehicle> baseReturn = new BaseReturn<CargoVehicle>();
            try
            {
                LoccarInfra.ORM.model.CargoVehicle tbCargoVehicle = new LoccarInfra.ORM.model.CargoVehicle()
                {
                    CargoCapacity = cargoVehicle.CargoCapacity,
                    CargoCompartmentSize = cargoVehicle.CargoCompartmentSize,
                    CargoType = cargoVehicle.CargoType,
                    TareWeight = cargoVehicle.TareWeight,
                    IdVehicle = cargoVehicle.IdVehicle,
                };

                await _vehicleRepository.UpdateCargoVehicle(tbCargoVehicle);

                baseReturn.Code = "200";
                baseReturn.Message = "Cargo vehicle updated.";
                baseReturn.Data = cargoVehicle;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        private async Task<BaseReturn<LeisureVehicle>> UpdateLeisureVehicle(LeisureVehicle leisureVehicle)
        {
            BaseReturn<LeisureVehicle> baseReturn = new BaseReturn<LeisureVehicle>();
            try
            {
                LoccarInfra.ORM.model.LeisureVehicle tbLeisureVehicle = new LoccarInfra.ORM.model.LeisureVehicle()
                {
                    Automatic = leisureVehicle.Automatic,
                    PowerSteering = leisureVehicle.PowerSteering,
                    AirConditioning = leisureVehicle.AirConditioning,
                    Category = leisureVehicle.Category,
                    IdVehicle = leisureVehicle.IdVehicle,
                };

                await _vehicleRepository.UpdateLeisureVehicle(tbLeisureVehicle);

                baseReturn.Code = "200";
                baseReturn.Message = "Leisure vehicle updated.";
                baseReturn.Data = leisureVehicle;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        private async Task<BaseReturn<Motorcycle>> UpdateMotorcycleVehicle(Motorcycle motorcycle)
        {
            BaseReturn<Motorcycle> baseReturn = new BaseReturn<Motorcycle>();
            try
            {
                LoccarInfra.ORM.model.Motorcycle tbMotorcycle = new LoccarInfra.ORM.model.Motorcycle()
                {
                    TractionControl = motorcycle.TractionControl,
                    AbsBrakes = motorcycle.AbsBrakes,
                    CruiseControl = motorcycle.CruiseControl,
                    IdVehicle = motorcycle.IdVehicle,
                };

                await _vehicleRepository.UpdateMotorcycleVehicle(tbMotorcycle);

                baseReturn.Code = "200";
                baseReturn.Message = "Motorcycle updated.";
                baseReturn.Data = motorcycle;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        private async Task<BaseReturn<PassengerVehicle>> UpdatePassengerVehicle(PassengerVehicle passengerVehicle)
        {
            BaseReturn<PassengerVehicle> baseReturn = new BaseReturn<PassengerVehicle>();
            try
            {
                LoccarInfra.ORM.model.PassengerVehicle tbPassengerVehicle = new LoccarInfra.ORM.model.PassengerVehicle()
                {
                    PassengerCapacity = passengerVehicle.PassengerCapacity,
                    Tv = passengerVehicle.Tv,
                    AirConditioning = passengerVehicle.AirConditioning,
                    PowerSteering = passengerVehicle.PowerSteering,
                    IdVehicle = passengerVehicle.IdVehicle,
                };

                await _vehicleRepository.UpdatePassengerVehicle(tbPassengerVehicle);

                baseReturn.Code = "200";
                baseReturn.Message = "Passenger vehicle updated.";
                baseReturn.Data = passengerVehicle;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<bool>> DeleteVehicle(int vehicleId)
        {
            BaseReturn<bool> baseReturn = new BaseReturn<bool>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();

                // Fixed authorization logic
                if (loggedUser?.Roles == null || 
                    (!loggedUser.Roles.Contains("CLIENT_ADMIN") && !loggedUser.Roles.Contains("CLIENT_EMPLOYEE")))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    baseReturn.Data = false;
                    return baseReturn;
                }

                bool success = await _vehicleRepository.DeleteVehicle(vehicleId);

                baseReturn.Code = success ? "200" : "404";
                baseReturn.Data = success;
                baseReturn.Message = success ? "Vehicle deleted successfully." : "Vehicle not found.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
                baseReturn.Data = false;
            }

            return baseReturn;
        }

        public async Task<BaseReturn<bool>> SetVehicleReserved(int vehicleId, bool reserved)
        {
            BaseReturn<bool> baseReturn = new BaseReturn<bool>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();

                // Allow access for authenticated users (any role)
                if (loggedUser?.Roles == null || !loggedUser.Roles.Any())
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    baseReturn.Data = false;
                    return baseReturn;
                }

                bool success = await _vehicleRepository.SetVehicleReserved(vehicleId, reserved);

                baseReturn.Code = success ? "200" : "404";
                baseReturn.Data = success;
                baseReturn.Message = success 
                    ? $"Vehicle {(reserved ? "reserved" : "released")} successfully." 
                    : "Vehicle not found.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
                baseReturn.Data = false;
            }

            return baseReturn;
        }
    }
}
