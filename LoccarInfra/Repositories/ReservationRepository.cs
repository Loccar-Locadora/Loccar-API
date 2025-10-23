using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoccarInfra.ORM.model;
using LoccarInfra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Reservation> GetReservationById(int reservationId)
        {
            return await _context.Reservations
                .Include(r => r.IdcustomerNavigation)
                .Include(r => r.IdvehicleNavigation)
                .FirstOrDefaultAsync(r => r.Reservationnumber == reservationId);
        }

        public async Task<List<Reservation>> GetReservationHistory(int customerId)
        {
            return await _context.Reservations
                .Include(r => r.IdcustomerNavigation)
                .Include(r => r.IdvehicleNavigation)
                .Where(r => r.Idcustomer == customerId)
                .OrderByDescending(r => r.RentalDate)
                .ToListAsync();
        }

        public async Task<bool> CancelReservation(int reservationNumber)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Reservationnumber == reservationNumber);

            if (reservation == null)
            {
                return false;
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RegisterDamage(int reservationNumber, string damageDescription)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Reservationnumber == reservationNumber);

            if (reservation == null)
            {
                return false;
            }

            reservation.DamageDescription = damageDescription;
            await _context.SaveChangesAsync();
            return true;
        }

        // Novos métodos CRUD
        public async Task<List<Reservation>> ListAllReservations()
        {
            return await _context.Reservations
                .Include(r => r.IdcustomerNavigation)
                .Include(r => r.IdvehicleNavigation)
                .ToListAsync();
        }

        public async Task<Reservation> UpdateReservation(Reservation reservation)
        {
            var existingReservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Reservationnumber == reservation.Reservationnumber);

            if (existingReservation == null)
            {
                return null;
            }

            existingReservation.RentalDate = reservation.RentalDate;
            existingReservation.ReturnDate = reservation.ReturnDate;
            existingReservation.RentalDays = reservation.RentalDays;
            existingReservation.DailyRate = reservation.DailyRate;
            existingReservation.RateType = reservation.RateType;
            existingReservation.InsuranceVehicle = reservation.InsuranceVehicle;
            existingReservation.InsuranceThirdParty = reservation.InsuranceThirdParty;
            existingReservation.TaxAmount = reservation.TaxAmount;
            existingReservation.DamageDescription = reservation.DamageDescription;

            await _context.SaveChangesAsync();
            return existingReservation;
        }
    }
}
