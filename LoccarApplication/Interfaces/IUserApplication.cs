using LoccarDomain;
using LoccarDomain.User.Models;
using LoccarDomain.Customer.Models;

namespace LoccarApplication.Interfaces
{
    public interface IUserApplication
    {
        Task<BaseReturn<List<User>>> ListAllUsers();
        Task<BaseReturn<object>> DeleteUser(int userId);
        Task<BaseReturn<User>> UpdateUser(int userId, Customer customerData);
        Task<BaseReturn<LoccarDomain.Customer.Models.Customer>> GetUserById(int userId);
        Task<BaseReturn<User>> GetUserByEmail(string email);
    }
}
