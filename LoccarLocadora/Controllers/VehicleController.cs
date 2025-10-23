using LoccarApplication.Interfaces;
using LoccarDomain;
using LoccarDomain.Customer.Models;
using LoccarDomain.LoggedUser.Models;
using LoccarDomain.Vehicle.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoccarWebapi.Controllers
{
    [Route("api/vehicle")]
    [ApiController]
    [Authorize]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleApplication _vehicleApplication;
        private readonly IAuthApplication _authApplication;

        public VehicleController(IVehicleApplication vehicleApplication, IAuthApplication authApplication)
        {
            _vehicleApplication = vehicleApplication;
            _authApplication = authApplication;
        }

        [HttpPost("register")]
        public async Task<BaseReturn<Vehicle>> RegisterVehicle(Vehicle vehicle)
        {
            return await _vehicleApplication.RegisterVehicle(vehicle);
        }

        [HttpGet("list/available")]
        public async Task<BaseReturn<List<Vehicle>>> ListAvailableVehicles()
        {
            return await _vehicleApplication.ListAvailableVehicles();
        }

        // Novos endpoints CRUD
        [HttpGet("{id}")]
        public async Task<BaseReturn<Vehicle>> GetVehicleById(int id)
        {
            return await _vehicleApplication.GetVehicleById(id);
        }

        [HttpGet("list/all")]
        public async Task<BaseReturn<List<Vehicle>>> ListAllVehicles()
        {
            return await _vehicleApplication.ListAllVehicles();
        }

        [HttpPut("update")]
        public async Task<BaseReturn<Vehicle>> UpdateVehicle(Vehicle vehicle)
        {
            return await _vehicleApplication.UpdateVehicle(vehicle);
        }

        [HttpDelete("delete/{id}")]
        public async Task<BaseReturn<bool>> DeleteVehicle(int id)
        {
            return await _vehicleApplication.DeleteVehicle(id);
        }

        [HttpPut("maintenance/{id}")]
        public async Task<BaseReturn<bool>> SetVehicleMaintenance(int id, [FromQuery] bool inMaintenance)
        {
            return await _vehicleApplication.SetVehicleMaintenance(id, inMaintenance);
        }
    }
}
