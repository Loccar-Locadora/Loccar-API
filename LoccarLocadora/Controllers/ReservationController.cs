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
    public class ReservationController : Controller
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
    }
}

