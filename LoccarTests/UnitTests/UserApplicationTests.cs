using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LoccarApplication;
using LoccarInfra.ORM.model;
using LoccarInfra.Repositories.Interfaces;
using Moq;
using Xunit;
using DomainUser = LoccarDomain.User.Models.User;

namespace LoccarTests.UnitTests
{
    public class UserApplicationTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly UserApplication _userApplication;

        public UserApplicationTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _userApplication = new UserApplication(_mockUserRepository.Object, _mockCustomerRepository.Object);
        }

        [Fact]
        public async Task ListAllUsersWhenUsersExistReturnsUsersWithCellphoneFromCustomer()
        {
            // Arrange
            var mockUsers = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "John Doe",
                    Email = "john@example.com",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    Roles = new List<Role>
                    {
                        new Role { Name = "CLIENT_USER" }
                    }
                },
                new User
                {
                    Id = 2,
                    Username = "Jane Smith",
                    Email = "jane@example.com",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    Roles = new List<Role>
                    {
                        new Role { Name = "CLIENT_ADMIN" }
                    }
                }
            };

            // Mock customers para buscar cellphone
            var johnCustomer = new Customer
            {
                IdCustomer = 1,
                Name = "John Doe",
                Email = "john@example.com",
                Phone = "11999999999"
            };

            var janeCustomer = new Customer
            {
                IdCustomer = 2,
                Name = "Jane Smith", 
                Email = "jane@example.com",
                Phone = "11888888888"
            };

            _mockUserRepository.Setup(x => x.ListAllUsers()).ReturnsAsync(mockUsers);
            _mockCustomerRepository.Setup(x => x.GetRegistrationByEmail("john@example.com")).ReturnsAsync(johnCustomer);
            _mockCustomerRepository.Setup(x => x.GetRegistrationByEmail("jane@example.com")).ReturnsAsync(janeCustomer);

            // Act
            var result = await _userApplication.ListAllUsers();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().HaveCount(2);
            result.Data[0].Name.Should().Be("John Doe");
            result.Data[0].Email.Should().Be("john@example.com");
            result.Data[0].Cellphone.Should().Be("11999999999"); // Obtido do Customer
            result.Data[1].Name.Should().Be("Jane Smith");
            result.Data[1].Email.Should().Be("jane@example.com");
            result.Data[1].Cellphone.Should().Be("11888888888"); // Obtido do Customer
            result.Message.Should().Be("User list obtained successfully.");
        }

        [Fact]
        public async Task ListAllUsersWhenCustomerNotFoundReturnsEmptyCellphone()
        {
            // Arrange
            var mockUsers = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "John Doe",
                    Email = "john@example.com",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    Roles = new List<Role>
                    {
                        new Role { Name = "CLIENT_USER" }
                    }
                }
            };

            _mockUserRepository.Setup(x => x.ListAllUsers()).ReturnsAsync(mockUsers);
            _mockCustomerRepository.Setup(x => x.GetRegistrationByEmail("john@example.com")).ReturnsAsync((Customer)null);

            // Act
            var result = await _userApplication.ListAllUsers();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().HaveCount(1);
            result.Data[0].Cellphone.Should().Be(string.Empty); // Customer não encontrado
        }

        [Fact]
        public async Task ListAllUsersWhenCustomerHasNoPhoneReturnsEmptyCellphone()
        {
            // Arrange
            var mockUsers = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "John Doe",
                    Email = "john@example.com",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    Roles = new List<Role>
                    {
                        new Role { Name = "CLIENT_USER" }
                    }
                }
            };

            var customerWithoutPhone = new Customer
            {
                IdCustomer = 1,
                Name = "John Doe",
                Email = "john@example.com",
                Phone = null // Sem telefone
            };

            _mockUserRepository.Setup(x => x.ListAllUsers()).ReturnsAsync(mockUsers);
            _mockCustomerRepository.Setup(x => x.GetRegistrationByEmail("john@example.com")).ReturnsAsync(customerWithoutPhone);

            // Act
            var result = await _userApplication.ListAllUsers();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().HaveCount(1);
            result.Data[0].Cellphone.Should().Be(string.Empty); // Phone é null no Customer
        }

        [Fact]
        public async Task ListAllUsersWhenNoUsersFoundReturnsNotFound()
        {
            // Arrange
            var emptyUserList = new List<User>();

            _mockUserRepository.Setup(x => x.ListAllUsers()).ReturnsAsync(emptyUserList);

            // Act
            var result = await _userApplication.ListAllUsers();

            // Assert
            result.Code.Should().Be("404");
            result.Message.Should().Be("No users found.");
        }

        [Fact]
        public async Task ListAllUsersWhenExceptionOccursReturnsError()
        {
            // Arrange
            string errorMessage = "Database connection failed";
            _mockUserRepository.Setup(x => x.ListAllUsers()).ThrowsAsync(new System.Exception(errorMessage));

            // Act
            var result = await _userApplication.ListAllUsers();

            // Assert
            result.Code.Should().Be("500");
            result.Message.Should().Be(errorMessage);
        }

        [Fact]
        public async Task ListAllUsersWhenCustomerRepositoryThrowsExceptionReturnsEmptyCellphone()
        {
            // Arrange
            var mockUsers = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "John Doe",
                    Email = "john@example.com",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    Roles = new List<Role>
                    {
                        new Role { Name = "CLIENT_USER" }
                    }
                }
            };

            _mockUserRepository.Setup(x => x.ListAllUsers()).ReturnsAsync(mockUsers);
            _mockCustomerRepository.Setup(x => x.GetRegistrationByEmail("john@example.com"))
                .ThrowsAsync(new System.Exception("Customer service error"));

            // Act
            var result = await _userApplication.ListAllUsers();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().HaveCount(1);
            result.Data[0].Cellphone.Should().Be(string.Empty); // Erro no Customer service, retorna vazio
        }

        [Fact]
        public async Task ListAllUsersWhenNullUsersReturnedReturnsNotFound()
        {
            // Arrange
            _mockUserRepository.Setup(x => x.ListAllUsers()).ReturnsAsync((List<User>)null);

            // Act
            var result = await _userApplication.ListAllUsers();

            // Assert
            result.Code.Should().Be("404");
            result.Message.Should().Be("No users found.");
        }
    }
}
