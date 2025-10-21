using AutoFixture;
using FluentAssertions;
using LoccarApplication;
using LoccarApplication.Interfaces;
using LoccarDomain;
using LoccarDomain.LoggedUser.Models;
using LoccarDomain.Vehicle.Models;
using LoccarInfra.Repositories.Interfaces;
using LoccarTests.Common;
using Moq;
using Xunit;

namespace LoccarTests.UnitTests.Applications
{
    public class VehicleApplicationTests : BaseUnitTest
    {
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<IAuthApplication> _authApplicationMock;
        private readonly VehicleApplication _vehicleApplication;

        public VehicleApplicationTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _authApplicationMock = new Mock<IAuthApplication>();
            _vehicleApplication = new VehicleApplication(_authApplicationMock.Object, _vehicleRepositoryMock.Object);
        }

        [Fact]
        public async Task RegisterVehicle_WhenUserIsUnauthorized_ShouldReturnUnauthorized()
        {
            // Arrange
            var vehicle = _fixture.Create<Vehicle>();
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Act
            var result = await _vehicleApplication.RegisterVehicle(vehicle);

            // Assert
            result.Code.Should().Be("401");
            result.Message.Should().Be("Usuário não autorizado.");
        }

        [Fact]
        public async Task RegisterVehicle_WhenUserIsAdmin_ShouldRegisterCargoVehicle()
        {
            // Arrange
            var cargoVehicle = _fixture.Create<CargoVehicle>();
            var vehicle = _fixture.Build<Vehicle>()
                .With(v => v.Type, VehicleType.Cargo)
                .With(v => v.CargoVehicle, cargoVehicle)
                .Without(v => v.Motorcycle)
                .Without(v => v.LeisureVehicle)
                .Without(v => v.PassengerVehicle)
                .Create();

            var loggedUser = new LoggedUser { Roles = new List<string> { "ADMIN" } };
            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            
            var tbVehicle = _fixture.Create<LoccarInfra.ORM.model.Vehicle>();
            var tbCargoVehicle = _fixture.Create<LoccarInfra.ORM.model.CargoVehicle>();
            
            _vehicleRepositoryMock.Setup(x => x.RegisterVehicle(It.IsAny<LoccarInfra.ORM.model.Vehicle>()))
                .ReturnsAsync(tbVehicle);
            _vehicleRepositoryMock.Setup(x => x.RegisterCargoVehicle(It.IsAny<LoccarInfra.ORM.model.CargoVehicle>()))
                .ReturnsAsync(tbCargoVehicle);

            // Act
            var result = await _vehicleApplication.RegisterVehicle(vehicle);

            // Assert
            result.Code.Should().Be("201");
            result.Message.Should().Be("Veículo cadastrado com sucesso");
            result.Data.Should().NotBeNull();
            _vehicleRepositoryMock.Verify(x => x.RegisterVehicle(It.IsAny<LoccarInfra.ORM.model.Vehicle>()), Times.Once);
            _vehicleRepositoryMock.Verify(x => x.RegisterCargoVehicle(It.IsAny<LoccarInfra.ORM.model.CargoVehicle>()), Times.Once);
        }

        [Theory]
        [InlineData(VehicleType.Cargo)]
        [InlineData(VehicleType.Motorcycle)]
        [InlineData(VehicleType.Leisure)]
        [InlineData(VehicleType.Passenger)]
        public async Task RegisterVehicle_WithDifferentVehicleTypes_ShouldHandleCorrectly(VehicleType vehicleType)
        {
            // Arrange
            var vehicle = CreateVehicleForType(vehicleType);
            var loggedUser = new LoggedUser { Roles = new List<string> { "ADMIN" } };
            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            SetupRepositoryMocksForVehicleType(vehicleType);

            // Act
            var result = await _vehicleApplication.RegisterVehicle(vehicle);

            // Assert
            result.Code.Should().Be("201");
            result.Message.Should().Be("Veículo cadastrado com sucesso");
        }

        [Fact]
        public async Task ListAvailableVehicles_WhenUserIsAuthenticated_ShouldReturnVehicles()
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            var tbVehicles = _fixture.CreateMany<LoccarInfra.ORM.model.Vehicle>(3).ToList();
            _vehicleRepositoryMock.Setup(x => x.ListAvailableVehicles()).ReturnsAsync(tbVehicles);

