using LoccarDomain;
using LoccarDomain.Reservation.Models;

namespace LoccarApplication.Interfaces
{
    public interface IReservationApplication
    {
        // US05 - Criar reserva
        Task<BaseReturn<Reservation>> CreateReservation(Reservation reservation);

        // US04 - Calcular custo total
        Task<BaseReturn<decimal>> CalculateTotalCost(int reservationId);

        // US10 - Cancelar reserva
        Task<BaseReturn<bool>> CancelReservation(int reservationId);

        // US11 - Histórico de reservas
        Task<BaseReturn<List<Reservation>>> GetReservationHistory(int customerId);

        Task<BaseReturn<bool>> RegisterDamage(int reservationNumber, string damageDescription);

    }
}
