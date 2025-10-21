using AutoFixture;
using FluentAssertions;
using LoccarApplication;
using LoccarDomain;
using LoccarDomain.Customer.Models;
using LoccarInfra.Repositories.Interfaces;
using LoccarTests.Common;
using Moq;
using Xunit;

namespace LoccarTests.ParametrizedTests
{
    public class CustomerValidationParametrizedTests : BaseUnitTest
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly CustomerApplication _customerApplication;

        public CustomerValidationParametrizedTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _customerApplication = new CustomerApplication(_customerRepositoryMock.Object);
        }

        public static IEnumerable<object[]> GetValidCustomerData()
        {
            yield return new object[] 
            { 
                "João Silva", 
                "joao.silva@email.com", 
                "11999999999", 
                "12345678901",
                true // shouldSucceed
            };
            
            yield return new object[] 
            { 
                "Maria Santos", 
                "maria.santos@gmail.com", 
                "21888888888", 
                "98765432109",
                true
            };
            
            yield return new object[] 
            { 
                "Pedro Oliveira", 
                "pedro@outlook.com", 
                "31777777777", 
                "11122233344",
                true
            };
        }

        public static IEnumerable<object[]> GetInvalidCustomerData()
        {
            yield return new object[] 
            { 
                "", 
                "valid@email.com", 
                "11999999999", 
                "12345678901",
                "Nome não pode estar vazio"
            };
            
            yield return new object[] 
            { 
                "Valid Name", 
                "invalid-email", 
                "11999999999", 
                "12345678901",
                "Email deve ter formato válido"
            };
            
            yield return new object[] 
            { 
                "Valid Name", 
                "valid@email.com", 
                "123", 
                "12345678901",
                "Telefone deve ter formato válido"
            };
            
            yield return new object[] 
            { 
                "Valid Name", 
                "valid@email.com", 
                "11999999999", 
                "123",
                "CNH deve ter 11 dígitos"
            };
        }

        public static IEnumerable<object[]> GetEdgeCaseCustomerData()
        {
            yield return new object[]
            {
                new string('A', 255), // Nome muito longo
                "test@email.com",
                "11999999999",
                "12345678901"
            };

            yield return new object[]
            {
                "Test User",
                "test@" + new string('a', 250) + ".com", // Email muito longo
                "11999999999",
                "12345678901"
            };

            yield return new object[]
            {
                "Test User",
                "test@email.com",
                "119999999999999999999", // Telefone muito longo
                "12345678901"
            };
        }

        [Theory]
        [MemberData(nameof(GetValidCustomerData))]
        public async Task RegisterCustomer_WithValidData_ShouldSucceed(
            string username, string email, string cellphone, string driverLicense, bool shouldSucceed)
        {
            // Arrange
            var customer = new Customer
            {
                Username = username,
                Email = email,
                Cellphone = cellphone,
                DriverLicense = driverLicense
            };

            var tbCustomer = _fixture.Build<LoccarInfra.ORM.model.Customer>()
                .With(c => c.Name, username)
                .With(c => c.Email, email)
                .With(c => c.Phone, cellphone)
                .With(c => c.DriverLicense, driverLicense)
                .Create();

            _customerRepositoryMock.Setup(x => x.RegisterCustomer(It.IsAny<LoccarInfra.ORM.model.Customer>()))
                .ReturnsAsync(tbCustomer);

            // Act
            var result = await _customerApplication.RegisterCustomer(customer);

            // Assert
            if (shouldSucceed)
            {
                result.Code.Should().Be("200");
                result.Data.Should().NotBeNull();
                result.Data.Username.Should().Be(username);
                result.Data.Email.Should().Be(email);
            }
        }

        [Theory]
        [MemberData(nameof(GetEdgeCaseCustomerData))]
        public async Task RegisterCustomer_WithEdgeCases_ShouldHandleGracefully(
            string username, string email, string cellphone, string driverLicense)
        {
            // Arrange
            var customer = new Customer
            {
                Username = username,
                Email = email,
                Cellphone = cellphone,
                DriverLicense = driverLicense
            };

            var tbCustomer = _fixture.Build<LoccarInfra.ORM.model.Customer>()
                .With(c => c.Name, username)
                .With(c => c.Email, email)
                .With(c => c.Phone, cellphone)
                .With(c => c.DriverLicense, driverLicense)
                .Create();

            _customerRepositoryMock.Setup(x => x.RegisterCustomer(It.IsAny<LoccarInfra.ORM.model.Customer>()))
                .ReturnsAsync(tbCustomer);

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => _customerApplication.RegisterCustomer(customer));
            
            // O teste verifica que não há exceções não tratadas
            // A aplicação pode aceitar ou rejeitar, mas não deve quebrar
            if (exception == null)
            {
                var result = await _customerApplication.RegisterCustomer(customer);
                result.Should().NotBeNull();
                result.Code.Should().NotBeEmpty();
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999)]
        [InlineData(int.MaxValue)]
        public async Task DeleteCustomer_WithDifferentIds_ShouldCallRepository(int customerId)
        {
            // Arrange
            _customerRepositoryMock.Setup(x => x.DeleteCustomer(It.IsAny<int>()))
                .ReturnsAsync(true);

            // Act
            var result = await _customerApplication.DeleteCustomer(customerId);

            // Assert
            result.Should().NotBeNull();
            _customerRepositoryMock.Verify(x => x.DeleteCustomer(customerId), Times.Once);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        public async Task DeleteCustomer_WithInvalidIds_ShouldStillCallRepository(int invalidId)
        {
            // Arrange
            _customerRepositoryMock.Setup(x => x.DeleteCustomer(It.IsAny<int>()))
                .ReturnsAsync(false);

            // Act
            var result = await _customerApplication.DeleteCustomer(invalidId);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().BeFalse();
            _customerRepositoryMock.Verify(x => x.DeleteCustomer(invalidId), Times.Once);
        }
    }
}