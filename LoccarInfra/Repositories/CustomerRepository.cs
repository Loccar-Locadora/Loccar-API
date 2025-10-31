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

        public CustomerRepository(DataBaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Customer> RegisterCustomer(Customer customer)
        {
            await _dbContext.Customers.AddAsync(customer);
            await _dbContext.SaveChangesAsync();

            return customer;
        }

        public async Task<Customer> GetRegistrationByEmail(string email)
        {
            return await _dbContext.Customers.Where(n => n.Email.Equals(email, StringComparison.Ordinal)).FirstOrDefaultAsync();
        }

        public async Task<Customer?> UpdateCustomer(Customer customer)
        {
            var existing = await _dbContext.Customers.FirstOrDefaultAsync(c => c.IdCustomer == customer.IdCustomer);
            if (existing == null)
            {
                return null;
            }

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
    }
}
