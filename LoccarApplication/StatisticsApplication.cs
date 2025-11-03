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
        private readonly IUserRepository _userRepository;

        public StatisticsApplication(
            IAuthApplication authApplication,
            ICustomerRepository customerRepository,
            IVehicleRepository vehicleRepository,
            IReservationRepository reservationRepository,
            IUserRepository userRepository)
        {
            _authApplication = authApplication;
            _customerRepository = customerRepository;
            _vehicleRepository = vehicleRepository;
            _reservationRepository = reservationRepository;
            _userRepository = userRepository;
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

                // Executar consultas sequencialmente para evitar problemas de concorrência
                int totalCustomers = await _customerRepository.GetTotalCustomersCount();
                int totalVehicles = await _vehicleRepository.GetTotalVehiclesCount();
                int activeReservations = await _reservationRepository.GetActiveReservationsCount();
                int availableVehicles = await _vehicleRepository.GetAvailableVehiclesCount();

                SystemStatistics statistics = new SystemStatistics
                {
                    TotalCustomers = totalCustomers,
                    TotalVehicles = totalVehicles,
                    ActiveReservations = activeReservations,
                    AvailableVehicles = availableVehicles,
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

        // Métodos de estatísticas de usuários
        public async Task<BaseReturn<UserStatistics>> GetUserStatistics()
        {
            BaseReturn<UserStatistics> baseReturn = new BaseReturn<UserStatistics>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();

                // Apenas CLIENT_ADMIN pode acessar estatísticas de usuários
                if (!AuthorizationHelper.IsAdmin(loggedUser))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized. Only administrators can access user statistics.";
                    return baseReturn;
                }

                // Usar método otimizado que faz uma única consulta
                var statisticsData = await _userRepository.GetUserStatisticsData();

                var userStats = new UserStatistics
                {
                    TotalUsers = statisticsData.TotalUsers,
                    ActiveUsers = statisticsData.ActiveUsers,
                    InactiveUsers = statisticsData.TotalUsers - statisticsData.ActiveUsers,
                    AdminUsers = statisticsData.RoleCounts.GetValueOrDefault("CLIENT_ADMIN", 0),
                    EmployeeUsers = statisticsData.RoleCounts.GetValueOrDefault("CLIENT_EMPLOYEE", 0),
                    CommonUsers = statisticsData.RoleCounts.GetValueOrDefault("CLIENT_USER", 0),
                    GeneratedAt = DateTime.Now
                };

                baseReturn.Code = "200";
                baseReturn.Data = userStats;
                baseReturn.Message = "User statistics retrieved successfully.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<DetailedUserStatistics>> GetDetailedUserStatistics()
        {
            BaseReturn<DetailedUserStatistics> baseReturn = new BaseReturn<DetailedUserStatistics>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();

                // Apenas CLIENT_ADMIN pode acessar estatísticas detalhadas de usuários
                if (!AuthorizationHelper.IsAdmin(loggedUser))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized. Only administrators can access detailed user statistics.";
                    return baseReturn;
                }

                // Usar método otimizado que faz uma única consulta
                var statisticsData = await _userRepository.GetUserStatisticsData();

                var roleBreakdown = statisticsData.RoleCounts.Select(kvp => new UserRoleBreakdown
                {
                    RoleName = kvp.Key,
                    UserCount = kvp.Value,
                    Percentage = statisticsData.TotalUsers > 0 
                        ? Math.Round((decimal)kvp.Value / statisticsData.TotalUsers * 100, 2) 
                        : 0
                }).ToList();

                var detailedStats = new DetailedUserStatistics
                {
                    TotalUsers = statisticsData.TotalUsers,
                    ActiveUsers = statisticsData.ActiveUsers,
                    InactiveUsers = statisticsData.TotalUsers - statisticsData.ActiveUsers,
                    RoleBreakdown = roleBreakdown,
                    GeneratedAt = DateTime.Now
                };

                baseReturn.Code = "200";
                baseReturn.Data = detailedStats;
                baseReturn.Message = "Detailed user statistics retrieved successfully.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }

        public async Task<BaseReturn<int>> GetUsersByRoleCount(string roleName)
        {
            BaseReturn<int> baseReturn = new BaseReturn<int>();

            try
            {
                LoggedUser loggedUser = _authApplication.GetLoggedUser();

                // Apenas CLIENT_ADMIN pode acessar contagem por role
                if (!AuthorizationHelper.IsAdmin(loggedUser))
                {
                    baseReturn.Code = "401";
                    baseReturn.Message = "User not authorized. Only administrators can access user role statistics.";
                    return baseReturn;
                }

                if (string.IsNullOrEmpty(roleName))
                {
                    baseReturn.Code = "400";
                    baseReturn.Message = "Role name is required.";
                    return baseReturn;
                }

                int count = await _userRepository.GetUsersByRoleCount(roleName.ToUpper());

                baseReturn.Code = "200";
                baseReturn.Data = count;
                baseReturn.Message = $"Users with role '{roleName}' count retrieved successfully.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return baseReturn;
        }
    }
}
