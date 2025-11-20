using LoccarApplication.Interfaces;
using LoccarDomain;
using LoccarDomain.Reservation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoccarWebapi.Controllers
{
    [Route("api/reservation")]
    [ApiController]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationApplication _reservationApplication;

        public ReservationController(IReservationApplication reservationApplication)
        {
            _reservationApplication = reservationApplication;
        }

        [HttpPost("register")]
        public async Task<BaseReturn<Reservation>> RegisterReservation(Reservation reservation)
        {
            return await _reservationApplication.CreateReservation(reservation);
        }

        [HttpGet("calculate-cost/{reservationId}")]
        public async Task<BaseReturn<decimal>> CalculateReservationCost(int reservationId)
        {
            return await _reservationApplication.CalculateTotalCost(reservationId);
        }

        [HttpDelete("cancel/{reservationNumber}")]
        public async Task<BaseReturn<bool>> CancelReservation(int reservationNumber)
        {
            return await _reservationApplication.CancelReservation(reservationNumber);
        }

        [HttpGet("history/{customerId}")]
        public async Task<BaseReturn<List<Reservation>>> GetReservationHistory(int customerId)
        {
            return await _reservationApplication.GetReservationHistory(customerId);
        }

        [HttpPost("damage/{reservationNumber}")]
        public async Task<BaseReturn<bool>> RegisterDamage(int reservationNumber, [FromBody] string damageDescription)
        {
            return await _reservationApplication.RegisterDamage(reservationNumber, damageDescription);
        }

        // Novos endpoints CRUD
        [HttpGet("{id}")]
        public async Task<BaseReturn<Reservation>> GetReservationById(int id)
        {
            return await _reservationApplication.GetReservationById(id);
        }

        [HttpGet("list/all")]
        public async Task<BaseReturn<List<Reservation>>> ListAllReservations()
        {
            return await _reservationApplication.ListAllReservations();
        }

        [HttpPut("update")]
        public async Task<BaseReturn<Reservation>> UpdateReservation(Reservation reservation)
        {
            return await _reservationApplication.UpdateReservation(reservation);
        }

        /// <summary>
        /// Busca o resumo de reservas do usuário logado, incluindo quantidades por status e detalhes de cada reserva
        /// </summary>
        /// <returns>Resumo com contadores e detalhes das reservas ativas, concluídas e canceladas</returns>
        /// <response code="200">Resumo de reservas obtido com sucesso</response>
        /// <response code="401">Usuário não autorizado</response>
        /// <response code="404">Usuário/Cliente não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(BaseReturn<UserReservationSummary>), 200)]
        [ProducesResponseType(typeof(BaseReturn<object>), 401)]
        [ProducesResponseType(typeof(BaseReturn<object>), 404)]
        [ProducesResponseType(typeof(BaseReturn<object>), 500)]
        public async Task<BaseReturn<UserReservationSummary>> GetMyReservationSummary()
        {
            return await _reservationApplication.GetLoggedUserReservationSummary();
        }
    }
}
