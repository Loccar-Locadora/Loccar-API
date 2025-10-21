using LoccarDomain.Customer.Models;
using Microsoft.AspNetCore.Mvc;
using LoccarDomain;
using LoccarApplication.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LoccarWebapi.Controllers
{
    [Route("api/Customer")]
    [ApiController]
    public class LesseeController : ControllerBase
    {
        readonly ICustomerApplication _customerApplication;
        public LesseeController(ICustomerApplication CustomerApplication)
        {
            _customerApplication = CustomerApplication;
        }

        [HttpPost("register")]
        public async Task<BaseReturn<Customer>> RegisterCustomer(Customer customer)
        {
            return await _customerApplication.RegisterCustomer(customer);
        }

        [HttpPut("update")]
        public async Task<BaseReturn<Customer>> UpdateCustomer(Customer customer)
        {
            return await _customerApplication.UpdateCustomer(customer);
        }

        [HttpDelete("delete/{id}")]
        public async Task<BaseReturn<bool>> DeleteCustomer(int id)
        {
            return await _customerApplication.DeleteCustomer(id);
        }

        [HttpGet("{id}")]
        public async Task<BaseReturn<Customer>> GetCustomerById(int id)
        {
            return await _customerApplication.GetCustomerById(id);
        }

        [HttpGet("list/all")]
        public async Task<BaseReturn<List<Customer>>> ListAllCustomers()
        {
            return await _customerApplication.ListAllCustomers();
        }
    } 
}
