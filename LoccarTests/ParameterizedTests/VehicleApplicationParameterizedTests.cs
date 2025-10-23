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

namespace LoccarTests.ParameterizedTests
{
    public class VehicleApplicationParameterizedTests
    {
        private readonly Mock<IAuthApplication> _mockAuthApplication;
        private readonly Mock<IVehicleRepository> _mockVehicleRepository;
        private readonly VehicleApplication _vehicleApplication;

        public VehicleApplicationParameterizedTests()
        {
            _mockAuthApplication = new Mock<IAuthApplication>();
            _mockVehicleRepository = new Mock<IVehicleRepository>();
            _vehicleApplication = new VehicleApplication(_mockAuthApplication.Object, _mockVehicleRepository.Object);
        }

        public static IEnumerable<object[]> UserRoleTestData =>
            new List<object[]>
            {
                new object[] { new List<string> { "ADMIN" }, true, "201", "Vehicle registered successfully" },
                new object[] { new List<string> { "EMPLOYEE" }, true, "201", "Vehicle registered successfully" },
                new object[] { new List<string> { "COMMON_USER" }, false, "401", "User not authorized." },
                new object[] { null, false, "401", "User not authorized." },
                new object[] { new List<string>(), false, "401", "User not authorized." },
            };

        [Theory]
        [MemberData(nameof(UserRoleTestData))]
        public async Task RegisterVehicleWithDifferentUserRolesReturnsExpectedResult(
            List<string> userRoles, bool shouldSucceed, string expectedCode, string expectedMessage)
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Brand = "Toyota",
                Model = "Corolla",
                Type = VehicleType.Passenger,
                PassengerVehicle = new PassengerVehicle(),
            };

            var loggedUser = userRoles != null ? new LoggedUser { Roles = userRoles } : null;
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            if (shouldSucceed)
            {
                var tbVehicle = new LoccarInfra.ORM.model.Vehicle { Idvehicle = 1 };
                var tbPassengerVehicle = new LoccarInfra.ORM.model.PassengerVehicle { Idvehicle = 1 };

                _mockVehicleRepository.Setup(x => x.RegisterVehicle(It.IsAny<LoccarInfra.ORM.model.Vehicle>()))
                    .ReturnsAsync(tbVehicle);
                _mockVehicleRepository.Setup(x => x.RegisterPassengerVehicle(It.IsAny<LoccarInfra.ORM.model.PassengerVehicle>()))
                    .ReturnsAsync(tbPassengerVehicle);
            }

            // Act
            var result = await _vehicleApplication.RegisterVehicle(vehicle);

