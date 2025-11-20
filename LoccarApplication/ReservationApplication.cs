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
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IAuthApplication _authApplication;
        private readonly IUserRepository _userRepository;

        public ReservationApplication(IReservationRepository reservationRepository, IVehicleRepository vehicleRepository,
            ICustomerRepository customerRepository, IAuthApplication authApplication, IUserRepository userRepository)
        {
            _reservationRepository = reservationRepository;
            _vehicleRepository = vehicleRepository;
            _customerRepository = customerRepository;
            _authApplication = authApplication;
            _userRepository = userRepository;
        }

        // US05 - Cliente: Reservar veículo online
        public async Task<BaseReturn<Reservation>> CreateReservation(Reservation reservation)
        {
            BaseReturn<Reservation> baseReturn = new BaseReturn<Reservation>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();
                if (loggedUser == null)
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    return baseReturn;
                }

                // Validar se o veículo existe e está disponível
                var vehicle = await _vehicleRepository.GetVehicleById(reservation.IdVehicle);
                if (vehicle == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "Vehicle not found.";
                    return baseReturn;
                }

                if (vehicle.Reserved == true)
                {
                    baseReturn.Code = "400";
                    baseReturn.Message = "Vehicle is not available.";
                    return baseReturn;
                }

                // Validar se o cliente existe
                var user = await _userRepository.GetUserById(loggedUser.id);
                var customer = await _customerRepository.GetRegistrationByEmail(user.Email);
                if (customer == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "Customer not found.";
                    return baseReturn;
                }

                LoccarInfra.ORM.model.Reservation tbReservation = new LoccarInfra.ORM.model.Reservation()
                {
                    IdCustomer = customer.IdCustomer,
                    IdVehicle = reservation.IdVehicle,
                    RentalDate = reservation.RentalDate,
                    ReturnDate = reservation.ReturnDate,
                    RentalDays = reservation.RentalDays,
                    DailyRate = reservation.DailyRate,
                    RateType = reservation.RateType,
                    InsuranceVehicle = reservation.InsuranceVehicle,
                    InsuranceThirdParty = reservation.InsuranceThirdParty,
                    TaxAmount = reservation.TaxAmount,
                    Reservationnumber = new Random().Next(100000, 999999),
                };

                var createdReservation = await _reservationRepository.CreateReservation(tbReservation);

                // Marcar veículo como reservado
                vehicle.Reserved = true;
                await _vehicleRepository.UpdateVehicle(vehicle);

                baseReturn.Code = "201";
                baseReturn.Message = "Reservation created successfully.";
                baseReturn.Data = new Reservation()
                {
                    Reservationnumber = createdReservation.Reservationnumber,
                    IdCustomer = createdReservation.IdCustomer,
                    IdVehicle = createdReservation.IdVehicle,
                    RentalDate = createdReservation.RentalDate,
                    ReturnDate = createdReservation.ReturnDate,
                    RentalDays = createdReservation.RentalDays,
                    DailyRate = createdReservation.DailyRate,
                    RateType = createdReservation.RateType,
                    InsuranceVehicle = createdReservation.InsuranceVehicle,
                    InsuranceThirdParty = createdReservation.InsuranceThirdParty,
                    TaxAmount = createdReservation.TaxAmount,
                    DamageDescription = createdReservation.DamageDescription
                };
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"Internal error: {ex.Message}";
            }

            return baseReturn;
        }

        // US04 - Cliente: Calcular custo total da reserva
        public async Task<BaseReturn<decimal>> CalculateTotalCost(int reservationId)
        {
            BaseReturn<decimal> baseReturn = new BaseReturn<decimal>();

            try
            {
                var reservation = await _reservationRepository.GetReservationById(reservationId);
                if (reservation == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "Reservation not found.";
                    return baseReturn;
                }

                var vehicle = await _vehicleRepository.GetVehicleById(reservation.IdVehicle);
                if (vehicle == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "Vehicle not found.";
                    return baseReturn;
                }

                int days = reservation.RentalDays ?? (reservation.ReturnDate - reservation.RentalDate).Days;
                if (days <= 0)
                {
                    days = 1; // Mínimo 1 dia
                }

                decimal dailyRate = reservation.DailyRate ?? vehicle.DailyRate ?? 0;
                decimal totalCost = days * dailyRate;

                // Adicionar seguros se existirem
                if (reservation.InsuranceVehicle.HasValue)
                {
                    totalCost += reservation.InsuranceVehicle.Value;
                }

                if (reservation.InsuranceThirdParty.HasValue)
                {
                    totalCost += reservation.InsuranceThirdParty.Value;
                }

                if (reservation.TaxAmount.HasValue)
                {
                    totalCost += reservation.TaxAmount.Value;
                }

                baseReturn.Code = "200";
                baseReturn.Message = "Cost calculated successfully.";
                baseReturn.Data = totalCost;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"Internal error: {ex.Message}";
            }

            return baseReturn;
        }

        // US10 - Cliente: Cancelar reserva
        public async Task<BaseReturn<bool>> CancelReservation(int reservationId)
        {
            BaseReturn<bool> baseReturn = new BaseReturn<bool>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();
                if (loggedUser == null)
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    baseReturn.Data = false;
                    return baseReturn;
                }

                bool success = await _reservationRepository.CancelReservation(reservationId);

                baseReturn.Code = success ? "200" : "404";
                baseReturn.Data = success;
                baseReturn.Message = success ? "Reservation cancelled successfully." : "Reservation not found.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"Internal error: {ex.Message}";
                baseReturn.Data = false;
            }

            return baseReturn;
        }

        // US11 - Cliente: Consultar histórico de reservas
        public async Task<BaseReturn<List<Reservation>>> GetReservationHistory(int customerId)
        {
            BaseReturn<List<Reservation>> baseReturn = new BaseReturn<List<Reservation>>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();
                if (loggedUser == null)
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    return baseReturn;
                }

                var tbReservations = await _reservationRepository.GetReservationHistory(customerId);

                if (tbReservations == null || !tbReservations.Any())
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "No reservations found for this customer.";
                    return baseReturn;
                }

                List<Reservation> reservations = new List<Reservation>();
                foreach (var tbReservation in tbReservations)
                {
                    reservations.Add(new Reservation()
                    {
                        Reservationnumber = tbReservation.Reservationnumber,
                        IdCustomer = tbReservation.IdCustomer,
                        IdVehicle = tbReservation.IdVehicle,
                        RentalDate = tbReservation.RentalDate,
                        ReturnDate = tbReservation.ReturnDate,
                        RentalDays = tbReservation.RentalDays,
                        DailyRate = tbReservation.DailyRate,
                        RateType = tbReservation.RateType,
                        InsuranceVehicle = tbReservation.InsuranceVehicle,
                        InsuranceThirdParty = tbReservation.InsuranceThirdParty,
                        TaxAmount = tbReservation.TaxAmount,
                        DamageDescription = tbReservation.DamageDescription,
                    });
                }

                baseReturn.Code = "200";
                baseReturn.Message = "Reservation history obtained successfully.";
                baseReturn.Data = reservations;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"Internal error: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<bool>> RegisterDamage(int reservationNumber, string damageDescription)
        {
            BaseReturn<bool> baseReturn = new BaseReturn<bool>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();
                if (loggedUser == null || (!loggedUser.Roles.Contains("ADMIN") && !loggedUser.Roles.Contains("EMPLOYEE")))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    baseReturn.Data = false;
                    return baseReturn;
                }

                bool success = await _reservationRepository.RegisterDamage(reservationNumber, damageDescription);

                baseReturn.Code = success ? "200" : "404";
                baseReturn.Data = success;
                baseReturn.Message = success ? "Damage registered successfully." : "Reservation not found.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"Internal error: {ex.Message}";
                baseReturn.Data = false;
            }

            return baseReturn;
        }

        // Novos métodos CRUD
        public async Task<BaseReturn<Reservation>> GetReservationById(int reservationId)
        {
            BaseReturn<Reservation> baseReturn = new BaseReturn<Reservation>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();
                if (loggedUser == null)
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    return baseReturn;
                }

                var tbReservation = await _reservationRepository.GetReservationById(reservationId);

                if (tbReservation == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "Reservation not found.";
                    return baseReturn;
                }

                Reservation reservation = new Reservation()
                {
                    Reservationnumber = tbReservation.Reservationnumber,
                    IdCustomer = tbReservation.IdCustomer,
                    IdVehicle = tbReservation.IdVehicle,
                    RentalDate = tbReservation.RentalDate,
                    ReturnDate = tbReservation.ReturnDate,
                    RentalDays = tbReservation.RentalDays,
                    DailyRate = tbReservation.DailyRate,
                    RateType = tbReservation.RateType,
                    InsuranceVehicle = tbReservation.InsuranceVehicle,
                    InsuranceThirdParty = tbReservation.InsuranceThirdParty,
                    TaxAmount = tbReservation.TaxAmount,
                    DamageDescription = tbReservation.DamageDescription,
                };

                baseReturn.Code = "200";
                baseReturn.Message = "Reservation found successfully.";
                baseReturn.Data = reservation;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"Internal error: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<List<Reservation>>> ListAllReservations()
        {
            BaseReturn<List<Reservation>> baseReturn = new BaseReturn<List<Reservation>>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();
                if (loggedUser == null || (!loggedUser.Roles.Contains("ADMIN") && !loggedUser.Roles.Contains("EMPLOYEE")))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    return baseReturn;
                }

                var tbReservations = await _reservationRepository.ListAllReservations();

                if (tbReservations == null || !tbReservations.Any())
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "No reservations found.";
                    return baseReturn;
                }

                List<Reservation> reservations = new List<Reservation>();
                foreach (var tbReservation in tbReservations)
                {
                    reservations.Add(new Reservation()
                    {
                        Reservationnumber = tbReservation.Reservationnumber,
                        IdCustomer = tbReservation.IdCustomer,
                        IdVehicle = tbReservation.IdVehicle,
                        RentalDate = tbReservation.RentalDate,
                        ReturnDate = tbReservation.ReturnDate,
                        RentalDays = tbReservation.RentalDays,
                        DailyRate = tbReservation.DailyRate,
                        RateType = tbReservation.RateType,
                        InsuranceVehicle = tbReservation.InsuranceVehicle,
                        InsuranceThirdParty = tbReservation.InsuranceThirdParty,
                        TaxAmount = tbReservation.TaxAmount,
                        DamageDescription = tbReservation.DamageDescription,
                    });
                }

                baseReturn.Code = "200";
                baseReturn.Message = "Reservation list obtained successfully.";
                baseReturn.Data = reservations;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"Internal error: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<Reservation>> UpdateReservation(Reservation reservation)
        {
            BaseReturn<Reservation> baseReturn = new BaseReturn<Reservation>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();
                if (loggedUser == null || (!loggedUser.Roles.Contains("ADMIN") && !loggedUser.Roles.Contains("EMPLOYEE")))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    return baseReturn;
                }

                LoccarInfra.ORM.model.Reservation tbReservation = new LoccarInfra.ORM.model.Reservation()
                {
                    Reservationnumber = reservation.Reservationnumber,
                    IdCustomer = reservation.IdCustomer,
                    IdVehicle = reservation.IdVehicle,
                    RentalDate = reservation.RentalDate,
                    ReturnDate = reservation.ReturnDate,
                    RentalDays = reservation.RentalDays,
                    DailyRate = reservation.DailyRate,
                    RateType = reservation.RateType,
                    InsuranceVehicle = reservation.InsuranceVehicle,
                    InsuranceThirdParty = reservation.InsuranceThirdParty,
                    TaxAmount = reservation.TaxAmount,
                    DamageDescription = reservation.DamageDescription,
                };

                var updatedReservation = await _reservationRepository.UpdateReservation(tbReservation);

                if (updatedReservation == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "Reservation not found.";
                    return baseReturn;
                }

                baseReturn.Code = "200";
                baseReturn.Data = reservation;
                baseReturn.Message = "Reservation updated successfully.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"Internal error: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<UserReservationSummary>> GetLoggedUserReservationSummary()
        {
            BaseReturn<UserReservationSummary> baseReturn = new BaseReturn<UserReservationSummary>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();
                if (loggedUser == null)
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    return baseReturn;
                }

                // Buscar o usuário para obter o email
                var user = await _userRepository.GetUserById(loggedUser.id);
                if (user == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "User not found.";
                    return baseReturn;
                }

                // Buscar o cliente pelo email
                var customer = await _customerRepository.GetRegistrationByEmail(user.Email);
                if (customer == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "Customer not found.";
                    return baseReturn;
                }

                var summary = await _reservationRepository.GetUserReservationSummary(customer.IdCustomer);

                baseReturn.Code = "200";
                baseReturn.Message = "User reservation summary retrieved successfully.";
                baseReturn.Data = summary;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"Internal error: {ex.Message}";
            }

            return baseReturn;
        }
    }
}