            // Act
            var result = await _vehicleApplication.ListAvailableVehicles();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().HaveCount(3);
            result.Message.Should().Be("Lista de veículos disponíveis:");
        }

        [Fact]
        public async Task ListAvailableVehicles_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
        {
            // Arrange
            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns((LoggedUser)null);

            // Act
            var result = await _vehicleApplication.ListAvailableVehicles();

            // Assert
            result.Code.Should().Be("401");
            result.Message.Should().Be("Usuário não autorizado");
        }

        [Fact]
        public async Task SetVehicleMaintenance_WhenUserIsNotAdmin_ShouldReturnUnauthorized()
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Act
            var result = await _vehicleApplication.SetVehicleMaintenance(1, true);

            // Assert
            result.Code.Should().Be("401");
            result.Message.Should().Be("Usuário não autorizado.");
            result.Data.Should().BeFalse();
        }

        [Theory]
        [InlineData(true, "200", "Veículo atualizado com sucesso.")]
        [InlineData(false, "404", "Veículo não encontrado.")]
        public async Task SetVehicleMaintenance_WhenUserIsAdmin_ShouldReturnExpectedResult(
            bool repositoryResult, string expectedCode, string expectedMessage)
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { "ADMIN" } };
            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _vehicleRepositoryMock.Setup(x => x.SetVehicleMaintenance(It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(repositoryResult);

            // Act
            var result = await _vehicleApplication.SetVehicleMaintenance(1, true);

            // Assert
            result.Code.Should().Be(expectedCode);
            result.Data.Should().Be(repositoryResult);
            result.Message.Should().Be(expectedMessage);
        }

        private Vehicle CreateVehicleForType(VehicleType vehicleType)
        {
            var vehicle = _fixture.Build<Vehicle>()
                .With(v => v.Type, vehicleType)
                .Without(v => v.CargoVehicle)
                .Without(v => v.Motorcycle)
                .Without(v => v.LeisureVehicle)
                .Without(v => v.PassengerVehicle)
                .Create();

            switch (vehicleType)
            {
                case VehicleType.Cargo:
                    vehicle.CargoVehicle = _fixture.Create<CargoVehicle>();
                    break;
                case VehicleType.Motorcycle:
                    vehicle.Motorcycle = _fixture.Create<Motorcycle>();
                    break;
                case VehicleType.Leisure:
                    vehicle.LeisureVehicle = _fixture.Create<LeisureVehicle>();
                    break;
                case VehicleType.Passenger:
                    vehicle.PassengerVehicle = _fixture.Create<PassengerVehicle>();
                    break;
            }

            return vehicle;
        }

        private void SetupRepositoryMocksForVehicleType(VehicleType vehicleType)
        {
            var tbVehicle = _fixture.Create<LoccarInfra.ORM.model.Vehicle>();
            _vehicleRepositoryMock.Setup(x => x.RegisterVehicle(It.IsAny<LoccarInfra.ORM.model.Vehicle>()))
                .ReturnsAsync(tbVehicle);

            switch (vehicleType)
            {
                case VehicleType.Cargo:
                    var tbCargoVehicle = _fixture.Create<LoccarInfra.ORM.model.CargoVehicle>();
                    _vehicleRepositoryMock.Setup(x => x.RegisterCargoVehicle(It.IsAny<LoccarInfra.ORM.model.CargoVehicle>()))
                        .ReturnsAsync(tbCargoVehicle);
                    break;
                case VehicleType.Motorcycle:
                    var tbMotorcycle = _fixture.Create<LoccarInfra.ORM.model.Motorcycle>();
                    _vehicleRepositoryMock.Setup(x => x.RegisterMotorcycleVehicle(It.IsAny<LoccarInfra.ORM.model.Motorcycle>()))
                        .ReturnsAsync(tbMotorcycle);
                    break;
                case VehicleType.Leisure:
                    var tbLeisureVehicle = _fixture.Create<LoccarInfra.ORM.model.LeisureVehicle>();
                    _vehicleRepositoryMock.Setup(x => x.RegisterLeisureVehicle(It.IsAny<LoccarInfra.ORM.model.LeisureVehicle>()))
                        .ReturnsAsync(tbLeisureVehicle);
                    break;
                case VehicleType.Passenger:
                    var tbPassengerVehicle = _fixture.Create<LoccarInfra.ORM.model.PassengerVehicle>();
                    _vehicleRepositoryMock.Setup(x => x.RegisterPassengerVehicle(It.IsAny<LoccarInfra.ORM.model.PassengerVehicle>()))
                        .ReturnsAsync(tbPassengerVehicle);
                    break;
            }
        }
    }
}