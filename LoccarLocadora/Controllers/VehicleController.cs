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

        /// <summary>
        /// Lista todos os veículos com contagem de totais, disponíveis e reservados
        /// </summary>
        /// <returns>Lista de veículos com estatísticas</returns>
        /// <response code="200">Lista de veículos com contagens retornada com sucesso</response>
        /// <response code="401">Usuário não autorizado (apenas ADMIN e EMPLOYEE)</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("list/all-with-counts")]
        public async Task<BaseReturn<VehicleListResponse>> ListAllVehiclesWithCounts()
        {
            return await _vehicleApplication.ListAllVehiclesWithCounts();
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

        /// <summary>
        /// Define o status de reserva de um veículo
        /// </summary>
        /// <param name="id">ID do veículo</param>
        /// <param name="reserved">Status de reserva (true = reservado, false = disponível)</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="200">Status de reserva atualizado com sucesso</response>
        /// <response code="401">Usuário não autorizado</response>
        /// <response code="404">Veículo não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("reserve/{id}")]
        public async Task<BaseReturn<bool>> SetVehicleReserved(int id, [FromQuery] bool reserved)
        {
            return await _vehicleApplication.SetVehicleReserved(id, reserved);
        }
    }
}
