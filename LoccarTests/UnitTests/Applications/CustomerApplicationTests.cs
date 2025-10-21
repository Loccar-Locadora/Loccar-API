using FluentAssertions;
using LoccarApplication;
using LoccarDomain;
using LoccarDomain.Customer.Models;
using LoccarInfra.Repositories.Interfaces;
using LoccarTests.Common;
using Moq;
using Xunit;

namespace LoccarTests.UnitTests.Applications
{
    public class CustomerApplicationTests : BaseUnitTest
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly CustomerApplication _customerApplication;

        public CustomerApplicationTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _customerApplication = new CustomerApplication(_customerRepositoryMock.Object);
        }

        [Fact]
        public async Task RegisterCustomer_WhenValidCustomer_ShouldReturnSuccess()
        {
            // Arrange
            var customer = _fixture.Create<Customer>();
            var tbCustomer = _fixture.Build<LoccarInfra.ORM.model.Customer>()
                .With(c => c.Name, customer.Username)
                .With(c => c.Email, customer.Email)
                .With(c => c.Phone, customer.Cellphone)
                .With(c => c.DriverLicense, customer.DriverLicense)
                .Create();

            _customerRepositoryMock.Setup(x => x.RegisterCustomer(It.IsAny<LoccarInfra.ORM.model.Customer>()))
                .ReturnsAsync(tbCustomer);

            // Act
            var result = await _customerApplication.RegisterCustomer(customer);

            // Assert
            result.Code.Should().Be("200");
            result.Message.Should().Be("Locatário cadastrado com sucesso.");
            result.Data.Should().NotBeNull();
            result.Data.Username.Should().Be(customer.Username);
            result.Data.Email.Should().Be(customer.Email);
            result.Data.Cellphone.Should().Be(customer.Cellphone);
            result.Data.DriverLicense.Should().Be(customer.DriverLicense);

            _customerRepositoryMock.Verify(x => x.RegisterCustomer(It.IsAny<LoccarInfra.ORM.model.Customer>()), Times.Once);
        }

        [Fact]
        public async Task RegisterCustomer_WhenRepositoryThrowsException_ShouldReturnError()
        {
            // Arrange
            var customer = _fixture.Create<Customer>();
            var exceptionMessage = "Database connection failed";
            _customerRepositoryMock.Setup(x => x.RegisterCustomer(It.IsAny<LoccarInfra.ORM.model.Customer>()))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _customerApplication.RegisterCustomer(customer);

            // Assert
            result.Code.Should().Be("500");
            result.Message.Should().Be(exceptionMessage);
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task UpdateCustomer_WhenCustomerExists_ShouldReturnSuccess()
        {
            // Arrange
            var customer = _fixture.Build<Customer>()
                .With(c => c.IdCustomer, 1)
                .Create();

            var tbCustomer = _fixture.Build<LoccarInfra.ORM.model.Customer>()
                .With(c => c.Idcustomer, 1)
                .With(c => c.Name, customer.Username)
                .With(c => c.Email, customer.Email)
                .With(c => c.Phone, customer.Cellphone)
                .With(c => c.DriverLicense, customer.DriverLicense)
                .Create();

            _customerRepositoryMock.Setup(x => x.UpdateCustomer(It.IsAny<LoccarInfra.ORM.model.Customer>()))
                .ReturnsAsync(tbCustomer);

            // Act
            var result = await _customerApplication.UpdateCustomer(customer);

            // Assert
            result.Code.Should().Be("200");
            result.Message.Should().Be("Locatário atualizado com sucesso.");
            result.Data.Should().NotBeNull();
            result.Data.Username.Should().Be(customer.Username);
            result.Data.Email.Should().Be(customer.Email);

            _customerRepositoryMock.Verify(x => x.UpdateCustomer(It.Is<LoccarInfra.ORM.model.Customer>(
                c => c.Idcustomer == 1 && c.Name == customer.Username)), Times.Once);
        }

        [Fact]
        public async Task UpdateCustomer_WhenCustomerNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var customer = _fixture.Create<Customer>();
            _customerRepositoryMock.Setup(x => x.UpdateCustomer(It.IsAny<LoccarInfra.ORM.model.Customer>()))
                .ReturnsAsync((LoccarInfra.ORM.model.Customer)null);

            // Act
            var result = await _customerApplication.UpdateCustomer(customer);

            // Assert
            result.Code.Should().Be("404");
            result.Message.Should().Be("Locatário não encontrado.");
            result.Data.Should().BeNull();
        }

        [Theory]
        [InlineData(true, "200", "Locatário excluído com sucesso.")]
        [InlineData(false, "404", "Locatário não encontrado.")]
        public async Task DeleteCustomer_WithDifferentScenarios_ShouldReturnExpectedResult(
            bool repositoryResult, string expectedCode, string expectedMessage)
        {
            // Arrange
            const int customerId = 1;
            _customerRepositoryMock.Setup(x => x.DeleteCustomer(customerId))
                .ReturnsAsync(repositoryResult);

            // Act
            var result = await _customerApplication.DeleteCustomer(customerId);

            // Assert
            result.Code.Should().Be(expectedCode);
            result.Message.Should().Be(expectedMessage);
            result.Data.Should().Be(repositoryResult);

            _customerRepositoryMock.Verify(x => x.DeleteCustomer(customerId), Times.Once);
        }

        [Fact]
        public async Task DeleteCustomer_WhenRepositoryThrowsException_ShouldReturnError()
        {
            // Arrange
            const int customerId = 1;
            var exceptionMessage = "Database connection failed";
            _customerRepositoryMock.Setup(x => x.DeleteCustomer(customerId))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _customerApplication.DeleteCustomer(customerId);

            // Assert
            result.Code.Should().Be("500");
            result.Data.Should().BeFalse();
            result.Message.Should().Be(exceptionMessage);
        }

        [Theory]
        [InlineData("john@email.com", "John Doe", "123456789", "11999999999")]
        [InlineData("mary@email.com", "Mary Jane", "987654321", "11888888888")]
        [InlineData("peter@email.com", "Peter Parker", "555444333", "11777777777")]
        public async Task RegisterCustomer_WithParameterizedData_ShouldReturnSuccess(
            string email, string username, string driverLicense, string cellphone)
        {
            // Arrange
            var customer = new Customer
            {
                Email = email,
                Username = username,
                DriverLicense = driverLicense,
                Cellphone = cellphone
            };

            var tbCustomer = new LoccarInfra.ORM.model.Customer
            {
                Name = customer.Username,
                Email = customer.Email,
                Phone = customer.Cellphone,
                DriverLicense = customer.DriverLicense,
                Created = DateTime.Now
            };

            _customerRepositoryMock.Setup(x => x.RegisterCustomer(It.IsAny<LoccarInfra.ORM.model.Customer>()))
                .ReturnsAsync(tbCustomer);

            // Act
            var result = await _customerApplication.RegisterCustomer(customer);

            // Assert
            result.Code.Should().Be("200");
            result.Data.Username.Should().Be(username);
            result.Data.Email.Should().Be(email);
            result.Data.DriverLicense.Should().Be(driverLicense);
            result.Data.Cellphone.Should().Be(cellphone);
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("")]
        [InlineData(null)]
        public async Task RegisterCustomer_WithInvalidEmail_ShouldStillCallRepository(string invalidEmail)
        {
            // Arrange - O teste simula que a validação de email pode acontecer em outro lugar
            var customer = _fixture.Build<Customer>()
                .With(c => c.Email, invalidEmail)
                .Create();

            var tbCustomer = _fixture.Build<LoccarInfra.ORM.model.Customer>()
                .With(c => c.Email, invalidEmail)
                .Create();

            _customerRepositoryMock.Setup(x => x.RegisterCustomer(It.IsAny<LoccarInfra.ORM.model.Customer>()))
                .ReturnsAsync(tbCustomer);

            // Act
            var result = await _customerApplication.RegisterCustomer(customer);

            // Assert
            result.Code.Should().Be("200"); // A aplicação não está fazendo validação de email internamente
            _customerRepositoryMock.Verify(x => x.RegisterCustomer(It.IsAny<LoccarInfra.ORM.model.Customer>()), Times.Once);
        }
    }
}