using LoccarApplication.Interfaces;
using LoccarDomain;
using LoccarDomain.Customer.Models;
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
                    LoccarInfra.ORM.model.Customer customerData = new LoccarInfra.ORM.model.Customer();
                    try
                    {
                        customerData = await _customerRepository.GetRegistrationByEmail(tabelaUser.Email);
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
                        DriverLicense = customerData?.DriverLicense ?? null,
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

        public async Task<BaseReturn<object>> DeleteUser(int userId)
        {
            BaseReturn<object> baseReturn = new BaseReturn<object>();

            try
            {
                // Primeiro, buscar o usuário para obter o email
                var user = await _userRepository.GetUserById(userId);
                if (user == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "User not found.";
                    return baseReturn;
                }

                // Buscar o customer correspondente pelo email (se existir)
                var customer = await _customerRepository.GetRegistrationByEmail(user.Email);

                // Deletar o usuário
                bool userDeleted = await _userRepository.DeleteUser(userId);
                if (!userDeleted)
                {
                    baseReturn.Code = "500";
                    baseReturn.Message = "Failed to delete user.";
                    return baseReturn;
                }

                // Se existir customer associado, deletá-lo também
                if (customer != null)
                {
                    await _customerRepository.DeleteCustomer(customer.IdCustomer);
                }

                baseReturn.Code = "200";
                baseReturn.Message = "User deleted successfully.";
                baseReturn.Data = null;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = ex.Message;
            }

            return baseReturn;
        }

        public async Task<BaseReturn<LoccarDomain.Customer.Models.Customer>> GetUserById(int userId)
        {
            BaseReturn<LoccarDomain.Customer.Models.Customer> baseReturn = new BaseReturn<LoccarDomain.Customer.Models.Customer>();

            try
            {
                // Primeiro, buscar o usuário para obter o email
                var user = await _userRepository.GetUserById(userId);
                if (user == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "User not found.";
                    return baseReturn;
                }

                // Buscar o customer correspondente pelo email (se existir)
                var customer = await _customerRepository.GetRegistrationByEmail(user.Email);

                LoccarDomain.Customer.Models.Customer customerData = new LoccarDomain.Customer.Models.Customer()
                {
                    Username = customer.Name,
                    Email = customer.Email,
                    DriverLicense = customer.DriverLicense,
                    Cellphone = customer.Phone
                };

                baseReturn.Code = "200";
                baseReturn.Message = "User deleted successfully.";
                baseReturn.Data = customerData;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = ex.Message;
            }

            return baseReturn;
        }

        public async Task<BaseReturn<User>> UpdateUser(int userId, Customer customerData)
        {
            BaseReturn<User> baseReturn = new BaseReturn<User>();

            try
            {
                // Primeiro, buscar o usuário para verificar se existe
                var existingUser = await _userRepository.GetUserById(userId);
                if (existingUser == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "User not found.";
                    return baseReturn;
                }

                // Buscar o customer correspondente pelo email atual (se existir)
                var existingCustomer = await _customerRepository.GetRegistrationByEmail(existingUser.Email);

                // Atualizar os dados do usuário
                if (!string.IsNullOrEmpty(customerData.Username))
                {
                    existingUser.Username = customerData.Username;
                }
                
                if (!string.IsNullOrEmpty(customerData.Email))
                {
                    existingUser.Email = customerData.Email;
                }
                
                // Usar DateTime.Now ao invés de DateTime.UtcNow para compatibilidade com PostgreSQL timestamp without time zone
                existingUser.UpdatedAt = DateTime.Now;

                // Salvar as mudanças no usuário
                var updatedUser = await _userRepository.UpdateUser(existingUser);
                if (updatedUser == null)
                {
                    baseReturn.Code = "500";
                    baseReturn.Message = "Failed to update user.";
                    return baseReturn;
                }

                // Se existir customer associado, atualizá-lo também usando UpdateCustomerOnly
                if (existingCustomer != null)
                {
                    if (!string.IsNullOrEmpty(customerData.Username))
                    {
                        existingCustomer.Name = customerData.Username;
                    }
                    
                    if (!string.IsNullOrEmpty(customerData.Email))
                    {
                        existingCustomer.Email = customerData.Email;
                    }
                    
                    if (!string.IsNullOrEmpty(customerData.Cellphone))
                    {
                        existingCustomer.Phone = customerData.Cellphone;
                    }

                    if (!string.IsNullOrEmpty(customerData.DriverLicense))
                    {
                        existingCustomer.DriverLicense = customerData.DriverLicense;
                    }

                    await _customerRepository.UpdateCustomerOnly(existingCustomer);
                }

                // Retornar o usuário atualizado no formato do domínio
                var userResponse = new User
                {
                    Id = updatedUser.Id,
                    Name = updatedUser.Username,
                    Email = updatedUser.Email,
                    Cellphone = existingCustomer?.Phone ?? string.Empty,
                    IsActive = updatedUser.IsActive,
                    CreatedAt = updatedUser.CreatedAt,
                    UpdatedAt = updatedUser.UpdatedAt,
                    Roles = updatedUser.Roles.Select(r => r.Name).ToList()
                };

                baseReturn.Code = "200";
                baseReturn.Message = "User updated successfully.";
                baseReturn.Data = userResponse;
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = ex.Message;
            }

            return baseReturn;
        }
        public async Task<BaseReturn<User>> GetUserByEmail(string email)
        {
            BaseReturn<User> baseReturn = new BaseReturn<User>();

            try
            {
                var user = await _userRepository.GetUserByEmail(email);
                if (user == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "User not found.";
                    return baseReturn;
                }

                // Buscar o customer correspondente pelo email (se existir)
                var customer = await _customerRepository.GetRegistrationByEmail(user.Email);

                LoccarDomain.User.Models.User userData = new LoccarDomain.User.Models.User()
                {
                    Name = customer.Name,
                    Email = customer.Email,
                    DriverLicense = customer.DriverLicense,
                    Cellphone = customer.Phone,
                    Roles = user.Roles.Select(r => r.Name).ToList()
                };

                baseReturn.Code = "200";
                baseReturn.Message = "User deleted successfully.";
                baseReturn.Data = userData;
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
