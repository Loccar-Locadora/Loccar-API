using LoccarInfra.ORM.model;
using LoccarDomain.Statistics.Models;

namespace LoccarInfra.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();

        Task<User> GetUserById(int userId);

        Task<List<User>> GetUsersByRole(string roleName);

        Task<int> GetTotalUsersCount();

        Task<int> GetActiveUsersCount();

        Task<int> GetUsersByRoleCount(string roleName);

        Task<Dictionary<string, int>> GetUserCountByRole();

        Task<UserStatisticsData> GetUserStatisticsData();

        Task<List<User>> ListAllUsers();
    }
}
