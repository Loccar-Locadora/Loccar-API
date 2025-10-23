using FluentAssertions;
using LoccarApplication;
using LoccarApplication.Interfaces;
using DomainCustomer = LoccarDomain.Customer.Models.Customer;
using LoccarDomain.LoggedUser.Models;
using LoccarInfra.ORM.model;
using LoccarInfra.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace LoccarTests.IntegrationTests
{
    public class CustomerApplicationIntegrationTests : IDisposable
    {
        private readonly DataBaseContext _context;
        private readonly CustomerRepository _customerRepository;
        private readonly CustomerApplication _customerApplication;

        public CustomerApplicationIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<DataBaseContext>()
                .UseInMemoryDatabase($"CustomerTestDb_{Guid.NewGuid()}")
                .Options;

            _context = new DataBaseContext(options);
            _customerRepository = new CustomerRepository(_context);
            _customerApplication = new CustomerApplication(_customerRepository);
        }

        [Fact]
        public async Task RegisterCustomer_WhenValidData_SavesToDatabase()
        {
            // Arrange
            var customer = new DomainCustomer
            {
                Username = "Joao Silva",
                Email = "joao@email.com",
                Cellphone = "11999999999",
                DriverLicense = "12345678901"
            };

            // Act
            var result = await _customerApplication.RegisterCustomer(customer);

            // Assert
            result.Code.Should().Be("200");
            result.Message.Should().Be("Customer registered successfully.");
            result.Data.Should().NotBeNull();
            result.Data.Username.Should().Be("Joao Silva");

            var customerInDb = await _context.Customers.FirstOrDefaultAsync(c => c.Email == "joao@email.com");
            customerInDb.Should().NotBeNull();
            customerInDb.Name.Should().Be("Joao Silva");
        }

        [Fact]
        public async Task GetCustomerById_WhenCustomerExists_ReturnsFromDatabase()
        {
            // Arrange
            var customer = new Customer
            {
                Name = "Maria Santos",
                Email = "maria@email.com",
                Phone = "11888888888",
                DriverLicense = "98765432109",
                Created = DateTime.Now
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _customerApplication.GetCustomerById(customer.Idcustomer);

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().NotBeNull();
            result.Data.Username.Should().Be("Maria Santos");
            result.Data.Email.Should().Be("maria@email.com");
        }

        [Fact]
        public async Task UpdateCustomer_WhenValidData_UpdatesInDatabase()
        {
            // Arrange
            var customer = new Customer
            {
                Name = "Pedro Oliveira",
                Email = "pedro@email.com",
                Phone = "11777777777",
                DriverLicense = "11111111111",
                Created = DateTime.Now
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var updatedCustomer = new DomainCustomer
            {
                IdCustomer = customer.Idcustomer,
                Username = "Pedro Oliveira Updated",
                Email = "pedro.updated@email.com",
                Cellphone = "11666666666",
                DriverLicense = "22222222222"
            };

            // Act
            var result = await _customerApplication.UpdateCustomer(updatedCustomer);

            // Assert
            result.Code.Should().Be("200");
            result.Message.Should().Be("Customer updated successfully.");

            var customerInDb = await _context.Customers.FindAsync(customer.Idcustomer);
            customerInDb.Should().NotBeNull();
            customerInDb.Name.Should().Be("Pedro Oliveira Updated");
            customerInDb.Email.Should().Be("pedro.updated@email.com");
        }

        [Fact]
        public async Task DeleteCustomer_WhenCustomerExists_RemovesFromDatabase()
        {
            // Arrange
            var customer = new Customer
            {
                Name = "Ana Costa",
                Email = "ana@email.com",
                Phone = "11555555555",
                DriverLicense = "33333333333",
                Created = DateTime.Now
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _customerApplication.DeleteCustomer(customer.Idcustomer);

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().BeTrue();
            result.Message.Should().Be("Customer deleted successfully.");

            var customerInDb = await _context.Customers.FindAsync(customer.Idcustomer);
            customerInDb.Should().BeNull();
        }

        [Fact]
        public async Task ListAllCustomers_WhenCustomersExist_ReturnsFromDatabase()
        {
            // Arrange
            var customers = new[]
            {
                new Customer
                {
                    Name = "Cliente 1",
                    Email = "cliente1@email.com",
                    Phone = "11111111111",
                    DriverLicense = "44444444444",
                    Created = DateTime.Now
                },
                new Customer
                {
                    Name = "Cliente 2",
                    Email = "cliente2@email.com",
                    Phone = "11222222222",
                    DriverLicense = "55555555555",
                    Created = DateTime.Now
                }
            };

            _context.Customers.AddRange(customers);
            await _context.SaveChangesAsync();

            // Act
            var result = await _customerApplication.ListAllCustomers();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().HaveCount(2);
            result.Data.Should().Contain(c => c.Username == "Cliente 1");
            result.Data.Should().Contain(c => c.Username == "Cliente 2");
        }

        [Theory]
        [InlineData("test@email.com", true)]
        [InlineData("invalid-email", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void ValidateEmailFormat_WithDifferentInputs_ReturnsExpectedResult(string email, bool expected)
        {
            // Act
            bool isValid = !string.IsNullOrWhiteSpace(email) && 
                          email.Contains("@") && 
                          email.Contains(".") &&
                          email.IndexOf("@") > 0 &&
                          email.LastIndexOf(".") > email.IndexOf("@");

            // Assert
            isValid.Should().Be(expected);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}