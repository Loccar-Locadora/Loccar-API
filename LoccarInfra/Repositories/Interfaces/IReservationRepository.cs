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

        // US04 - Obter reserva com veículo (para cálculo de custo)
        Task<Reservation> GetReservationWithVehicle(int reservationNumber);

        // US10 - Cancelar reserva
        Task<bool> CancelReservation(int reservationNumber);

        Task<List<Reservation>> GetReservationsByCustomer(int customerId);

        Task<bool> RegisterDamage(int reservationNumber, string damageDescription);

    }
}
