using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LoccarApplication;
using LoccarApplication.Interfaces;
using LoccarDomain.LoggedUser.Models;
using LoccarDomain.Statistics.Models;
using LoccarInfra.Repositories.Interfaces;
using Moq;
using Xunit;

namespace LoccarTests.UnitTests
{
    public class StatisticsApplicationTests
    {
        private readonly Mock<IAuthApplication> _mockAuthApplication;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<IVehicleRepository> _mockVehicleRepository;
        private readonly Mock<IReservationRepository> _mockReservationRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly StatisticsApplication _statisticsApplication;

        public StatisticsApplicationTests()
        {
            _mockAuthApplication = new Mock<IAuthApplication>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockVehicleRepository = new Mock<IVehicleRepository>();
            _mockReservationRepository = new Mock<IReservationRepository>();
            _mockUserRepository = new Mock<IUserRepository>();

            _statisticsApplication = new StatisticsApplication(
                _mockAuthApplication.Object,
                _mockCustomerRepository.Object,
                _mockVehicleRepository.Object,
                _mockReservationRepository.Object,
                _mockUserRepository.Object);
        }

        [Theory]
        [InlineData("CLIENT_ADMIN")]
        [InlineData("client_admin")]
        [InlineData("Client_Admin")]
        [InlineData("CLIENT_EMPLOYEE")]
        [InlineData("client_employee")]
        [InlineData("Client_Employee")]
        public async Task GetTotalCustomersCountWhenAuthorizedUserReturnsCount(string role)
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = new List<string> { role },
                Authenticated = true
            };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockCustomerRepository.Setup(x => x.GetTotalCustomersCount()).ReturnsAsync(25);

            // Act
            var result = await _statisticsApplication.GetTotalCustomersCount();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().Be(25);
            result.Message.Should().Be("Total customers count retrieved successfully.");
        }

        [Theory]
        [InlineData("CLIENT_USER")]
        [InlineData("GUEST")]
        [InlineData("")]
        public async Task GetTotalCustomersCountWhenUnauthorizedUserReturnsUnauthorized(string role)
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = new List<string> { role },
                Authenticated = true
            };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Act
            var result = await _statisticsApplication.GetTotalCustomersCount();

            // Assert
            result.Code.Should().Be("401");
            result.Message.Should().Be("User not authorized.");
        }

        [Fact]
        public async Task GetTotalCustomersCountWhenNullUserReturnsUnauthorized()
        {
            // Arrange
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns((LoggedUser)null);

            // Act
            var result = await _statisticsApplication.GetTotalCustomersCount();

            // Assert
            result.Code.Should().Be("401");
            result.Message.Should().Be("User not authorized.");
        }

        [Fact]
        public async Task GetTotalCustomersCountWhenNullRolesReturnsUnauthorized()
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = null,
                Authenticated = true
            };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Act
            var result = await _statisticsApplication.GetTotalCustomersCount();

            // Assert
            result.Code.Should().Be("401");
            result.Message.Should().Be("User not authorized.");
        }

        [Fact]
        public async Task GetTotalCustomersCountWhenEmptyRolesReturnsUnauthorized()
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = new List<string>(),
                Authenticated = true
            };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Act
            var result = await _statisticsApplication.GetTotalCustomersCount();

            // Assert
            result.Code.Should().Be("401");
            result.Message.Should().Be("User not authorized.");
        }

        [Fact]
        public async Task GetTotalVehiclesCountWhenEmployeeUserReturnsCount()
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = new List<string> { "CLIENT_EMPLOYEE" },
                Authenticated = true
            };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.GetTotalVehiclesCount()).ReturnsAsync(50);

            // Act
            var result = await _statisticsApplication.GetTotalVehiclesCount();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().Be(50);
            result.Message.Should().Be("Total vehicles count retrieved successfully.");
        }

        [Fact]
        public async Task GetActiveReservationsCountWhenAdminUserReturnsCount()
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockReservationRepository.Setup(x => x.GetActiveReservationsCount()).ReturnsAsync(12);

            // Act
            var result = await _statisticsApplication.GetActiveReservationsCount();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().Be(12);
            result.Message.Should().Be("Active reservations count retrieved successfully.");
        }

        [Theory]
        [InlineData("CLIENT_USER")]
        [InlineData("GUEST")]
        public async Task GetAvailableVehiclesCountWhenAuthenticatedUserReturnsCount(string role)
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = new List<string> { role },
                Authenticated = true
            };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.GetAvailableVehiclesCount()).ReturnsAsync(35);

            // Act
            var result = await _statisticsApplication.GetAvailableVehiclesCount();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().Be(35);
            result.Message.Should().Be("Available vehicles count retrieved successfully.");
        }

        [Fact]
        public async Task GetAvailableVehiclesCountWhenNotAuthenticatedReturnsUnauthorized()
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = new List<string> { "CLIENT_USER" },
                Authenticated = false // Not authenticated
            };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Act
            var result = await _statisticsApplication.GetAvailableVehiclesCount();

            // Assert
            result.Code.Should().Be("401");
            result.Message.Should().Be("User not authorized.");
        }

        [Fact]
        public async Task GetSystemStatisticsWhenAuthorizedUserReturnsCompleteStatistics()
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockCustomerRepository.Setup(x => x.GetTotalCustomersCount()).ReturnsAsync(25);
            _mockVehicleRepository.Setup(x => x.GetTotalVehiclesCount()).ReturnsAsync(50);
            _mockReservationRepository.Setup(x => x.GetActiveReservationsCount()).ReturnsAsync(12);
            _mockVehicleRepository.Setup(x => x.GetAvailableVehiclesCount()).ReturnsAsync(35);

            // Act
            var result = await _statisticsApplication.GetSystemStatistics();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().NotBeNull();
            result.Data.TotalCustomers.Should().Be(25);
            result.Data.TotalVehicles.Should().Be(50);
            result.Data.ActiveReservations.Should().Be(12);
            result.Data.AvailableVehicles.Should().Be(35);
            result.Data.GeneratedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
            result.Message.Should().Be("System statistics retrieved successfully.");
        }

        [Fact]
        public async Task GetSystemStatisticsWhenExceptionOccursReturnsError()
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockCustomerRepository.Setup(x => x.GetTotalCustomersCount()).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _statisticsApplication.GetSystemStatistics();

            // Assert
            result.Code.Should().Be("500");
            result.Message.Should().StartWith("An unexpected error occurred:");
        }

        [Fact]
        public async Task GetSystemStatisticsWithMultipleRolesIncludingAdminReturnsSuccess()
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = new List<string> { "CLIENT_USER", "CLIENT_ADMIN", "GUEST" },
                Authenticated = true
            };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockCustomerRepository.Setup(x => x.GetTotalCustomersCount()).ReturnsAsync(10);
            _mockVehicleRepository.Setup(x => x.GetTotalVehiclesCount()).ReturnsAsync(20);
            _mockReservationRepository.Setup(x => x.GetActiveReservationsCount()).ReturnsAsync(5);
            _mockVehicleRepository.Setup(x => x.GetAvailableVehiclesCount()).ReturnsAsync(15);

            // Act
            var result = await _statisticsApplication.GetSystemStatistics();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().NotBeNull();
        }
    }
}
