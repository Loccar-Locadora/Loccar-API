using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LoccarApplication;
using LoccarApplication.Interfaces;
using LoccarDomain.LoggedUser.Models;
using LoccarDomain.Vehicle.Models;
using LoccarInfra.Repositories.Interfaces;
using Moq;
using Xunit;
using DomainVehicle = LoccarDomain.Vehicle.Models.Vehicle;
using OrmVehicle = LoccarInfra.ORM.model.Vehicle;

namespace LoccarTests.UnitTests
{
    public class VehicleApplicationListWithCountsTests
    {
        private readonly Mock<IAuthApplication> _mockAuthApplication;
        private readonly Mock<IVehicleRepository> _mockVehicleRepository;
        private readonly VehicleApplication _vehicleApplication;

        public VehicleApplicationListWithCountsTests()
        {
            _mockAuthApplication = new Mock<IAuthApplication>();
            _mockVehicleRepository = new Mock<IVehicleRepository>();
            _vehicleApplication = new VehicleApplication(_mockAuthApplication.Object, _mockVehicleRepository.Object);
        }

        [Theory]
        [InlineData("CLIENT_ADMIN")]
        [InlineData("CLIENT_EMPLOYEE")]
        public async Task ListAllVehiclesWithCountsWhenAuthorizedUserReturnsVehiclesWithCounts(string role)
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { role },
                Authenticated = true
            };

            var mockVehicles = new List<OrmVehicle>
            {
                new OrmVehicle { IdVehicle = 1, Brand = "Toyota", Model = "Corolla", Reserved = false },
                new OrmVehicle { IdVehicle = 2, Brand = "Honda", Model = "Civic", Reserved = true },
                new OrmVehicle { IdVehicle = 3, Brand = "Ford", Model = "Focus", Reserved = false }
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.ListAllVehicles()).ReturnsAsync(mockVehicles);
            _mockVehicleRepository.Setup(x => x.GetTotalVehiclesCount()).ReturnsAsync(3);
            _mockVehicleRepository.Setup(x => x.GetAvailableVehiclesCount()).ReturnsAsync(2);

            // Act
            var result = await _vehicleApplication.ListAllVehiclesWithCounts();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().NotBeNull();
            result.Data.Vehicles.Should().HaveCount(3);
            result.Data.TotalVehicles.Should().Be(3);
            result.Data.AvailableVehicles.Should().Be(2);
            result.Data.ReservedVehicles.Should().Be(1); // 3 - 2 = 1
            result.Message.Should().Contain("Total: 3, Available: 2, Reserved: 1");
        }

        [Fact]
        public async Task ListAllVehiclesWithCountsWhenUnauthorizedUserReturnsUnauthorized()
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { "CLIENT_USER" },
                Authenticated = true
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Act
            var result = await _vehicleApplication.ListAllVehiclesWithCounts();

            // Assert
            result.Code.Should().Be("401");
            result.Message.Should().Be("User not authorized");
        }

        [Fact]
        public async Task ListAllVehiclesWithCountsWhenNoVehiclesFoundReturnsEmptyResponse()
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };

            var emptyVehicleList = new List<OrmVehicle>();

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.ListAllVehicles()).ReturnsAsync(emptyVehicleList);
            _mockVehicleRepository.Setup(x => x.GetTotalVehiclesCount()).ReturnsAsync(0);
            _mockVehicleRepository.Setup(x => x.GetAvailableVehiclesCount()).ReturnsAsync(0);

            // Act
            var result = await _vehicleApplication.ListAllVehiclesWithCounts();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().NotBeNull();
            result.Data.Vehicles.Should().BeEmpty();
            result.Data.TotalVehicles.Should().Be(0);
            result.Data.AvailableVehicles.Should().Be(0);
            result.Data.ReservedVehicles.Should().Be(0);
            result.Message.Should().Be("No vehicles found.");
        }

        [Fact]
        public async Task ListAllVehiclesWithCountsWhenExceptionOccursReturnsError()
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.ListAllVehicles()).ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _vehicleApplication.ListAllVehiclesWithCounts();

            // Assert
            result.Code.Should().Be("500");
            result.Message.Should().StartWith("An unexpected error occurred:");
        }

        [Fact]
        public async Task ListAllVehiclesWithCountsWhenAllVehiclesReservedReturnsCorrectCounts()
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };

            var mockVehicles = new List<OrmVehicle>
            {
                new OrmVehicle { IdVehicle = 1, Brand = "Toyota", Model = "Corolla", Reserved = true },
                new OrmVehicle { IdVehicle = 2, Brand = "Honda", Model = "Civic", Reserved = true }
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.ListAllVehicles()).ReturnsAsync(mockVehicles);
            _mockVehicleRepository.Setup(x => x.GetTotalVehiclesCount()).ReturnsAsync(2);
            _mockVehicleRepository.Setup(x => x.GetAvailableVehiclesCount()).ReturnsAsync(0);

            // Act
            var result = await _vehicleApplication.ListAllVehiclesWithCounts();

            // Assert
            result.Code.Should().Be("200");
            result.Data.TotalVehicles.Should().Be(2);
            result.Data.AvailableVehicles.Should().Be(0);
            result.Data.ReservedVehicles.Should().Be(2);
            result.Message.Should().Contain("Total: 2, Available: 0, Reserved: 2");
        }

        [Fact]
        public async Task ListAllVehiclesWithCountsWhenAllVehiclesAvailableReturnsCorrectCounts()
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };

            var mockVehicles = new List<OrmVehicle>
            {
                new OrmVehicle { IdVehicle = 1, Brand = "Toyota", Model = "Corolla", Reserved = false },
                new OrmVehicle { IdVehicle = 2, Brand = "Honda", Model = "Civic", Reserved = false },
                new OrmVehicle { IdVehicle = 3, Brand = "Ford", Model = "Focus", Reserved = false }
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.ListAllVehicles()).ReturnsAsync(mockVehicles);
            _mockVehicleRepository.Setup(x => x.GetTotalVehiclesCount()).ReturnsAsync(3);
            _mockVehicleRepository.Setup(x => x.GetAvailableVehiclesCount()).ReturnsAsync(3);

            // Act
            var result = await _vehicleApplication.ListAllVehiclesWithCounts();

            // Assert
            result.Code.Should().Be("200");
            result.Data.TotalVehicles.Should().Be(3);
            result.Data.AvailableVehicles.Should().Be(3);
            result.Data.ReservedVehicles.Should().Be(0);
            result.Message.Should().Contain("Total: 3, Available: 3, Reserved: 0");
        }

        [Fact]
        public async Task ListAllVehiclesWithCountsGeneratesTimestampCorrectly()
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };

            var mockVehicles = new List<OrmVehicle>
            {
                new OrmVehicle { IdVehicle = 1, Brand = "Toyota", Model = "Corolla", Reserved = false }
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.ListAllVehicles()).ReturnsAsync(mockVehicles);
            _mockVehicleRepository.Setup(x => x.GetTotalVehiclesCount()).ReturnsAsync(1);
            _mockVehicleRepository.Setup(x => x.GetAvailableVehiclesCount()).ReturnsAsync(1);

            var beforeCall = System.DateTime.Now;

            // Act
            var result = await _vehicleApplication.ListAllVehiclesWithCounts();

            var afterCall = System.DateTime.Now;

            // Assert
            result.Code.Should().Be("200");
            result.Data.GeneratedAt.Should().BeAfter(beforeCall.AddSeconds(-1));
            result.Data.GeneratedAt.Should().BeBefore(afterCall.AddSeconds(1));
        }
    }
}
