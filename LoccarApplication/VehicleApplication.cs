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

                if (loggedUser.Roles.Contains("COMMON_USER") || loggedUser.Roles == null)
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "Usuário não autorizado.";
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
                   Reserved = false
                };

                await _vehicleRepository.RegisterVehicle(tbVehicle);

                switch (vehicle.Type)
                {
                    case VehicleType.Cargo:
                        baseReturn.Data = vehicle.CargoVehicle != null ? (await RegisterCargoVehicle(vehicle.CargoVehicle)).Data : null;
                        break;
                    case VehicleType.Motorcycle:
                        baseReturn.Data = vehicle.CargoVehicle != null ? (await RegisterMotorcycleVehicle(vehicle.Motorcycle)).Data : null;
                        break;
                    case VehicleType.Leisure:
                        baseReturn.Data = vehicle.CargoVehicle != null ? (await RegisterLeisureVehicle(vehicle.LeisureVehicle)).Data : null;
                        break;
                    case VehicleType.Passenger:
                        baseReturn.Data = vehicle.CargoVehicle != null ? (await RegisterPassengerVehicle(vehicle.PassengerVehicle)).Data : null;
                        break;
                }

                if (baseReturn.Data == null)
                {
                    baseReturn.Code = "400";
                    baseReturn.Message = "Não foi possível cadastrar o veículo";
                    return baseReturn;
                }

                baseReturn.Code = "201";
                baseReturn.Data = vehicle;
                baseReturn.Message = "Veículo cadastrado com sucesso";

            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"Ocorreu um erro inesperado: {ex.Message}";
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
                    Idvehicle = cargoVehicle.Idvehicle
                };

                await _vehicleRepository.RegisterCargoVehicle(tbCargoVehicle);

                baseReturn.Code = "201";
                baseReturn.Message = "Veículo de carga cadastrado.";
                baseReturn.Data = cargoVehicle; 
            }
            catch (Exception ex) 
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"Ocorreu um erro inesperado: {ex.Message}";
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
                    Idvehicle = leisureVehicle.Idvehicle
                };

                await _vehicleRepository.RegisterLeisureVehicle(tbLeisureVehicle);

                baseReturn.Code = "201";
                baseReturn.Message = "Veículo de lazer cadastrado.";
                baseReturn.Data = leisureVehicle;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"Ocorreu um erro inesperado: {ex.Message}";
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
                    Idvehicle = motorcycle.Idvehicle
                };

                await _vehicleRepository.RegisterMotorcycleVehicle(tbMotorcycle);

                baseReturn.Code = "201";
                baseReturn.Message = "Motocicleta cadastrada.";
                baseReturn.Data = motorcycle;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"Ocorreu um erro inesperado: {ex.Message}";
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
                    Idvehicle = passengerVehicle.Idvehicle
                };

                await _vehicleRepository.RegisterPassengerVehicle(tbPassengerVehicle);

                baseReturn.Code = "201";
                baseReturn.Message = "Veículo de passageiro cadastrado.";
                baseReturn.Data = passengerVehicle;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"Ocorreu um erro inesperado: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<List<Vehicle>>> ListAvailableVehicles()
        {
            BaseReturn<List<Vehicle>> baseReturn = new BaseReturn<List<Vehicle>>();

            try
            {
                LoggedUser user = _authApplication.GetLoggedUser();

                if (user == null)
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "Usuário não autorizado";
                    return baseReturn;
                }

                List<LoccarInfra.ORM.model.Vehicle> tbVehicles = await _vehicleRepository.ListAvailableVehicles();

                if(tbVehicles == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "Nenhum veículo disponível encontrado.";
                    return baseReturn;
                }

                List<Vehicle> vehicles = new List<Vehicle>();
                foreach (LoccarInfra.ORM.model.Vehicle tbVehicle in tbVehicles)
                {
                    Vehicle vehicle = new Vehicle()
                    {
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

                        // CargoVehicle
                        CargoVehicle = tbVehicle.CargoVehicle != null ? new CargoVehicle()
                        {
                            IdVehicle = tbVehicle.CargoVehicle.Idvehicle,
                            CargoCapacity = tbVehicle.CargoVehicle.CargoCapacity,
                            CargoType = tbVehicle.CargoVehicle.CargoType,
                            TareWeight = tbVehicle.CargoVehicle.TareWeight,
                            CargoCompartmentSize = tbVehicle.CargoVehicle.CargoCompartmentSize
                        } : null,

                        // PassengerVehicle
                        PassengerVehicle = tbVehicle.PassengerVehicle != null ? new PassengerVehicle()
                        {
                            IdVehicle = tbVehicle.PassengerVehicle.Idvehicle,
                            PassengerCapacity = tbVehicle.PassengerVehicle.PassengerCapacity,
                            Tv = tbVehicle.PassengerVehicle.Tv,
                            AirConditioning = tbVehicle.PassengerVehicle.AirConditioning,
                            PowerSteering = tbVehicle.PassengerVehicle.PowerSteering
                        } : null,

                        // LeisureVehicle
                        LeisureVehicle = tbVehicle.LeisureVehicle != null ? new LeisureVehicle()
                        {
                            IdVehicle = tbVehicle.LeisureVehicle.Idvehicle,
                            Automatic = tbVehicle.LeisureVehicle.Automatic,
                            PowerSteering = tbVehicle.LeisureVehicle.PowerSteering,
                            AirConditioning = tbVehicle.LeisureVehicle.AirConditioning,
                            Category = tbVehicle.LeisureVehicle.Category
                        } : null,

                        // Motorcycle
                        Motorcycle = tbVehicle.Motorcycle != null ? new Motorcycle()
                        {
                            IdVehicle = tbVehicle.Motorcycle.Idvehicle,
                            TractionControl = tbVehicle.Motorcycle.TractionControl,
                            AbsBrakes = tbVehicle.Motorcycle.AbsBrakes,
                            CruiseControl = tbVehicle.Motorcycle.CruiseControl
                        } : null,
                    };

                    vehicles.Add(vehicle);
                }


                baseReturn.Code = "200";
                baseReturn.Message = "Lista de veículos disponíveis:";
                baseReturn.Data = vehicles;

            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"Ocorreu um erro inesperado{ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<bool>> SetVehicleMaintenance(int vehicleId, bool inMaintenance)
        {
            BaseReturn<bool> baseReturn = new BaseReturn<bool>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();
                if (!loggedUser.Roles.Contains("ADMIN"))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "Usuário não autorizado.";
                    baseReturn.Data = false;
                    return baseReturn;
                }

                bool success = await _vehicleRepository.SetVehicleMaintenance(vehicleId, inMaintenance);

                baseReturn.Code = success ? "200" : "404";
                baseReturn.Data = success;
                baseReturn.Message = success ? "Veículo atualizado com sucesso." : "Veículo não encontrado.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = ex.Message;
                baseReturn.Data = false;
            }

            return baseReturn;
        }

    }
}
