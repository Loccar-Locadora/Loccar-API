using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using LoccarApplication;
using LoccarApplication.Interfaces;
using LoccarDomain;
using LoccarDomain.LoggedUser.Models;
using LoccarDomain.Vehicle.Models;
using LoccarInfra.Repositories.Interfaces;
using Moq;
using Xunit;

namespace LoccarTests.UnitTests
{
    [Collection("TestCollection")]
    public class VehicleApplicationTests
    {
        private readonly Mock<IAuthApplication> _mockAuthApplication;
        private readonly Mock<IVehicleRepository> _mockVehicleRepository;
        private readonly VehicleApplication _vehicleApplication;
        private readonly Fixture _fixture;

        public VehicleApplicationTests()
        {
            _mockAuthApplication = new Mock<IAuthApplication>();
            _mockVehicleRepository = new Mock<IVehicleRepository>();
            _vehicleApplication = new VehicleApplication(_mockAuthApplication.Object, _mockVehicleRepository.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task RegisterVehicle_WhenUserIsNotAuthorized_ReturnsUnauthorized()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Brand = "Test Brand",
                Model = "Test Model",
                Type = VehicleType.Passenger
            };
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Act
            var result = await _vehicleApplication.RegisterVehicle(vehicle);

            // Assert
            result.Code.Should().Be("401");
            result.Message.Should().Be("User not authorized.");
        }

        [Fact]
        public async Task RegisterVehicle_WhenUserIsAdmin_RegistersVehicleSuccessfully()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Brand = "Toyota",
                Model = "Corolla",
                Type = VehicleType.Cargo,
                CargoVehicle = new CargoVehicle
                {
                    CargoCapacity = 1000,
                    CargoType = "General",
                    TareWeight = 2000,
                    CargoCompartmentSize = "Large"
                }
            };
            
