using LoccarDomain;
using LoccarDomain.Statistics.Models;

namespace LoccarApplication.Interfaces
{
    public interface IStatisticsApplication
    {
        Task<BaseReturn<int>> GetTotalCustomersCount();
        Task<BaseReturn<int>> GetTotalVehiclesCount();
        Task<BaseReturn<int>> GetActiveReservationsCount();
        Task<BaseReturn<int>> GetAvailableVehiclesCount();
        Task<BaseReturn<SystemStatistics>> GetSystemStatistics();

        // Métodos de receita
        Task<BaseReturn<MonthlyRevenue>> GetMonthlyRevenue(int year, int month);
        Task<BaseReturn<MonthlyRevenueDetailed>> GetMonthlyRevenueDetailed(int year, int month);
        Task<BaseReturn<MonthlyRevenue>> GetCurrentMonthRevenue();
        Task<BaseReturn<decimal>> GetYearRevenue(int year);
        Task<BaseReturn<List<MonthlyRevenue>>> GetYearlyRevenueBreakdown(int year);

        // Métodos de estatísticas de usuários
        Task<BaseReturn<UserStatistics>> GetUserStatistics();
        Task<BaseReturn<DetailedUserStatistics>> GetDetailedUserStatistics();
        Task<BaseReturn<int>> GetUsersByRoleCount(string roleName);
    }
}
