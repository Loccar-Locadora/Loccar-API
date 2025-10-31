using System.Security.Cryptography;
using System.Text;
using LoccarApplication.Interfaces;
using LoccarDomain;
using LoccarDomain.Customer.Models;
using LoccarInfra.Repositories;
using LoccarInfra.Repositories.Interfaces;

namespace LoccarApplication
{
    public class CustomerApplication : ICustomerApplication
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerApplication(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<BaseReturn<Customer>> RegisterCustomer(Customer customer)
        {
            BaseReturn<Customer> baseReturn = new BaseReturn<Customer>();

            try
            {
                LoccarInfra.ORM.model.Customer tabelaCustomer = new LoccarInfra.ORM.model.Customer()
                {
                    Name = customer.Username,
                    Email = customer.Email,
                    Phone = customer.Cellphone,
                    DriverLicense = customer.DriverLicense,
                    Created = DateTime.Now,
                };

                var response = await _customerRepository.RegisterCustomer(tabelaCustomer);

                Customer customerResponse = new Customer()
                {
                    Username = response.Name,
                    Email = response.Email,
                    Cellphone = response.Phone,
                    DriverLicense = response.DriverLicense,
                    Created = response.Created,
                };

                baseReturn.Code = "200";
                baseReturn.Data = customerResponse;
                baseReturn.Message = "Customer registered successfully.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = ex.Message;
            }

            return baseReturn;
        }

        public async Task<BaseReturn<Customer>> UpdateCustomer(Customer customer)
        {
            BaseReturn<Customer> baseReturn = new BaseReturn<Customer>();

            try
            {
                LoccarInfra.ORM.model.Customer tabelaCustomer = new LoccarInfra.ORM.model.Customer()
                {
                    IdCustomer = (int)customer.IdCustomer,
                    Name = customer.Username,
                    Email = customer.Email,
                    Phone = customer.Cellphone,
                    DriverLicense = customer.DriverLicense,
                };

                var response = await _customerRepository.UpdateCustomer(tabelaCustomer);

                if (response == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "Customer not found.";
                    baseReturn.Data = null;
                    return baseReturn;
                }

                Customer customerResponse = new Customer()
                {
                    Username = response.Name,
                    Email = response.Email,
                    Cellphone = response.Phone,
                    DriverLicense = response.DriverLicense,
                    Created = response.Created,
                };

                baseReturn.Code = "200";
                baseReturn.Data = customerResponse;
                baseReturn.Message = "Customer updated successfully.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = ex.Message;
            }

            return baseReturn;
        }

        public async Task<BaseReturn<bool>> DeleteCustomer(int customerId)
        {
            BaseReturn<bool> baseReturn = new BaseReturn<bool>();

            try
            {
                bool success = await _customerRepository.DeleteCustomer(customerId);

                baseReturn.Code = success ? "200" : "404";
                baseReturn.Data = success;
                baseReturn.Message = success ? "Customer deleted successfully." : "Customer not found.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Data = false;
                baseReturn.Message = ex.Message;
            }

            return baseReturn;
        }

        // Novos métodos CRUD
        public async Task<BaseReturn<Customer>> GetCustomerById(int customerId)
        {
            BaseReturn<Customer> baseReturn = new BaseReturn<Customer>();

            try
            {
                var tabelaCustomer = await _customerRepository.GetCustomerById(customerId);

                if (tabelaCustomer == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "Customer not found.";
                    return baseReturn;
                }

                Customer customerResponse = new Customer()
                {
                    IdCustomer = tabelaCustomer.IdCustomer,
                    Username = tabelaCustomer.Name,
                    Email = tabelaCustomer.Email,
                    Cellphone = tabelaCustomer.Phone,
                    DriverLicense = tabelaCustomer.DriverLicense,
                    Created = tabelaCustomer.Created,
                };

                baseReturn.Code = "200";
                baseReturn.Data = customerResponse;
                baseReturn.Message = "Customer found successfully.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = ex.Message;
            }

            return baseReturn;
        }

        public async Task<BaseReturn<List<Customer>>> ListAllCustomers()
        {
            BaseReturn<List<Customer>> baseReturn = new BaseReturn<List<Customer>>();

            try
            {
                var tabelaCustomers = await _customerRepository.ListAllCustomers();

                if (tabelaCustomers == null || !tabelaCustomers.Any())
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "No customers found.";
                    return baseReturn;
                }

                List<Customer> customers = new List<Customer>();
                foreach (var tabelaCustomer in tabelaCustomers)
                {
                    customers.Add(new Customer()
                    {
                        IdCustomer = tabelaCustomer.IdCustomer,
                        Username = tabelaCustomer.Name,
                        Email = tabelaCustomer.Email,
                        Cellphone = tabelaCustomer.Phone,
                        DriverLicense = tabelaCustomer.DriverLicense,
                        Created = tabelaCustomer.Created,
                    });
                }

                baseReturn.Code = "200";
                baseReturn.Data = customers;
                baseReturn.Message = "Customer list obtained successfully.";
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