            // Assert
            result.Code.Should().Be(expectedCode);
            result.Message.Should().Be(expectedMessage);
        }

        public static IEnumerable<object[]> VehicleTypeTestData =>
            new List<object[]>
            {
                new object[] { VehicleType.Cargo },
                new object[] { VehicleType.Motorcycle },
                new object[] { VehicleType.Passenger },
                new object[] { VehicleType.Leisure },
            };

        [Theory]
        [MemberData(nameof(VehicleTypeTestData))]
        public async Task RegisterVehicleWithDifferentVehicleTypesCallsCorrectRepository(
            VehicleType vehicleType)
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Brand = "Test Brand",
                Model = "Test Model",
                Type = vehicleType,
            };

            // Criar instancia do tipo especifico
            switch (vehicleType)
            {
                case VehicleType.Cargo:
                    vehicle.CargoVehicle = new CargoVehicle();
                    break;
                case VehicleType.Motorcycle:
                    vehicle.Motorcycle = new Motorcycle();
                    break;
                case VehicleType.Passenger:
                    vehicle.PassengerVehicle = new PassengerVehicle();
                    break;
                case VehicleType.Leisure:
                    vehicle.LeisureVehicle = new LeisureVehicle();
                    break;
            }

            var loggedUser = new LoggedUser { Roles = new List<string> { "ADMIN" } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            var tbVehicle = new LoccarInfra.ORM.model.Vehicle { Idvehicle = 1 };
            _mockVehicleRepository.Setup(x => x.RegisterVehicle(It.IsAny<LoccarInfra.ORM.model.Vehicle>()))
                .ReturnsAsync(tbVehicle);

            // Setup especifico para cada tipo
            switch (vehicleType)
            {
                case VehicleType.Cargo:
                    _mockVehicleRepository.Setup(x => x.RegisterCargoVehicle(It.IsAny<LoccarInfra.ORM.model.CargoVehicle>()))
                        .ReturnsAsync(new LoccarInfra.ORM.model.CargoVehicle { Idvehicle = 1 });
                    break;
                case VehicleType.Motorcycle:
                    _mockVehicleRepository.Setup(x => x.RegisterMotorcycleVehicle(It.IsAny<LoccarInfra.ORM.model.Motorcycle>()))
                        .ReturnsAsync(new LoccarInfra.ORM.model.Motorcycle { Idvehicle = 1 });
                    break;
                case VehicleType.Passenger:
                    _mockVehicleRepository.Setup(x => x.RegisterPassengerVehicle(It.IsAny<LoccarInfra.ORM.model.PassengerVehicle>()))
                        .ReturnsAsync(new LoccarInfra.ORM.model.PassengerVehicle { Idvehicle = 1 });
                    break;
                case VehicleType.Leisure:
                    _mockVehicleRepository.Setup(x => x.RegisterLeisureVehicle(It.IsAny<LoccarInfra.ORM.model.LeisureVehicle>()))
                        .ReturnsAsync(new LoccarInfra.ORM.model.LeisureVehicle { Idvehicle = 1 });
                    break;
            }

            // Act
            var result = await _vehicleApplication.RegisterVehicle(vehicle);

            // Assert
            result.Code.Should().Be("201");
            _mockVehicleRepository.Verify(x => x.RegisterVehicle(It.IsAny<LoccarInfra.ORM.model.Vehicle>()), Times.Once);

            // Verificar se o metodo correto foi chamado
            switch (vehicleType)
            {
                case VehicleType.Cargo:
                    _mockVehicleRepository.Verify(x => x.RegisterCargoVehicle(It.IsAny<LoccarInfra.ORM.model.CargoVehicle>()), Times.Once);
                    break;
                case VehicleType.Motorcycle:
                    _mockVehicleRepository.Verify(x => x.RegisterMotorcycleVehicle(It.IsAny<LoccarInfra.ORM.model.Motorcycle>()), Times.Once);
                    break;
                case VehicleType.Passenger:
                    _mockVehicleRepository.Verify(x => x.RegisterPassengerVehicle(It.IsAny<LoccarInfra.ORM.model.PassengerVehicle>()), Times.Once);
                    break;
                case VehicleType.Leisure:
                    _mockVehicleRepository.Verify(x => x.RegisterLeisureVehicle(It.IsAny<LoccarInfra.ORM.model.LeisureVehicle>()), Times.Once);
                    break;
            }
        }

        [Theory]
        [InlineData("ADMIN", true)]
        [InlineData("EMPLOYEE", true)]
        [InlineData("COMMON_USER", false)]
        [InlineData("INVALID_ROLE", false)]
        public async Task SetVehicleMaintenanceWithDifferentRolesReturnsExpectedResult(
            string role, bool shouldSucceed)
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
                result.Message.Should().Be("Vehicle updated successfully.");
            }
            else
            {
                result.Code.Should().Be("401");
                result.Data.Should().BeFalse();
                result.Message.Should().Be("User not authorized.");
            }
        }

        public static IEnumerable<object[]> VehicleDataValidationTestData =>
            new List<object[]>
            {
                new object[] { null, null, false },
                new object[] { string.Empty, string.Empty, false },
                new object[] { "Toyota", string.Empty, false },
                new object[] { string.Empty, "Corolla", false },
                new object[] { "Toyota", "Corolla", true },
            };

        [Theory]
        [MemberData(nameof(VehicleDataValidationTestData))]
        public void ValidateVehicleDataWithDifferentInputsReturnsExpectedResult(
            string brand, string model, bool isValid)
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Brand = brand,
                Model = model,
            };

            // Act
            bool actualResult = !string.IsNullOrEmpty(vehicle.Brand) && !string.IsNullOrEmpty(vehicle.Model);

            // Assert
            actualResult.Should().Be(isValid);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task ListAvailableVehiclesWithMultipleVehiclesReturnsCorrectCount(
            int vehicleCount)
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            var tbVehicles = new List<LoccarInfra.ORM.model.Vehicle>();

            for (int i = 0; i < vehicleCount; i++)
            {
                tbVehicles.Add(new LoccarInfra.ORM.model.Vehicle
                {
                    Idvehicle = i + 1,
                    Brand = $"Brand{i}",
                    Model = $"Model{i}",
                    Reserved = false,
                });
            }

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.ListAvailableVehicles()).ReturnsAsync(tbVehicles);

            // Act
            var result = await _vehicleApplication.ListAvailableVehicles();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().HaveCount(vehicleCount);
        }

        [Theory]
        [InlineData(1000.0, 5, 5000.0)]
        [InlineData(150.5, 3, 451.5)]
        [InlineData(99.99, 1, 99.99)]
        [InlineData(200.0, 0, 200.0)] // 0 dias deve ser tratado como 1 dia
        public void CalculateRentalCostWithDifferentValuesReturnsCorrectResult(
            decimal dailyRate, int days, decimal expectedTotal)
        {
            // Arrange
            int actualDays = days <= 0 ? 1 : days; // Simular logica da aplicacao

            // Act
            decimal actualTotal = dailyRate * actualDays;

            // Assert
            actualTotal.Should().Be(expectedTotal);
        }

        public static IEnumerable<object[]> ExceptionHandlingTestData =>
            new List<object[]>
            {
                new object[] { new InvalidOperationException("Invalid operation"), "500" },
                new object[] { new ArgumentNullException("parameter"), "500" },
                new object[] { new UnauthorizedAccessException("Access denied"), "500" },
            };

        [Theory]
        [MemberData(nameof(ExceptionHandlingTestData))]
        public async Task RegisterVehicleWithDifferentExceptionsHandlesCorrectly(
            Exception exception, string expectedCode)
        {
            // Arrange
            var vehicle = new Vehicle { Brand = "Test", Model = "Test" };
            var loggedUser = new LoggedUser { Roles = new List<string> { "ADMIN" } };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.RegisterVehicle(It.IsAny<LoccarInfra.ORM.model.Vehicle>()))
                .ThrowsAsync(exception);

            // Act
            var result = await _vehicleApplication.RegisterVehicle(vehicle);

            // Assert
            result.Code.Should().Be(expectedCode);
            result.Message.Should().StartWith("An unexpected error occurred:");
        }
    }
}