            var loggedUser = new LoggedUser { Roles = new List<string> { "ADMIN" } };
            var tbVehicle = new LoccarInfra.ORM.model.Vehicle { Idvehicle = 1 };
            var tbCargoVehicle = new LoccarInfra.ORM.model.CargoVehicle { Idvehicle = 1 };
            
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.RegisterVehicle(It.IsAny<LoccarInfra.ORM.model.Vehicle>()))
                .ReturnsAsync(tbVehicle);
            _mockVehicleRepository.Setup(x => x.RegisterCargoVehicle(It.IsAny<LoccarInfra.ORM.model.CargoVehicle>()))
                .ReturnsAsync(tbCargoVehicle);

            // Act
            var result = await _vehicleApplication.RegisterVehicle(vehicle);

            // Assert
            result.Code.Should().Be("201");
            result.Message.Should().Be("Vehicle registered successfully");
            result.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task RegisterVehicle_WhenExceptionOccurs_ReturnsServerError()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Brand = "Test Brand",
                Model = "Test Model",
                Type = VehicleType.Passenger
            };
            var loggedUser = new LoggedUser { Roles = new List<string> { "ADMIN" } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.RegisterVehicle(It.IsAny<LoccarInfra.ORM.model.Vehicle>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _vehicleApplication.RegisterVehicle(vehicle);

            // Assert
            result.Code.Should().Be("500");
            result.Message.Should().Contain("An unexpected error occurred");
        }

        [Fact]
        public async Task ListAvailableVehicles_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
        {
            // Arrange
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns((LoggedUser)null);

            // Act
            var result = await _vehicleApplication.ListAvailableVehicles();

            // Assert
            result.Code.Should().Be("401");
            result.Message.Should().Be("User not authorized");
        }

        [Fact]
        public async Task ListAvailableVehicles_WhenNoVehiclesAvailable_ReturnsNotFound()
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.ListAvailableVehicles())
                .ReturnsAsync((List<LoccarInfra.ORM.model.Vehicle>)null);

            // Act
            var result = await _vehicleApplication.ListAvailableVehicles();

            // Assert
            result.Code.Should().Be("404");
            result.Message.Should().Be("No available vehicles found.");
        }

        [Fact]
        public async Task ListAvailableVehicles_WhenVehiclesExist_ReturnsVehicles()
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            var tbVehicles = new List<LoccarInfra.ORM.model.Vehicle>
            {
                new LoccarInfra.ORM.model.Vehicle 
                { 
                    Idvehicle = 1, 
                    Brand = "Toyota", 
                    Model = "Corolla",
                    Reserved = false
                }
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.ListAvailableVehicles()).ReturnsAsync(tbVehicles);

            // Act
            var result = await _vehicleApplication.ListAvailableVehicles();

            // Assert
            result.Code.Should().Be("200");
            result.Message.Should().Be("Available vehicles list:");
            result.Data.Should().HaveCount(1);
            result.Data.First().Brand.Should().Be("Toyota");
        }

        [Theory]
        [InlineData("ADMIN", true)]
        [InlineData("EMPLOYEE", true)]
        [InlineData("COMMON_USER", false)]
        public async Task SetVehicleMaintenance_WithDifferentRoles_ReturnsExpectedResult(string role, bool shouldSucceed)
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { role } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            
            if (shouldSucceed)
            {
                _mockVehicleRepository.Setup(x => x.SetVehicleMaintenance(It.IsAny<int>(), It.IsAny<bool>()))
                    .ReturnsAsync(true);
            }

            // Act
            var result = await _vehicleApplication.SetVehicleMaintenance(1, true);

            // Assert
            if (shouldSucceed)
            {
                result.Code.Should().Be("200");
                result.Data.Should().BeTrue();
            }
            else
            {
                result.Code.Should().Be("401");
                result.Message.Should().Be("User not authorized.");
                result.Data.Should().BeFalse();
            }
        }

        [Fact]
        public async Task GetVehicleById_WhenVehicleExists_ReturnsVehicle()
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            var tbVehicle = new LoccarInfra.ORM.model.Vehicle 
            { 
                Idvehicle = 1, 
                Brand = "Toyota", 
                Model = "Corolla" 
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.GetVehicleById(1)).ReturnsAsync(tbVehicle);

            // Act
            var result = await _vehicleApplication.GetVehicleById(1);

            // Assert
            result.Code.Should().Be("200");
            result.Message.Should().Be("Vehicle found successfully.");
            result.Data.Should().NotBeNull();
            result.Data.Brand.Should().Be("Toyota");
        }

        [Fact]
        public async Task GetVehicleById_WhenVehicleNotFound_ReturnsNotFound()
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.GetVehicleById(1))
                .ReturnsAsync((LoccarInfra.ORM.model.Vehicle)null);

            // Act
            var result = await _vehicleApplication.GetVehicleById(1);

            // Assert
            result.Code.Should().Be("404");
            result.Message.Should().Be("Vehicle not found.");
        }

        [Fact]
        public async Task DeleteVehicle_WhenAuthorizedUser_DeletesSuccessfully()
        {
            // Arrange
            int vehicleId = 1;
            var loggedUser = new LoggedUser { Roles = new List<string> { "ADMIN" } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.DeleteVehicle(vehicleId)).ReturnsAsync(true);

            // Act
            var result = await _vehicleApplication.DeleteVehicle(vehicleId);

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().BeTrue();
            result.Message.Should().Be("Vehicle deleted successfully.");
        }

        [Fact]
        public async Task UpdateVehicle_WhenAuthorizedUser_UpdatesSuccessfully()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Idvehicle = 1,
                Brand = "Toyota",
                Model = "Corolla",
                DailyRate = 100.0m
            };
            var loggedUser = new LoggedUser { Roles = new List<string> { "ADMIN" } };
            var updatedTbVehicle = new LoccarInfra.ORM.model.Vehicle { Idvehicle = vehicle.Idvehicle };
            
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.UpdateVehicle(It.IsAny<LoccarInfra.ORM.model.Vehicle>()))
                .ReturnsAsync(updatedTbVehicle);

            // Act
            var result = await _vehicleApplication.UpdateVehicle(vehicle);

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().NotBeNull();
            result.Message.Should().Be("Vehicle updated successfully");
        }
    }
}