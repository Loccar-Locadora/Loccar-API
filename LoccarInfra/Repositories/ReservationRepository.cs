using LoccarInfra.ORM.model;
using LoccarInfra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoccarInfra.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly DataBaseContext _context;

        public ReservationRepository(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<Reservation> CreateReservation(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }

        public async Task<Reservation> GetReservationWithVehicle(int reservationNumber)
        {
            return await _context.Reservations
                .Include(r => r.IdvehicleNavigation) // Inclui os detalhes do veículo
                .FirstOrDefaultAsync(r => r.Reservationnumber == reservationNumber);
        }

        public async Task<bool> CancelReservation(int reservationNumber)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Reservationnumber == reservationNumber);

            if (reservation == null)
                return false;

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Reservation>> GetReservationsByCustomer(int customerId)
        {
            return await _context.Reservations
                .Where(r => r.Idcustomer == customerId)
                .Include(r => r.IdvehicleNavigation) // Inclui os detalhes do veículo
                .ToListAsync();
        }

        public async Task<bool> RegisterDamage(int reservationNumber, string damageDescription)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Reservationnumber == reservationNumber);

            if (reservation == null) return false;

            reservation.DamageDescription = damageDescription; // adicione o campo na ORM
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
