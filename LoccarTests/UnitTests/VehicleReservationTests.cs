using FluentAssertions;
using LoccarApplication;
using LoccarApplication.Interfaces;
using LoccarDomain;
using LoccarDomain.LoggedUser.Models;
using LoccarInfra.Repositories.Interfaces;
using Moq;
using Xunit;

namespace LoccarTests.UnitTests
{
    public class VehicleReservationTests
    {
        private readonly Mock<IAuthApplication> _mockAuthApplication;
        private readonly Mock<IVehicleRepository> _mockVehicleRepository;
        private readonly VehicleApplication _vehicleApplication;

        public VehicleReservationTests()
        {
            _mockAuthApplication = new Mock<IAuthApplication>();
            _mockVehicleRepository = new Mock<IVehicleRepository>();
            _vehicleApplication = new VehicleApplication(_mockAuthApplication.Object, _mockVehicleRepository.Object);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SetVehicleReserved_WithValidUser_ShouldReturnSuccess(bool reservedStatus)
        {
            // Arrange
            var vehicleId = 1;
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.SetVehicleReserved(vehicleId, reservedStatus))
                .ReturnsAsync(true);

            // Act
            var result = await _vehicleApplication.SetVehicleReserved(vehicleId, reservedStatus);

            // Assert
            result.Should().NotBeNull();
            result.Code.Should().Be("200");
            result.Data.Should().BeTrue();
            result.Message.Should().Contain(reservedStatus ? "reserved" : "released");
        }

        [Fact]
        public async Task SetVehicleReserved_WithUnauthorizedUser_ShouldReturnUnauthorized()
        {
            // Arrange
            var vehicleId = 1;
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns((LoggedUser)null);

            // Act
            var result = await _vehicleApplication.SetVehicleReserved(vehicleId, true);

            // Assert
            result.Should().NotBeNull();
            result.Code.Should().Be("401");
            result.Data.Should().BeFalse();
            result.Message.Should().Be("User not authorized.");
        }

        [Fact]
        public async Task SetVehicleReserved_WithUserWithoutRoles_ShouldReturnUnauthorized()
        {
            // Arrange
            var vehicleId = 1;
            var loggedUser = new LoggedUser { Roles = new List<string>() };
            
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Act
            var result = await _vehicleApplication.SetVehicleReserved(vehicleId, true);

            // Assert
            result.Should().NotBeNull();
            result.Code.Should().Be("401");
            result.Data.Should().BeFalse();
            result.Message.Should().Be("User not authorized.");
        }

        [Fact]
        public async Task SetVehicleReserved_WithNonExistentVehicle_ShouldReturnNotFound()
        {
            // Arrange
            var vehicleId = 999;
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.SetVehicleReserved(vehicleId, true))
                .ReturnsAsync(false);

            // Act
            var result = await _vehicleApplication.SetVehicleReserved(vehicleId, true);

            // Assert
            result.Should().NotBeNull();
            result.Code.Should().Be("404");
            result.Data.Should().BeFalse();
            result.Message.Should().Be("Vehicle not found.");
        }

        [Fact]
        public async Task SetVehicleReserved_WithException_ShouldReturnInternalServerError()
        {
            // Arrange
            var vehicleId = 1;
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.SetVehicleReserved(vehicleId, true))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _vehicleApplication.SetVehicleReserved(vehicleId, true);

            // Assert
            result.Should().NotBeNull();
            result.Code.Should().Be("500");
            result.Data.Should().BeFalse();
            result.Message.Should().StartWith("An unexpected error occurred:");
        }

        [Theory]
        [InlineData("COMMON_USER")]
        [InlineData("CLIENT_ADMIN")]
        [InlineData("CLIENT_EMPLOYEE")]
        [InlineData("ADMIN")]
        [InlineData("EMPLOYEE")]
        public async Task SetVehicleReserved_WithDifferentRoles_ShouldReturnSuccess(string role)
        {
            // Arrange
            var vehicleId = 1;
            var loggedUser = new LoggedUser { Roles = new List<string> { role } };
            
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.SetVehicleReserved(vehicleId, true))
                .ReturnsAsync(true);

            // Act
            var result = await _vehicleApplication.SetVehicleReserved(vehicleId, true);

            // Assert
            result.Should().NotBeNull();
            result.Code.Should().Be("200");
            result.Data.Should().BeTrue();
        }
    }
}
