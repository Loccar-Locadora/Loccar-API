using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using LoccarApplication;
using LoccarDomain;
using LoccarDomain.Customer.Models;
using LoccarInfra.Repositories.Interfaces;
using Moq;
using Xunit;

namespace LoccarTests.UnitTests
{
    public class CustomerApplicationTests
    {
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly CustomerApplication _customerApplication;
        private readonly Fixture _fixture;

        public CustomerApplicationTests()
        {
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _customerApplication = new CustomerApplication(_mockCustomerRepository.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task RegisterCustomerWhenValidCustomerReturnsSuccess()
        {
            // Arrange
            var customer = new Customer
            {
                Username = "Joao Silva",
                Email = "joao@email.com",
                Cellphone = "11999999999",
                DriverLicense = "12345678901",
            };
            var tbCustomer = new LoccarInfra.ORM.model.Customer
            {
                Name = customer.Username,
                Email = customer.Email,
                Phone = customer.Cellphone,
                DriverLicense = customer.DriverLicense,
                Created = DateTime.Now,
            };

            _mockCustomerRepository.Setup(x => x.RegisterCustomer(It.IsAny<LoccarInfra.ORM.model.Customer>()))
                .ReturnsAsync(tbCustomer);

            // Act
            var result = await _customerApplication.RegisterCustomer(customer);

            // Assert
            result.Code.Should().Be("200");
            result.Message.Should().Be("Customer registered successfully.");
            result.Data.Should().NotBeNull();
            result.Data.Username.Should().Be(customer.Username);
        }

        [Fact]
        public async Task RegisterCustomerWhenExceptionOccursReturnsServerError()
        {
            // Arrange
            var customer = new Customer
            {
                Username = "Test User",
                Email = "test@email.com",
                Cellphone = "11999999999",
                DriverLicense = "12345678901",
            };
            _mockCustomerRepository.Setup(x => x.RegisterCustomer(It.IsAny<LoccarInfra.ORM.model.Customer>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _customerApplication.RegisterCustomer(customer);

            // Assert
            result.Code.Should().Be("500");
            result.Message.Should().Be("Database error");
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task UpdateCustomerWhenCustomerExistsUpdatesSuccessfully()
        {
            // Arrange
            var customer = new Customer
            {
                IdCustomer = 1,
                Username = "Test User",
                Email = "test@email.com",
                Cellphone = "11999999999",
                DriverLicense = "12345678901",
            };
            var tbCustomer = new LoccarInfra.ORM.model.Customer
            {
                IdCustomer = 1,
                Name = customer.Username,
                Email = customer.Email,
                Phone = customer.Cellphone,
                DriverLicense = customer.DriverLicense,
                Created = DateTime.Now,
            };

            _mockCustomerRepository.Setup(x => x.UpdateCustomer(It.IsAny<LoccarInfra.ORM.model.Customer>()))
                .ReturnsAsync(tbCustomer);

            // Act
            var result = await _customerApplication.UpdateCustomer(customer);

            // Assert
            result.Code.Should().Be("200");
            result.Message.Should().Be("Customer updated successfully.");
            result.Data.Should().NotBeNull();
            result.Data.Username.Should().Be(customer.Username);
        }

        [Fact]
        public async Task UpdateCustomerWhenCustomerNotFoundReturnsNotFound()
        {
            // Arrange
            var customer = new Customer
            {
                IdCustomer = 1,
                Username = "Test User",
                Email = "test@email.com",
                Cellphone = "11999999999",
                DriverLicense = "12345678901",
            };
            _mockCustomerRepository.Setup(x => x.UpdateCustomer(It.IsAny<LoccarInfra.ORM.model.Customer>()))
                .ReturnsAsync((LoccarInfra.ORM.model.Customer)null);

            // Act
            var result = await _customerApplication.UpdateCustomer(customer);

            // Assert
            result.Code.Should().Be("404");
            result.Message.Should().Be("Customer not found.");
            result.Data.Should().BeNull();
        }

        [Theory]
        [InlineData(1, true, "200", "Customer deleted successfully.")]
        [InlineData(2, false, "404", "Customer not found.")]
        public async Task DeleteCustomerWithDifferentScenariosReturnsExpectedResult(
            int customerId, bool deleteSuccess, string expectedCode, string expectedMessage)
        {
            // Arrange
            _mockCustomerRepository.Setup(x => x.DeleteCustomer(customerId))
                .ReturnsAsync(deleteSuccess);

            // Act
            var result = await _customerApplication.DeleteCustomer(customerId);

            // Assert
            result.Code.Should().Be(expectedCode);
            result.Message.Should().Be(expectedMessage);
            result.Data.Should().Be(deleteSuccess);
        }

        [Fact]
        public async Task DeleteCustomerWhenExceptionOccursReturnsServerError()
        {
            // Arrange
            _mockCustomerRepository.Setup(x => x.DeleteCustomer(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _customerApplication.DeleteCustomer(1);

            // Assert
            result.Code.Should().Be("500");
            result.Message.Should().Be("Database error");
            result.Data.Should().BeFalse();
        }

        [Fact]
        public async Task GetCustomerByIdWhenCustomerExistsReturnsCustomer()
        {
            // Arrange
            var tbCustomer = new LoccarInfra.ORM.model.Customer
            {
                IdCustomer = 1,
                Name = "Joao Silva",
                Email = "joao@email.com",
                Phone = "11999999999",
                DriverLicense = "123456789",
                Created = DateTime.Now,
            };

            _mockCustomerRepository.Setup(x => x.GetCustomerById(1))
                .ReturnsAsync(tbCustomer);

            // Act
            var result = await _customerApplication.GetCustomerById(1);

            // Assert
            result.Code.Should().Be("200");
            result.Message.Should().Be("Customer found successfully.");
            result.Data.Should().NotBeNull();
            result.Data.Username.Should().Be("Joao Silva");
            result.Data.Email.Should().Be("joao@email.com");
        }

        [Fact]
        public async Task GetCustomerByIdWhenCustomerNotFoundReturnsNotFound()
        {
            // Arrange
            _mockCustomerRepository.Setup(x => x.GetCustomerById(1))
                .ReturnsAsync((LoccarInfra.ORM.model.Customer)null);

            // Act
            var result = await _customerApplication.GetCustomerById(1);

            // Assert
            result.Code.Should().Be("404");
            result.Message.Should().Be("Customer not found.");
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task ListAllCustomersWhenCustomersExistReturnsCustomers()
        {
            // Arrange
            var tbCustomers = new List<LoccarInfra.ORM.model.Customer>
            {
                new LoccarInfra.ORM.model.Customer
                {
                    IdCustomer = 1,
                    Name = "Joao Silva",
                    Email = "joao@email.com",
                    Phone = "11999999999",
                    DriverLicense = "123456789",
                    Created = DateTime.Now,
                },
                new LoccarInfra.ORM.model.Customer
                {
                    IdCustomer = 2,
                    Name = "Maria Santos",
                    Email = "maria@email.com",
                    Phone = "11888888888",
                    DriverLicense = "987654321",
                    Created = DateTime.Now,
                },
            };

            _mockCustomerRepository.Setup(x => x.ListAllCustomers())
                .ReturnsAsync(tbCustomers);

            // Act
            var result = await _customerApplication.ListAllCustomers();

            // Assert
            result.Code.Should().Be("200");
            result.Message.Should().Be("Customer list obtained successfully.");
            result.Data.Should().HaveCount(2);
            result.Data.First().Username.Should().Be("Joao Silva");
        }

        [Fact]
        public async Task ListAllCustomersWhenNoCustomersFoundReturnsNotFound()
        {
            // Arrange
            _mockCustomerRepository.Setup(x => x.ListAllCustomers())
                .ReturnsAsync(new List<LoccarInfra.ORM.model.Customer>());

            // Act
            var result = await _customerApplication.ListAllCustomers();

            // Assert
            result.Code.Should().Be("404");
            result.Message.Should().Be("No customers found.");
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task ListAllCustomersWhenExceptionOccursReturnsServerError()
        {
            // Arrange
            string errorMessage = "Database connection failed";
            _mockCustomerRepository.Setup(x => x.ListAllCustomers())
                .ThrowsAsync(new Exception(errorMessage));

            // Act
            var result = await _customerApplication.ListAllCustomers();

            // Assert
            result.Code.Should().Be("500");
            result.Message.Should().Be(errorMessage);
            result.Data.Should().BeNull();
        }
    }
}
