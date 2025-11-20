using System.Collections.Generic;
using System.Threading.Tasks;
using LoccarDomain;
using LoccarDomain.LoggedUser.Models;
using LoccarDomain.Reservation.Models;
using DbReservation = LoccarInfra.ORM.model.Reservation;

namespace LoccarInfra.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        // US05 - Criar reserva
        Task<DbReservation> CreateReservation(DbReservation reservation);

        Task<DbReservation> GetReservationById(int reservationId);

        // US10 - Cancelar reserva
        Task<bool> CancelReservation(int reservationNumber);

        Task<List<DbReservation>> GetReservationHistory(int customerId);

        Task<bool> RegisterDamage(int reservationNumber, string damageDescription);

        // Novos métodos CRUD
        Task<List<DbReservation>> ListAllReservations();

        Task<DbReservation> UpdateReservation(DbReservation reservation);

        // Métodos de estatísticas
        Task<int> GetActiveReservationsCount();

        // Métodos de receita
        Task<decimal> GetMonthlyRevenue(int year, int month);
        Task<List<DbReservation>> GetReservationsByMonth(int year, int month);
        Task<decimal> GetCurrentMonthRevenue();
        Task<decimal> GetYearRevenue(int year);

        // Método para buscar resumo de reservas do usuário
        Task<UserReservationSummary> GetUserReservationSummary(int customerId);
    }
}
