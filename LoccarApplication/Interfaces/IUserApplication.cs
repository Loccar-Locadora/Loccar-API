using LoccarDomain;
using LoccarDomain.User.Models;

namespace LoccarApplication.Interfaces
{
    public interface IUserApplication
    {
        Task<BaseReturn<List<User>>> ListAllUsers();
    }
}
