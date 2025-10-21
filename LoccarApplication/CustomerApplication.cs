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
        readonly ICustomerRepository _CustomerRepository;
        public CustomerApplication(ICustomerRepository CustomerRepository)
        {
            _CustomerRepository = CustomerRepository;
        }
        public async Task<BaseReturn<Customer>> RegisterCustomer(Customer Customer)
        {
            BaseReturn<Customer> baseReturn = new BaseReturn<Customer>();

            try
            {
                LoccarInfra.ORM.model.Customer tabelaCustomer = new LoccarInfra.ORM.model.Customer()
                {
                    Name = Customer.Username,
                    Email = Customer.Email,
                    Phone = Customer.Cellphone,
                    DriverLicense = Customer.DriverLicense,
                    Created = DateTime.Now
                };

                var response = await _CustomerRepository.RegisterCustomer(tabelaCustomer);

                Customer customerResponse = new Customer()
                {
                    Username = response.Name,
                    Email = response.Email,
                    Cellphone = response.Phone,
                    DriverLicense = response.DriverLicense,
                    Created = response.Created
                };

                baseReturn.Code = "200";
                baseReturn.Data = customerResponse;
                baseReturn.Message = "Locatário cadastrado com sucesso.";
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
                    Idcustomer = (int)customer.IdCustomer,
                    Name = customer.Username,
                    Email = customer.Email,
                    Phone = customer.Cellphone,
                    DriverLicense = customer.DriverLicense
                };

                var response = await _CustomerRepository.UpdateCustomer(tabelaCustomer);

                if (response == null)
                {
                    baseReturn.Code = "404";
                    baseReturn.Message = "Locatário não encontrado.";
                    baseReturn.Data = null;
                    return baseReturn;
                }

                Customer customerResponse = new Customer()
                {
                    Username = response.Name,
                    Email = response.Email,
                    Cellphone = response.Phone,
                    DriverLicense = response.DriverLicense,
                    Created = response.Created
                };

                baseReturn.Code = "200";
                baseReturn.Data = customerResponse;
                baseReturn.Message = "Locatário atualizado com sucesso.";
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
                bool success = await _CustomerRepository.DeleteCustomer(customerId);

                baseReturn.Code = success ? "200" : "404";
                baseReturn.Data = success;
                baseReturn.Message = success ? "Locatário excluído com sucesso." : "Locatário não encontrado.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Data = false;
                baseReturn.Message = ex.Message;
            }

            return baseReturn;
        }

    }
}
