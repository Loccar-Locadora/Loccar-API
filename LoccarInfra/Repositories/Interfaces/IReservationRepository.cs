using System.Collections.Generic;
using System.Threading.Tasks;
using LoccarDomain;
using LoccarDomain.LoggedUser.Models;
using LoccarInfra.ORM.model;

namespace LoccarInfra.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        // US05 - Criar reserva
        Task<Reservation> CreateReservation(Reservation reservation);

        Task<Reservation> GetReservationById(int reservationId);

        // US10 - Cancelar reserva
        Task<bool> CancelReservation(int reservationNumber);

        Task<List<Reservation>> GetReservationHistory(int customerId);

        Task<bool> RegisterDamage(int reservationNumber, string damageDescription);

        // Novos métodos CRUD
        Task<List<Reservation>> ListAllReservations();

        Task<Reservation> UpdateReservation(Reservation reservation);

        // Métodos de estatísticas
        Task<int> GetActiveReservationsCount();

        // Métodos de receita
        Task<decimal> GetMonthlyRevenue(int year, int month);
        Task<List<Reservation>> GetReservationsByMonth(int year, int month);
        Task<decimal> GetCurrentMonthRevenue();
        Task<decimal> GetYearRevenue(int year);
    }
}
