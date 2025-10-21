using AutoFixture;
using FluentAssertions;
using LoccarApplication;
using LoccarApplication.Interfaces;
using LoccarDomain.LoggedUser.Models;
using LoccarDomain.Vehicle.Models;
using LoccarInfra.Repositories.Interfaces;
using LoccarTests.Common;
using Moq;
using Xunit;

namespace LoccarTests.ParametrizedTests
{
    public class VehicleBusinessRulesParametrizedTests : BaseUnitTest
    {
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<IAuthApplication> _authApplicationMock;
        private readonly VehicleApplication _vehicleApplication;

        public VehicleBusinessRulesParametrizedTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _authApplicationMock = new Mock<IAuthApplication>();
            _vehicleApplication = new VehicleApplication(_authApplicationMock.Object, _vehicleRepositoryMock.Object);
        }

        public static IEnumerable<object[]> GetUserRolesData()
        {
            yield return new object[] { new List<string> { "ADMIN" }, true };
            yield return new object[] { new List<string> { "EMPLOYEE" }, true };
            yield return new object[] { new List<string> { "ADMIN", "EMPLOYEE" }, true };
            yield return new object[] { new List<string> { "COMMON_USER" }, false };
            yield return new object[] { new List<string>(), false };
            yield return new object[] { null, false };
        }

        public static IEnumerable<object[]> GetVehicleTypesData()
        {
            yield return new object[] { VehicleType.Cargo, typeof(CargoVehicle) };
            yield return new object[] { VehicleType.Motorcycle, typeof(Motorcycle) };
            yield return new object[] { VehicleType.Leisure, typeof(LeisureVehicle) };
            yield return new object[] { VehicleType.Passenger, typeof(PassengerVehicle) };
        }

        public static IEnumerable<object[]> GetVehicleRatesData()
        {
            yield return new object[] { 50.00m, 1400.00m, 45.00m, 55.00m };   // Rates baixas
            yield return new object[] { 100.00m, 2800.00m, 90.00m, 110.00m }; // Rates médias
            yield return new object[] { 200.00m, 5600.00m, 180.00m, 220.00m }; // Rates altas
            yield return new object[] { 0.01m, 0.28m, 0.01m, 0.01m };         // Rates mínimas
            yield return new object[] { 9999.99m, 279999.72m, 8999.99m, 10999.99m }; // Rates máximas
        }

        public static IEnumerable<object[]> GetMaintenanceScenarios()
        {
            yield return new object[] { 1, true, true, "200", "Veículo atualizado com sucesso." };
            yield return new object[] { 1, false, true, "200", "Veículo atualizado com sucesso." };
            yield return new object[] { 999, true, false, "404", "Veículo não encontrado." };
            yield return new object[] { 999, false, false, "404", "Veículo não encontrado." };
        }

        [Theory]
        [MemberData(nameof(GetUserRolesData))]
        public async Task RegisterVehicle_WithDifferentUserRoles_ShouldHandleAuthorization(
            List<string> roles, bool shouldSucceed)
        {
            // Arrange
            var vehicle = CreateValidCargoVehicle();
            var loggedUser = new LoggedUser { Roles = roles };
            
            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            
            if (shouldSucceed)
            {
                SetupSuccessfulVehicleRegistration();
            }

            // Act
            var result = await _vehicleApplication.RegisterVehicle(vehicle);

            // Assert
            if (shouldSucceed)
            {
                result.Code.Should().Be("201");
                result.Message.Should().Be("Veículo cadastrado com sucesso");
            }
            else
            {
                result.Code.Should().Be("401");
                result.Message.Should().Be("Usuário não autorizado.");
            }
        }

        [Theory]
        [MemberData(nameof(GetVehicleTypesData))]
        public async Task RegisterVehicle_WithDifferentVehicleTypes_ShouldCallCorrectRepository(
            VehicleType vehicleType, Type expectedSpecializationType)
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
            
            // Verificar que o método correto do repositório foi chamado
            _vehicleRepositoryMock.Verify(x => x.RegisterVehicle(It.IsAny<LoccarInfra.ORM.model.Vehicle>()), Times.Once);
            
