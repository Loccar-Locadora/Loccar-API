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
    public class StatisticsApplicationUserTests
    {
        private readonly Mock<IAuthApplication> _mockAuthApplication;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<IVehicleRepository> _mockVehicleRepository;
        private readonly Mock<IReservationRepository> _mockReservationRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly StatisticsApplication _statisticsApplication;

        public StatisticsApplicationUserTests()
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

        [Fact]
        public async Task GetUserStatisticsWhenAdminUserReturnsStatistics()
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };

            var statisticsData = new UserStatisticsData
            {
                TotalUsers = 100,
                ActiveUsers = 85,
                RoleCounts = new Dictionary<string, int>
                {
                    { "CLIENT_ADMIN", 5 },
                    { "CLIENT_EMPLOYEE", 15 },
                    { "CLIENT_USER", 80 }
                }
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockUserRepository.Setup(x => x.GetUserStatisticsData()).ReturnsAsync(statisticsData);

            // Act
            var result = await _statisticsApplication.GetUserStatistics();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().NotBeNull();
            result.Data.TotalUsers.Should().Be(100);
            result.Data.ActiveUsers.Should().Be(85);
            result.Data.InactiveUsers.Should().Be(15);
            result.Data.AdminUsers.Should().Be(5);
            result.Data.EmployeeUsers.Should().Be(15);
            result.Data.CommonUsers.Should().Be(80);
            result.Message.Should().Be("User statistics retrieved successfully.");
        }

        [Theory]
        [InlineData("CLIENT_EMPLOYEE")]
        [InlineData("CLIENT_USER")]
        [InlineData("GUEST")]
        public async Task GetUserStatisticsWhenNonAdminUserReturnsUnauthorized(string role)
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { role },
                Authenticated = true
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Act
            var result = await _statisticsApplication.GetUserStatistics();

            // Assert
            result.Code.Should().Be("401");
            result.Message.Should().Be("User not authorized. Only administrators can access user statistics.");
        }

        [Fact]
        public async Task GetDetailedUserStatisticsWhenAdminUserReturnsDetailedStatistics()
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };

            var statisticsData = new UserStatisticsData
            {
                TotalUsers = 100,
                ActiveUsers = 90,
                RoleCounts = new Dictionary<string, int>
                {
                    { "CLIENT_ADMIN", 5 },
                    { "CLIENT_EMPLOYEE", 15 },
                    { "CLIENT_USER", 75 },
                    { "GUEST", 5 }
                }
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockUserRepository.Setup(x => x.GetUserStatisticsData()).ReturnsAsync(statisticsData);

            // Act
            var result = await _statisticsApplication.GetDetailedUserStatistics();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().NotBeNull();
            result.Data.TotalUsers.Should().Be(100);
            result.Data.ActiveUsers.Should().Be(90);
            result.Data.InactiveUsers.Should().Be(10);
            result.Data.RoleBreakdown.Should().HaveCount(4);

            var adminRole = result.Data.RoleBreakdown.Find(r => r.RoleName == "CLIENT_ADMIN");
            adminRole.Should().NotBeNull();
            adminRole.UserCount.Should().Be(5);
            adminRole.Percentage.Should().Be(5.0m);

            var employeeRole = result.Data.RoleBreakdown.Find(r => r.RoleName == "CLIENT_EMPLOYEE");
            employeeRole.Should().NotBeNull();
            employeeRole.UserCount.Should().Be(15);
            employeeRole.Percentage.Should().Be(15.0m);
        }

        [Fact]
        public async Task GetUsersByRoleCountWhenAdminUserReturnsCount()
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockUserRepository.Setup(x => x.GetUsersByRoleCount("CLIENT_ADMIN")).ReturnsAsync(5);

            // Act
            var result = await _statisticsApplication.GetUsersByRoleCount("CLIENT_ADMIN");

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().Be(5);
            result.Message.Should().Be("Users with role 'CLIENT_ADMIN' count retrieved successfully.");
        }

        [Fact]
        public async Task GetUsersByRoleCountWhenNonAdminUserReturnsUnauthorized()
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { "CLIENT_EMPLOYEE" },
                Authenticated = true
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Act
            var result = await _statisticsApplication.GetUsersByRoleCount("CLIENT_ADMIN");

            // Assert
            result.Code.Should().Be("401");
            result.Message.Should().Be("User not authorized. Only administrators can access user role statistics.");
        }

        [Fact]
        public async Task GetUsersByRoleCountWhenEmptyRoleNameReturnsBadRequest()
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Act
            var result = await _statisticsApplication.GetUsersByRoleCount(string.Empty);

            // Assert
            result.Code.Should().Be("400");
            result.Message.Should().Be("Role name is required.");
        }

        [Fact]
        public async Task GetUsersByRoleCountWhenNullRoleNameReturnsBadRequest()
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Act
            var result = await _statisticsApplication.GetUsersByRoleCount(null);

            // Assert
            result.Code.Should().Be("400");
            result.Message.Should().Be("Role name is required.");
        }

        [Theory]
        [InlineData("client_admin")]
        [InlineData("CLIENT_ADMIN")]
        [InlineData("Client_Admin")]
        public async Task GetUsersByRoleCountConvertsRoleNameToUppercase(string roleName)
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockUserRepository.Setup(x => x.GetUsersByRoleCount("CLIENT_ADMIN")).ReturnsAsync(5);

            // Act
            var result = await _statisticsApplication.GetUsersByRoleCount(roleName);

            // Assert
            result.Code.Should().Be("200");
            _mockUserRepository.Verify(x => x.GetUsersByRoleCount("CLIENT_ADMIN"), Times.Once);
        }

        [Fact]
        public async Task GetUserStatisticsWhenExceptionOccursReturnsError()
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockUserRepository.Setup(x => x.GetUserStatisticsData()).ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _statisticsApplication.GetUserStatistics();

            // Assert
            result.Code.Should().Be("500");
            result.Message.Should().StartWith("An unexpected error occurred:");
        }
    }
}
