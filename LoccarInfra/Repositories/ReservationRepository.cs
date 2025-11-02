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
                .Include(r => r.IdCustomerNavigation)
                .Include(r => r.IdVehicleNavigation)
                .FirstOrDefaultAsync(r => r.Reservationnumber == reservationId);
        }

        public async Task<List<Reservation>> GetReservationHistory(int customerId)
        {
            return await _context.Reservations
                .Include(r => r.IdCustomerNavigation)
                .Include(r => r.IdVehicleNavigation)
                .Where(r => r.IdCustomer == customerId)
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
                .Include(r => r.IdCustomerNavigation)
                .Include(r => r.IdVehicleNavigation)
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

        // Métodos de estatísticas
        public async Task<int> GetActiveReservationsCount()
        {
            return await _context.Reservations
                .CountAsync(r => r.RentalDate <= DateTime.Now && r.ReturnDate >= DateTime.Now);
        }

        // Métodos de receita
        public async Task<decimal> GetMonthlyRevenue(int year, int month)
        {
            var reservations = await GetReservationsByMonth(year, month);
            return CalculateTotalRevenue(reservations);
        }

        public async Task<List<Reservation>> GetReservationsByMonth(int year, int month)
        {
            return await _context.Reservations
                .Include(r => r.IdVehicleNavigation)
                .Where(r => r.RentalDate.Year == year && r.RentalDate.Month == month)
                .ToListAsync();
        }

        public async Task<decimal> GetCurrentMonthRevenue()
        {
            var now = DateTime.Now;
            return await GetMonthlyRevenue(now.Year, now.Month);
        }

        public async Task<decimal> GetYearRevenue(int year)
        {
            var reservations = await _context.Reservations
                .Include(r => r.IdVehicleNavigation)
                .Where(r => r.RentalDate.Year == year)
                .ToListAsync();

            return CalculateTotalRevenue(reservations);
        }

        private decimal CalculateTotalRevenue(List<Reservation> reservations)
        {
            decimal totalRevenue = 0;

            foreach (var reservation in reservations)
            {
                // Calcular receita base
                int days = reservation.RentalDays ?? (reservation.ReturnDate - reservation.RentalDate).Days;
                if (days <= 0) days = 1;

                decimal dailyRate = reservation.DailyRate ?? reservation.IdVehicleNavigation?.DailyRate ?? 0;
                decimal baseRevenue = days * dailyRate;

                // Adicionar seguros e taxas
                decimal insuranceRevenue = (reservation.InsuranceVehicle ?? 0) + (reservation.InsuranceThirdParty ?? 0);
                decimal taxRevenue = reservation.TaxAmount ?? 0;

                totalRevenue += baseRevenue + insuranceRevenue + taxRevenue;
            }

            return totalRevenue;
        }
    }
}