            switch (vehicleType)
            {
                case VehicleType.Cargo:
                    _vehicleRepositoryMock.Verify(x => x.RegisterCargoVehicle(It.IsAny<LoccarInfra.ORM.model.CargoVehicle>()), Times.Once);
                    break;
                case VehicleType.Motorcycle:
                    _vehicleRepositoryMock.Verify(x => x.RegisterMotorcycleVehicle(It.IsAny<LoccarInfra.ORM.model.Motorcycle>()), Times.Once);
                    break;
                case VehicleType.Leisure:
                    _vehicleRepositoryMock.Verify(x => x.RegisterLeisureVehicle(It.IsAny<LoccarInfra.ORM.model.LeisureVehicle>()), Times.Once);
                    break;
                case VehicleType.Passenger:
                    _vehicleRepositoryMock.Verify(x => x.RegisterPassengerVehicle(It.IsAny<LoccarInfra.ORM.model.PassengerVehicle>()), Times.Once);
                    break;
            }
        }

        [Theory]
        [MemberData(nameof(GetVehicleRatesData))]
        public async Task RegisterVehicle_WithDifferentRates_ShouldAcceptAllValidValues(
            decimal dailyRate, decimal monthlyRate, decimal reducedDailyRate, decimal companyDailyRate)
        {
            // Arrange
            var vehicle = CreateValidCargoVehicle();
            vehicle.DailyRate = dailyRate;
            vehicle.MonthlyRate = monthlyRate;
            vehicle.ReducedDailyRate = reducedDailyRate;
            vehicle.CompanyDailyRate = companyDailyRate;

            var loggedUser = new LoggedUser { Roles = new List<string> { "ADMIN" } };
            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            SetupSuccessfulVehicleRegistration();

            // Act
            var result = await _vehicleApplication.RegisterVehicle(vehicle);

            // Assert
            result.Code.Should().Be("201");
            result.Data.Should().NotBeNull();
            result.Data.DailyRate.Should().Be(dailyRate);
            result.Data.MonthlyRate.Should().Be(monthlyRate);
        }

        [Theory]
        [MemberData(nameof(GetMaintenanceScenarios))]
        public async Task SetVehicleMaintenance_WithDifferentScenarios_ShouldReturnExpectedResults(
            int vehicleId, bool inMaintenance, bool repositorySuccess, string expectedCode, string expectedMessage)
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { "ADMIN" } };
            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _vehicleRepositoryMock.Setup(x => x.SetVehicleMaintenance(vehicleId, inMaintenance))
                .ReturnsAsync(repositorySuccess);

            // Act
            var result = await _vehicleApplication.SetVehicleMaintenance(vehicleId, inMaintenance);

            // Assert
            result.Code.Should().Be(expectedCode);
            result.Message.Should().Be(expectedMessage);
            result.Data.Should().Be(repositorySuccess);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task RegisterVehicle_WithInvalidBrand_ShouldStillRegister(string invalidBrand)
        {
            // Arrange - A aplicação não está fazendo validação de marca
            var vehicle = CreateValidCargoVehicle();
            vehicle.Brand = invalidBrand;

            var loggedUser = new LoggedUser { Roles = new List<string> { "ADMIN" } };
            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            SetupSuccessfulVehicleRegistration();

            // Act
            var result = await _vehicleApplication.RegisterVehicle(vehicle);

            // Assert
            result.Code.Should().Be("201"); // A aplicação aceita marcas inválidas
        }

        [Theory]
        [InlineData(1800, 2024)] // Ano muito antigo
        [InlineData(2024, 2030)] // Ano futuro
        [InlineData(2020, 2020)] // Mesmo ano
        public async Task RegisterVehicle_WithDifferentYears_ShouldAccept(int manufacturingYear, int modelYear)
        {
            // Arrange
            var vehicle = CreateValidCargoVehicle();
            vehicle.ManufacturingYear = manufacturingYear;
            vehicle.ModelYear = modelYear;

            var loggedUser = new LoggedUser { Roles = new List<string> { "ADMIN" } };
            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            SetupSuccessfulVehicleRegistration();

            // Act
            var result = await _vehicleApplication.RegisterVehicle(vehicle);

            // Assert
            result.Code.Should().Be("201");
            result.Data.ManufacturingYear.Should().Be(manufacturingYear);
            result.Data.ModelYear.Should().Be(modelYear);
        }

        private Vehicle CreateValidCargoVehicle()
        {
            return _fixture.Build<Vehicle>()
                .With(v => v.Type, VehicleType.Cargo)
                .With(v => v.CargoVehicle, _fixture.Create<CargoVehicle>())
                .Without(v => v.Motorcycle)
                .Without(v => v.LeisureVehicle)
                .Without(v => v.PassengerVehicle)
                .Create();
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

        private void SetupSuccessfulVehicleRegistration()
        {
            var tbVehicle = _fixture.Create<LoccarInfra.ORM.model.Vehicle>();
            var tbCargoVehicle = _fixture.Create<LoccarInfra.ORM.model.CargoVehicle>();
            
            _vehicleRepositoryMock.Setup(x => x.RegisterVehicle(It.IsAny<LoccarInfra.ORM.model.Vehicle>()))
                .ReturnsAsync(tbVehicle);
            _vehicleRepositoryMock.Setup(x => x.RegisterCargoVehicle(It.IsAny<LoccarInfra.ORM.model.CargoVehicle>()))
                .ReturnsAsync(tbCargoVehicle);
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