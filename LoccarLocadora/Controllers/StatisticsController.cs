using LoccarApplication.Interfaces;
using LoccarDomain;
using LoccarDomain.Statistics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoccarWebapi.Controllers
{
    [Route("api/statistics")]
    [ApiController]
    [Authorize]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsApplication _statisticsApplication;

        public StatisticsController(IStatisticsApplication statisticsApplication)
        {
            _statisticsApplication = statisticsApplication;
        }

        /// <summary>
        /// Obtém o total de clientes cadastrados no sistema
        /// </summary>
        /// <returns>Número total de clientes</returns>
        [HttpGet("customers/count")]
        public async Task<BaseReturn<int>> GetTotalCustomersCount()
        {
            return await _statisticsApplication.GetTotalCustomersCount();
        }

        /// <summary>
        /// Obtém o total de veículos cadastrados no sistema
        /// </summary>
        /// <returns>Número total de veículos</returns>
        [HttpGet("vehicles/count")]
        public async Task<BaseReturn<int>> GetTotalVehiclesCount()
        {
            return await _statisticsApplication.GetTotalVehiclesCount();
        }

        /// <summary>
        /// Obtém o total de reservas ativas no sistema
        /// </summary>
        /// <returns>Número de reservas ativas</returns>
        [HttpGet("reservations/active/count")]
        public async Task<BaseReturn<int>> GetActiveReservationsCount()
        {
            return await _statisticsApplication.GetActiveReservationsCount();
        }

        /// <summary>
        /// Obtém o total de veículos disponíveis para locação
        /// </summary>
        /// <returns>Número de veículos disponíveis</returns>
        [HttpGet("vehicles/available/count")]
        public async Task<BaseReturn<int>> GetAvailableVehiclesCount()
        {
            return await _statisticsApplication.GetAvailableVehiclesCount();
        }

        /// <summary>
        /// Obtém todas as estatísticas do sistema em uma única chamada
        /// </summary>
        /// <returns>Objeto com todas as estatísticas do sistema</returns>
        [HttpGet("system")]
        public async Task<BaseReturn<SystemStatistics>> GetSystemStatistics()
        {
            return await _statisticsApplication.GetSystemStatistics();
        }

        /// <summary>
        /// Obtém a receita de um mês específico
        /// </summary>
        /// <param name="year">Ano</param>
        /// <param name="month">Mês (1-12)</param>
        /// <returns>Receita mensal</returns>
        [HttpGet("revenue/monthly/{year}/{month}")]
        public async Task<BaseReturn<MonthlyRevenue>> GetMonthlyRevenue(int year, int month)
        {
            return await _statisticsApplication.GetMonthlyRevenue(year, month);
        }

        /// <summary>
        /// Obtém a receita detalhada de um mês específico com breakdown por categoria
        /// </summary>
        /// <param name="year">Ano</param>
        /// <param name="month">Mês (1-12)</param>
        /// <returns>Receita mensal detalhada</returns>
        [HttpGet("revenue/monthly/{year}/{month}/detailed")]
        public async Task<BaseReturn<MonthlyRevenueDetailed>> GetMonthlyRevenueDetailed(int year, int month)
        {
            return await _statisticsApplication.GetMonthlyRevenueDetailed(year, month);
        }

        /// <summary>
        /// Obtém a receita do mês atual
        /// </summary>
        /// <returns>Receita do mês atual</returns>
        [HttpGet("revenue/monthly/current")]
        public async Task<BaseReturn<MonthlyRevenue>> GetCurrentMonthRevenue()
        {
            return await _statisticsApplication.GetCurrentMonthRevenue();
        }

        /// <summary>
        /// Obtém a receita total de um ano
        /// </summary>
        /// <param name="year">Ano</param>
        /// <returns>Receita anual</returns>
        [HttpGet("revenue/yearly/{year}")]
        public async Task<BaseReturn<decimal>> GetYearRevenue(int year)
        {
            return await _statisticsApplication.GetYearRevenue(year);
        }

        /// <summary>
        /// Obtém o breakdown da receita por mês de um ano específico
        /// </summary>
        /// <param name="year">Ano</param>
        /// <returns>Lista com receita de cada mês do ano</returns>
        [HttpGet("revenue/yearly/{year}/breakdown")]
        public async Task<BaseReturn<List<MonthlyRevenue>>> GetYearlyRevenueBreakdown(int year)
        {
            return await _statisticsApplication.GetYearlyRevenueBreakdown(year);
        }

        /// <summary>
        /// Obtém estatísticas básicas de usuários do sistema
        /// </summary>
        /// <returns>Estatísticas de usuários incluindo total, ativos, e contagem por role</returns>
        [HttpGet("users")]
        public async Task<BaseReturn<UserStatistics>> GetUserStatistics()
        {
            return await _statisticsApplication.GetUserStatistics();
        }

        /// <summary>
        /// Obtém estatísticas detalhadas de usuários com breakdown por role
        /// </summary>
        /// <returns>Estatísticas detalhadas incluindo percentuais por role</returns>
        [HttpGet("users/detailed")]
        public async Task<BaseReturn<DetailedUserStatistics>> GetDetailedUserStatistics()
        {
            return await _statisticsApplication.GetDetailedUserStatistics();
        }

        /// <summary>
        /// Obtém a contagem de usuários por role específica
        /// </summary>
        /// <param name="roleName">Nome da role (ADMIN, EMPLOYEE, COMMON_USER)</param>
        /// <returns>Número de usuários com a role especificada</returns>
        [HttpGet("users/role/{roleName}")]
        public async Task<BaseReturn<int>> GetUsersByRoleCount(string roleName)
        {
            return await _statisticsApplication.GetUsersByRoleCount(roleName);
        }
    }
}
