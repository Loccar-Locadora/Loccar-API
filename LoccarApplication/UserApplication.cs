using LoccarApplication.Interfaces;
using LoccarDomain;
using LoccarDomain.User.Models;
using LoccarInfra.Repositories.Interfaces;

namespace LoccarApplication
{
    public class UserApplication : IUserApplication
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;

        public UserApplication(IUserRepository userRepository, ICustomerRepository customerRepository)
        {
            _userRepository = userRepository;
            _customerRepository = customerRepository;
        }

        public async Task<BaseReturn<List<User>>> ListAllUsers()
        {
            BaseReturn<List<User>> baseReturn = new BaseReturn<List<User>>();

            try
            {
                var tabelaUsers = await _userRepository.ListAllUsers();

                if (tabelaUsers == null || !tabelaUsers.Any())
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "No users found.";
                    return baseReturn;
                }

                List<User> users = new List<User>();
                foreach (var tabelaUser in tabelaUsers)
                {
                    // Buscar cellphone na tabela Customer usando GetRegistrationByEmail
                    string cellphone = string.Empty;
                    try
                    {
                        var customerData = await _customerRepository.GetRegistrationByEmail(tabelaUser.Email);
                        if (customerData != null && !string.IsNullOrEmpty(customerData.Phone))
                        {
                            cellphone = customerData.Phone;
                        }
                    }
                    catch
                    {
                        // Se não encontrar customer ou der erro, mantém string vazia
                        cellphone = string.Empty;
                    }

                    users.Add(new User()
                    {
                        Id = tabelaUser.Id,
                        Name = tabelaUser.Username,
                        Email = tabelaUser.Email,
                        Cellphone = cellphone, // Obtido da tabela Customer
                        IsActive = tabelaUser.IsActive,
                        CreatedAt = tabelaUser.CreatedAt,
                        UpdatedAt = tabelaUser.UpdatedAt,
                        Roles = tabelaUser.Roles.Select(r => r.Name).ToList()
                    });
                }

                baseReturn.Code = "200";
                baseReturn.Data = users;
                baseReturn.Message = "User list obtained successfully.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = ex.Message;
            }

            return baseReturn;
        }
    }
}
