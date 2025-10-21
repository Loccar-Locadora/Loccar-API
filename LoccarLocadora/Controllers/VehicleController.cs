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
    public class VehicleController : Controller
    {
        private readonly IVehicleApplication _vehicleApplication; 
        private readonly IAuthApplication _authApplication;
        public VehicleController(IVehicleApplication vehicleApplication, IAuthApplication authApplication)
        {
            _vehicleApplication = vehicleApplication;
            _authApplication = authApplication;
        }

        [HttpPost("register")]
        public async Task<BaseReturn<Vehicle>> RegisterVehicle(Vehicle Vehicle)
        {
            return await _vehicleApplication.RegisterVehicle(Vehicle);
        }

        [HttpGet("list/available")]
        public async Task<BaseReturn<List<Vehicle>>> ListAvailableVehicles()
        {
            return await _vehicleApplication.ListAvailableVehicles();
        }
    }
}
