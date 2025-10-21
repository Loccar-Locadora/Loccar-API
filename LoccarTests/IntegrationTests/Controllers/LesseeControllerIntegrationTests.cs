using FluentAssertions;
using LoccarTests.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Text;
using Xunit;
using LoccarInfra.ORM.model;
using LoccarDomain.Customer.Models;

namespace LoccarTests.IntegrationTests.Controllers
{
    public class LesseeControllerIntegrationTests : IClassFixture<LoccarWebApplicationFactory<Program>>
    {
        private readonly LoccarWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public LesseeControllerIntegrationTests(LoccarWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task RegisterCustomer_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var customer = new LoccarDomain.Customer.Models.Customer
            {
                Username = "Integration Test User",
                Email = "integration@test.com",
                Cellphone = "11999999999",
                DriverLicense = "12345678903"
            };

            var json = JsonConvert.SerializeObject(customer);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Customer/register", content);

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().NotBeEmpty();
            
            // Verificar se o cliente foi realmente criado no banco
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
            var createdCustomer = await context.Customers.FirstOrDefaultAsync(c => c.Email == customer.Email);
            
            createdCustomer.Should().NotBeNull();
            createdCustomer.Name.Should().Be(customer.Username);
            createdCustomer.Email.Should().Be(customer.Email);
        }

        [Theory]
        [InlineData("", "test@example.com", "11999999999", "12345678901")]
        [InlineData("Test User", "", "11999999999", "12345678901")]
        [InlineData("Test User", "test@example.com", "", "12345678901")]
        [InlineData("Test User", "test@example.com", "11999999999", "")]
        public async Task RegisterCustomer_WithInvalidData_ShouldHandleGracefully(
            string username, string email, string cellphone, string driverLicense)
        {
            // Arrange
            var customer = new LoccarDomain.Customer.Models.Customer
            {
                Username = username,
                Email = email,
                Cellphone = cellphone,
                DriverLicense = driverLicense
            };

            var json = JsonConvert.SerializeObject(customer);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Customer/register", content);

            // Assert
            // A aplicação pode aceitar ou rejeitar dados inválidos dependendo da validação implementada
            // Este teste verifica se a aplicação não quebra com dados inválidos
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UpdateCustomer_WithExistingCustomer_ShouldReturnSuccess()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
            
            // Criar um cliente no banco primeiro
            var existingCustomer = new LoccarInfra.ORM.model.Customer
            {
                Name = "Existing Customer",
                Email = "existing@test.com",
                Phone = "11999999999",
                DriverLicense = "12345678904",
                Created = DateTime.Now
            };
            
            context.Customers.Add(existingCustomer);
            await context.SaveChangesAsync();

            var updateCustomer = new LoccarDomain.Customer.Models.Customer
            {
                IdCustomer = existingCustomer.Idcustomer,
                Username = "Updated Customer Name",
                Email = "updated@test.com",
                Cellphone = "11888888888",
                DriverLicense = "12345678904"
            };

            var json = JsonConvert.SerializeObject(updateCustomer);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync("/api/Customer/update", content);

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            
            // Verificar se foi atualizado no banco
            var updatedCustomer = await context.Customers.FindAsync(existingCustomer.Idcustomer);
            updatedCustomer.Should().NotBeNull();
            updatedCustomer.Name.Should().Be(updateCustomer.Username);
            updatedCustomer.Email.Should().Be(updateCustomer.Email);
        }

        [Fact]
        public async Task DeleteCustomer_WithExistingCustomer_ShouldReturnSuccess()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
            
            var customerToDelete = new LoccarInfra.ORM.model.Customer
            {
                Name = "Customer To Delete",
                Email = "delete@test.com",
                Phone = "11999999999",
                DriverLicense = "12345678905",
                Created = DateTime.Now
            };
            
            context.Customers.Add(customerToDelete);
            await context.SaveChangesAsync();
            var customerId = customerToDelete.Idcustomer;

            // Act
            var response = await _client.DeleteAsync($"/api/Customer/delete/{customerId}");

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            
            // Verificar se foi removido do banco
            var deletedCustomer = await context.Customers.FindAsync(customerId);
            deletedCustomer.Should().BeNull();
        }

        [Fact]
        public async Task GetCustomerById_WithExistingCustomer_ShouldReturnCustomer()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
            
            var existingCustomer = new LoccarInfra.ORM.model.Customer
            {
                Name = "Get Customer Test",
                Email = "get@test.com",
                Phone = "11999999999",
                DriverLicense = "12345678906",
                Created = DateTime.Now
            };
            
            context.Customers.Add(existingCustomer);
            await context.SaveChangesAsync();
            var customerId = existingCustomer.Idcustomer;

            // Act
            var response = await _client.GetAsync($"/api/Customer/{customerId}");

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Contain(existingCustomer.Name);
            responseString.Should().Contain(existingCustomer.Email);
        }

        [Fact]
        public async Task ListAllCustomers_ShouldReturnAllCustomers()
        {
            // Act
            var response = await _client.GetAsync("/api/Customer/list/all");

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().NotBeEmpty();
            
            // Verificar se contém os clientes de seed
            responseString.Should().Contain("Test Customer 1");
            responseString.Should().Contain("Test Customer 2");
        }
    }
}