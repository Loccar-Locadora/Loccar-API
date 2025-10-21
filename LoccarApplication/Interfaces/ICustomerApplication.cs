using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoccarDomain.Customer;
using LoccarDomain;
using LoccarDomain.Customer.Models;

namespace LoccarApplication.Interfaces
{
    public interface ICustomerApplication
    {
        Task<BaseReturn<Customer>> RegisterCustomer(Customer Customer);
        Task<BaseReturn<Customer>> UpdateCustomer(Customer customer);
        Task<BaseReturn<bool>> DeleteCustomer(int customerId);
        
        // Novos métodos CRUD
        Task<BaseReturn<Customer>> GetCustomerById(int customerId);
        Task<BaseReturn<List<Customer>>> ListAllCustomers();
    }
}
