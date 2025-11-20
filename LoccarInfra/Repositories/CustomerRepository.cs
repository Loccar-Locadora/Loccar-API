using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoccarInfra.ORM.model;
using LoccarInfra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LoccarInfra.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DataBaseContext _dbContext;
        private readonly IUserRepository _userRepository;

        public CustomerRepository(DataBaseContext dbContext, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
        }

        public async Task<Customer> RegisterCustomer(Customer customer)
        {
            await _dbContext.Customers.AddAsync(customer);
            await _dbContext.SaveChangesAsync();

            return customer;
        }

        public async Task<Customer> GetRegistrationByEmail(string email)
        {
            return await _dbContext.Customers
                .Where(c => c.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<Customer?> UpdateCustomer(Customer customer)
        {
            var existing = await _dbContext.Customers.FirstOrDefaultAsync(c => c.IdCustomer == customer.IdCustomer);
            if (existing == null)
            {
                return null;
            }

            // Atualizar dados do Customer
            existing.Name = customer.Name;
            existing.Email = customer.Email;
            existing.Phone = customer.Phone;
            existing.DriverLicense = customer.DriverLicense;

            // Buscar e atualizar User associado pelo email
            if (!string.IsNullOrEmpty(customer.Email))
            {
                var associatedUser = await _userRepository.FindUserByEmail(customer.Email);
                if (associatedUser != null)
                {
                    // Atualizar dados do User
                    associatedUser.Username = customer.Name;
                    associatedUser.Email = customer.Email;
                    associatedUser.UpdatedAt = DateTime.Now; // Mudando de DateTime.UtcNow para DateTime.Now

                    await _userRepository.UpdateUser(associatedUser);
                }
            }

            await _dbContext.SaveChangesAsync();
            return existing;
        }

        // Método para atualizar apenas o customer, sem tocar no user
        public async Task<Customer?> UpdateCustomerOnly(Customer customer)
        {
            var existing = await _dbContext.Customers.FirstOrDefaultAsync(c => c.IdCustomer == customer.IdCustomer);
            if (existing == null)
            {
                return null;
            }

            // Atualizar apenas os dados do Customer
            existing.Name = customer.Name;
            existing.Email = customer.Email;
            existing.Phone = customer.Phone;
            existing.DriverLicense = customer.DriverLicense;

            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteCustomer(int customerId)
        {
            var existing = await _dbContext.Customers.FirstOrDefaultAsync(c => c.IdCustomer == customerId);
            if (existing == null)
            {
                return false;
            }

            // Remover apenas o customer, sem deletar o user
            // O user pode existir sem ser um customer
            _dbContext.Customers.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Novos métodos CRUD
        public async Task<Customer> GetCustomerById(int customerId)
        {
            return await _dbContext.Customers.FirstOrDefaultAsync(c => c.IdCustomer == customerId);
        }

        public async Task<List<Customer>> ListAllCustomers()
        {
            return await _dbContext.Customers.ToListAsync();
        }

        // Métodos de estatísticas
        public async Task<int> GetTotalCustomersCount()
        {
            return await _dbContext.Customers.CountAsync();
        }
    }
}
