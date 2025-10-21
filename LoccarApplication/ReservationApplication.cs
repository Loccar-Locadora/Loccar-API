using LoccarApplication.Interfaces;
using LoccarDomain;
using LoccarDomain.LoggedUser.Models;
using LoccarDomain.Reservation.Models;
using LoccarInfra.Repositories.Interfaces;

namespace LoccarApplication
{
    public class ReservationApplication : IReservationApplication
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IAuthApplication _authApplication;

        public ReservationApplication(IReservationRepository reservationRepository, IAuthApplication authApplication)
        {
            _reservationRepository = reservationRepository;
            _authApplication = authApplication;
        }

        // US05 - Cliente: Reservar veículo online
        public async Task<BaseReturn<Reservation>> CreateReservation(Reservation reservation)
        {
            BaseReturn<Reservation> baseReturn = new BaseReturn<Reservation>();

            try
            {
                // Mapeia do Domain para ORM
                var tabelaReservation = new LoccarInfra.ORM.model.Reservation()
                {
                    Idcustomer = reservation.Idcustomer,
                    Idvehicle = reservation.Idvehicle,
                    RentalDate = reservation.RentalDate,
                    ReturnDate = reservation.ReturnDate,
                    DailyRate = reservation.DailyRate,
                    RateType = reservation.RateType,
                    InsuranceVehicle = reservation.InsuranceVehicle,
                    InsuranceThirdParty = reservation.InsuranceThirdParty,
                    TaxAmount = reservation.TaxAmount
                };

                var response = await _reservationRepository.CreateReservation(tabelaReservation);

                // Mapeia de volta para Domain
                var reservationResponse = new Reservation()
                {
                    Reservationnumber = response.Reservationnumber,
                    Idcustomer = response.Idcustomer,
                    Idvehicle = response.Idvehicle,
                    RentalDate = response.RentalDate,
                    ReturnDate = response.ReturnDate,
                    DailyRate = response.DailyRate,
                    RateType = response.RateType,
                    InsuranceVehicle = response.InsuranceVehicle,
                    InsuranceThirdParty = response.InsuranceThirdParty,
                    TaxAmount = response.TaxAmount
                };

                baseReturn.Code = "200";
                baseReturn.Data = reservationResponse;
                baseReturn.Message = "Reserva criada com sucesso.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = ex.Message;
            }

            return baseReturn;
        }

        // US04 - Cliente: Calcular custo total da reserva
        public async Task<BaseReturn<decimal>> CalculateTotalCost(int reservationNumber)
        {
            BaseReturn<decimal> baseReturn = new BaseReturn<decimal>();

            try
            {
                var reservation = await _reservationRepository.GetReservationWithVehicle(reservationNumber);
                if (reservation == null)
                    throw new Exception("Reserva não encontrada.");

                var days = (reservation.ReturnDate - reservation.RentalDate).Days;
                if (days <= 0) days = 1;

                decimal totalCost = (reservation.DailyRate ?? 0) * days;

                baseReturn.Code = "200";
                baseReturn.Data = totalCost;
                baseReturn.Message = "Cálculo realizado com sucesso.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = ex.Message;
            }

            return baseReturn;
        }

        // US10 - Cliente: Cancelar reserva
        public async Task<BaseReturn<bool>> CancelReservation(int reservationNumber)
        {
            BaseReturn<bool> baseReturn = new BaseReturn<bool>();

            try
            {
                var success = await _reservationRepository.CancelReservation(reservationNumber);

                baseReturn.Code = success ? "200" : "404";
                baseReturn.Data = success;
                baseReturn.Message = success ? "Reserva cancelada com sucesso." : "Reserva não encontrada.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Data = false;
                baseReturn.Message = ex.Message;
            }

            return baseReturn;
        }

        // US11 - Cliente: Consultar histórico de reservas
        public async Task<BaseReturn<List<Reservation>>> GetReservationHistory(int customerId)
        {
            BaseReturn<List<Reservation>> baseReturn = new BaseReturn<List<Reservation>>();

            try
            {
                var response = await _reservationRepository.GetReservationsByCustomer(customerId);

                var reservations = response.Select(r => new Reservation()
                {
                    Reservationnumber = r.Reservationnumber,
                    Idcustomer = r.Idcustomer,
                    Idvehicle = r.Idvehicle,
                    RentalDate = r.RentalDate,
                    ReturnDate = r.ReturnDate,
                    DailyRate = r.DailyRate,
                    RateType = r.RateType,
                    InsuranceVehicle = r.InsuranceVehicle,
                    InsuranceThirdParty = r.InsuranceThirdParty,
                    TaxAmount = r.TaxAmount
                }).ToList();

                baseReturn.Code = "200";
                baseReturn.Data = reservations;
                baseReturn.Message = "Histórico de reservas consultado com sucesso.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = ex.Message;
            }

            return baseReturn;
        }

        public async Task<BaseReturn<bool>> RegisterDamage(int reservationNumber, string damageDescription)
        {
            BaseReturn<bool> baseReturn = new BaseReturn<bool>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();
                if (!loggedUser.Roles.Contains("EMPLOYEE") && !loggedUser.Roles.Contains("ADMIN"))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "Usuário não autorizado.";
                    baseReturn.Data = false;
                    return baseReturn;
                }

                bool success = await _reservationRepository.RegisterDamage(reservationNumber, damageDescription);

                baseReturn.Code = success ? "200" : "404";
                baseReturn.Data = success;
                baseReturn.Message = success ? "Danos registrados com sucesso." : "Reserva não encontrada.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = ex.Message;
                baseReturn.Data = false;
            }

            return baseReturn;
        }

    }
}
