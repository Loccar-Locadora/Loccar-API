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
    }
}
