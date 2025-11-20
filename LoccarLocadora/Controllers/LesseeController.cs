using LoccarApplication.Interfaces;
using LoccarDomain;
using LoccarDomain.Customer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoccarWebapi.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class LesseeController : ControllerBase
    {
        private readonly ICustomerApplication _customerApplication;

        public LesseeController(ICustomerApplication customerApplication)
        {
            _customerApplication = customerApplication;
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

        [HttpGet("list/all")]
        public async Task<BaseReturn<List<Customer>>> ListAllCustomers()
        {
            return await _customerApplication.ListAllCustomers();
        }
    }
}
