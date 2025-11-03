using LoccarInfra.ORM.model;
using LoccarInfra.Repositories.Interfaces;
using LoccarDomain.Statistics.Models;
using Microsoft.EntityFrameworkCore;

namespace LoccarInfra.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataBaseContext _context;
        private readonly ICustomerRepository _customerRepository;

        public UserRepository(DataBaseContext context, ICustomerRepository customerRepository)
        {
            _context = context;
            _customerRepository = customerRepository;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users
                .Include(u => u.Roles)
                .ToListAsync();
        }

        public async Task<User> GetUserById(int userId)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<List<User>> GetUsersByRole(string roleName)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .Where(u => u.Roles.Any(r => r.Name == roleName))
                .ToListAsync();
        }

        public async Task<int> GetTotalUsersCount()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> GetActiveUsersCount()
        {
            return await _context.Users.CountAsync(u => u.IsActive == true);
        }

        public async Task<int> GetUsersByRoleCount(string roleName)
        {
            return await _context.Users
                .CountAsync(u => u.Roles.Any(r => r.Name == roleName));
        }

        public async Task<Dictionary<string, int>> GetUserCountByRole()
        {
            var roleStats = await _context.Roles
                .Select(r => new { 
                    RoleName = r.Name, 
                    UserCount = r.Users.Count 
                })
                .ToDictionaryAsync(x => x.RoleName, x => x.UserCount);

            return roleStats;
        }

        public async Task<UserStatisticsData> GetUserStatisticsData()
        {
            // Carregar todos os usuários com suas roles em uma única consulta
            var usersWithRoles = await _context.Users
                .Include(u => u.Roles)
                .Select(u => new
                {
                    u.Id,
                    IsActive = u.IsActive ?? false,
                    RoleNames = u.Roles.Select(r => r.Name).ToList()
                })
                .ToListAsync();

            var totalUsers = usersWithRoles.Count;
            var activeUsers = usersWithRoles.Count(u => u.IsActive);

            // Contar usuários por role
            var roleCounts = new Dictionary<string, int>();
            
            foreach (var user in usersWithRoles)
            {
                foreach (var roleName in user.RoleNames)
                {
                    if (roleCounts.TryGetValue(roleName, out int currentCount))
                    {
                        roleCounts[roleName] = currentCount + 1;
                    }
                    else
                    {
                        roleCounts[roleName] = 1;
                    }
                }
            }

            return new UserStatisticsData
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                RoleCounts = roleCounts
            };
        }

        public async Task<List<User>> ListAllUsers()
        {
            return await _context.Users
                .Include(u => u.Roles)
                .Where(u => u.IsActive == true)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }
    }
}
