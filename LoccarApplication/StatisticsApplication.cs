using LoccarApplication.Common;
using LoccarApplication.Interfaces;
using LoccarDomain;
using LoccarDomain.LoggedUser.Models;
using LoccarDomain.Statistics.Models;
using LoccarInfra.Repositories.Interfaces;
using System.Globalization;

namespace LoccarApplication
{
    public class StatisticsApplication : IStatisticsApplication
    {
        private readonly IAuthApplication _authApplication;
        private readonly ICustomerRepository _customerRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IReservationRepository _reservationRepository;

        public StatisticsApplication(
            IAuthApplication authApplication,
            ICustomerRepository customerRepository,
            IVehicleRepository vehicleRepository,
            IReservationRepository reservationRepository)
        {
            _authApplication = authApplication;
            _customerRepository = customerRepository;
            _vehicleRepository = vehicleRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task<BaseReturn<int>> GetTotalCustomersCount()
        {
            BaseReturn<int> baseReturn = new BaseReturn<int>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();

                // Apenas ADMIN e EMPLOYEE podem acessar estatísticas
                if (!AuthorizationHelper.HasAdminOrEmployeeRole(loggedUser))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    return baseReturn;
                }

                int count = await _customerRepository.GetTotalCustomersCount();

                baseReturn.Code = "200";
                baseReturn.Data = count;
                baseReturn.Message = "Total customers count retrieved successfully.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<int>> GetTotalVehiclesCount()
        {
            BaseReturn<int> baseReturn = new BaseReturn<int>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();

                // Apenas ADMIN e EMPLOYEE podem acessar estatísticas
                if (!AuthorizationHelper.HasAdminOrEmployeeRole(loggedUser))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    return baseReturn;
                }

                int count = await _vehicleRepository.GetTotalVehiclesCount();

                baseReturn.Code = "200";
                baseReturn.Data = count;
                baseReturn.Message = "Total vehicles count retrieved successfully.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<int>> GetActiveReservationsCount()
        {
            BaseReturn<int> baseReturn = new BaseReturn<int>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();

                // Apenas ADMIN e EMPLOYEE podem acessar estatísticas
                if (!AuthorizationHelper.HasAdminOrEmployeeRole(loggedUser))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    return baseReturn;
                }

                int count = await _reservationRepository.GetActiveReservationsCount();

                baseReturn.Code = "200";
                baseReturn.Data = count;
                baseReturn.Message = "Active reservations count retrieved successfully.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<int>> GetAvailableVehiclesCount()
        {
            BaseReturn<int> baseReturn = new BaseReturn<int>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();

                // Qualquer usuário autenticado pode ver veículos disponíveis
                if (!AuthorizationHelper.IsAuthenticated(loggedUser))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    return baseReturn;
                }

                int count = await _vehicleRepository.GetAvailableVehiclesCount();

                baseReturn.Code = "200";
                baseReturn.Data = count;
                baseReturn.Message = "Available vehicles count retrieved successfully.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<SystemStatistics>> GetSystemStatistics()
        {
            BaseReturn<SystemStatistics> baseReturn = new BaseReturn<SystemStatistics>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();

                // Apenas ADMIN e EMPLOYEE podem acessar estatísticas completas
                if (!AuthorizationHelper.HasAdminOrEmployeeRole(loggedUser))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    return baseReturn;
                }

                // Executar todas as consultas em paralelo para melhor performance
                Task<int> totalCustomersTask = _customerRepository.GetTotalCustomersCount();
                Task<int> totalVehiclesTask = _vehicleRepository.GetTotalVehiclesCount();
                Task<int> activeReservationsTask = _reservationRepository.GetActiveReservationsCount();
                Task<int> availableVehiclesTask = _vehicleRepository.GetAvailableVehiclesCount();

                await Task.WhenAll(totalCustomersTask, totalVehiclesTask, activeReservationsTask, availableVehiclesTask);

                SystemStatistics statistics = new SystemStatistics
                {
                    TotalCustomers = totalCustomersTask.Result,
                    TotalVehicles = totalVehiclesTask.Result,
                    ActiveReservations = activeReservationsTask.Result,
                    AvailableVehicles = availableVehiclesTask.Result,
                    GeneratedAt = DateTime.Now
                };

                baseReturn.Code = "200";
                baseReturn.Data = statistics;
                baseReturn.Message = "System statistics retrieved successfully.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        // Métodos de receita
        public async Task<BaseReturn<MonthlyRevenue>> GetMonthlyRevenue(int year, int month)
        {
            BaseReturn<MonthlyRevenue> baseReturn = new BaseReturn<MonthlyRevenue>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();

                // Apenas ADMIN e EMPLOYEE podem acessar receitas
                if (!AuthorizationHelper.HasAdminOrEmployeeRole(loggedUser))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    return baseReturn;
                }

                if (month < 1 || month > 12)
                {
                    baseReturn.Code = "400";
                    baseReturn.Message = "Invalid month. Must be between 1 and 12.";
                    return baseReturn;
                }

                var reservations = await _reservationRepository.GetReservationsByMonth(year, month);
                decimal totalRevenue = await _reservationRepository.GetMonthlyRevenue(year, month);

                var monthlyRevenue = new MonthlyRevenue
                {
                    Year = year,
                    Month = month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    TotalRevenue = totalRevenue,
                    TotalReservations = reservations.Count,
                    AverageRevenuePerReservation = reservations.Count > 0 ? totalRevenue / reservations.Count : 0,
                    GeneratedAt = DateTime.Now
                };

                baseReturn.Code = "200";
                baseReturn.Data = monthlyRevenue;
                baseReturn.Message = "Monthly revenue retrieved successfully.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<MonthlyRevenueDetailed>> GetMonthlyRevenueDetailed(int year, int month)
        {
            BaseReturn<MonthlyRevenueDetailed> baseReturn = new BaseReturn<MonthlyRevenueDetailed>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();

                // Apenas ADMIN e EMPLOYEE podem acessar receitas
                if (!AuthorizationHelper.HasAdminOrEmployeeRole(loggedUser))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    return baseReturn;
                }

                if (month < 1 || month > 12)
                {
                    baseReturn.Code = "400";
                    baseReturn.Message = "Invalid month. Must be between 1 and 12.";
                    return baseReturn;
                }

                var reservations = await _reservationRepository.GetReservationsByMonth(year, month);
                
                decimal baseRevenue = 0;
                decimal insuranceRevenue = 0;
                decimal taxRevenue = 0;

                foreach (var reservation in reservations)
                {
                    // Calcular receita base
                    int days = reservation.RentalDays ?? (reservation.ReturnDate - reservation.RentalDate).Days;
                    if (days <= 0) days = 1;

                    decimal dailyRate = reservation.DailyRate ?? reservation.IdVehicleNavigation?.DailyRate ?? 0;
                    baseRevenue += days * dailyRate;

                    // Somar seguros e taxas
                    insuranceRevenue += (reservation.InsuranceVehicle ?? 0) + (reservation.InsuranceThirdParty ?? 0);
                    taxRevenue += reservation.TaxAmount ?? 0;
                }

                decimal totalRevenue = baseRevenue + insuranceRevenue + taxRevenue;

                var monthlyRevenueDetailed = new MonthlyRevenueDetailed
                {
                    Year = year,
                    Month = month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    Revenue = new RevenueBreakdown
                    {
                        BaseRevenue = baseRevenue,
                        InsuranceRevenue = insuranceRevenue,
                        TaxRevenue = taxRevenue,
                        TotalRevenue = totalRevenue
                    },
                    TotalReservations = reservations.Count,
                    AverageRevenuePerReservation = reservations.Count > 0 ? totalRevenue / reservations.Count : 0,
                    GeneratedAt = DateTime.Now
                };

                baseReturn.Code = "200";
                baseReturn.Data = monthlyRevenueDetailed;
                baseReturn.Message = "Detailed monthly revenue retrieved successfully.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<MonthlyRevenue>> GetCurrentMonthRevenue()
        {
            var now = DateTime.Now;
            return await GetMonthlyRevenue(now.Year, now.Month);
        }

        public async Task<BaseReturn<decimal>> GetYearRevenue(int year)
        {
            BaseReturn<decimal> baseReturn = new BaseReturn<decimal>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();

                // Apenas ADMIN e EMPLOYEE podem acessar receitas
                if (!AuthorizationHelper.HasAdminOrEmployeeRole(loggedUser))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    return baseReturn;
                }

                decimal yearRevenue = await _reservationRepository.GetYearRevenue(year);

                baseReturn.Code = "200";
                baseReturn.Data = yearRevenue;
                baseReturn.Message = $"Year {year} revenue retrieved successfully.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<List<MonthlyRevenue>>> GetYearlyRevenueBreakdown(int year)
        {
            BaseReturn<List<MonthlyRevenue>> baseReturn = new BaseReturn<List<MonthlyRevenue>>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();

                // Apenas ADMIN e EMPLOYEE podem acessar receitas
                if (!AuthorizationHelper.HasAdminOrEmployeeRole(loggedUser))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized.";
                    return baseReturn;
                }

                var monthlyRevenues = new List<MonthlyRevenue>();

                // Executar consultas em paralelo para todos os meses
                var tasks = new List<Task<MonthlyRevenue>>();

                for (int month = 1; month <= 12; month++)
                {
                    tasks.Add(GetMonthlyRevenueData(year, month));
                }

                var results = await Task.WhenAll(tasks);
                monthlyRevenues.AddRange(results);

                baseReturn.Code = "200";
                baseReturn.Data = monthlyRevenues;
                baseReturn.Message = $"Yearly revenue breakdown for {year} retrieved successfully.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        private async Task<MonthlyRevenue> GetMonthlyRevenueData(int year, int month)
        {
            var reservations = await _reservationRepository.GetReservationsByMonth(year, month);
            decimal totalRevenue = await _reservationRepository.GetMonthlyRevenue(year, month);

            return new MonthlyRevenue
            {
                Year = year,
                Month = month,
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                TotalRevenue = totalRevenue,
                TotalReservations = reservations.Count,
                AverageRevenuePerReservation = reservations.Count > 0 ? totalRevenue / reservations.Count : 0,
                GeneratedAt = DateTime.Now
            };
        }
    }
}
