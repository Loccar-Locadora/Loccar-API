using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoccarInfra.ORM.model;

namespace LoccarInfra.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> GetRegistrationByEmail(string email);

        Task<Customer> RegisterCustomer(Customer customer);

        Task<Customer?> UpdateCustomer(Customer customer);

        Task<bool> DeleteCustomer(int customerId);

        Task<Customer> GetCustomerById(int customerId);

        Task<List<Customer>> ListAllCustomers();

        // Métodos de estatísticas
        Task<int> GetTotalCustomersCount();
    }
}
