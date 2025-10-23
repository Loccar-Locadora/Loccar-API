using FluentAssertions;
using LoccarApplication;
using LoccarApplication.Interfaces;
using LoccarDomain.LoggedUser.Models;
using LoccarDomain.Vehicle.Models;
using LoccarInfra.ORM.model;
using LoccarInfra.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace LoccarTests.IntegrationTests
{
    [Collection("TestCollection")]
    public class VehicleApplicationIntegrationTests : IDisposable
    {
        private readonly DataBaseContext _context;
        private readonly VehicleRepository _vehicleRepository;
        private readonly Mock<IAuthApplication> _mockAuthApplication;
        private readonly VehicleApplication _vehicleApplication;

        public VehicleApplicationIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<DataBaseContext>()
                .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new DataBaseContext(options);
            _vehicleRepository = new VehicleRepository(_context);
            _mockAuthApplication = new Mock<IAuthApplication>();
            _vehicleApplication = new VehicleApplication(_mockAuthApplication.Object, _vehicleRepository);
        }

        [Fact]
        public async Task RegisterVehicleWhenValidDataSavesToDatabase()
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { "ADMIN" } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            var vehicle = new LoccarDomain.Vehicle.Models.Vehicle
            {
                Brand = "Toyota",
                Model = "Corolla",
                ManufacturingYear = 2023,
                ModelYear = 2023,
                DailyRate = 100.0m,
                MonthlyRate = 2500.0m,
                CompanyDailyRate = 80.0m,
                ReducedDailyRate = 70.0m,
                FuelTankCapacity = 50,
                Vin = "1HGBH41JXMN109186",
                Type = VehicleType.Passenger,
                PassengerVehicle = new LoccarDomain.Vehicle.Models.PassengerVehicle
                {
                    PassengerCapacity = 5,
                    Tv = true,
                    AirConditioning = true,
                    PowerSteering = true,
                },
            };

            // Act
            var result = await _vehicleApplication.RegisterVehicle(vehicle);

            // Assert
            result.Code.Should().Be("201");
            result.Message.Should().Be("Vehicle registered successfully");

            var vehicleInDb = await _context.Vehicles.FirstOrDefaultAsync(v => v.Brand == "Toyota");
            vehicleInDb.Should().NotBeNull();
            vehicleInDb.Model.Should().Be("Corolla");
            vehicleInDb.DailyRate.Should().Be(100.0m);
        }

        [Fact]
        public async Task ListAvailableVehiclesWhenVehiclesExistReturnsFromDatabase()
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Seed data
            var vehicle1 = new LoccarInfra.ORM.model.Vehicle
            {
                Brand = "Honda",
                Model = "Civic",
                DailyRate = 90.0m,
                Reserved = false,
            };

            var vehicle2 = new LoccarInfra.ORM.model.Vehicle
            {
                Brand = "Ford",
                Model = "Focus",
                DailyRate = 85.0m,
                Reserved = true, // Este nao deve aparecer na lista de disponiveis
            };

            _context.Vehicles.AddRange(vehicle1, vehicle2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _vehicleApplication.ListAvailableVehicles();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().HaveCount(1);
            result.Data.First().Brand.Should().Be("Honda");
            result.Data.First().Model.Should().Be("Civic");
        }

        [Fact]
        public async Task GetVehicleByIdWhenVehicleExistsReturnsFromDatabase()
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            var vehicle = new LoccarInfra.ORM.model.Vehicle
            {
                Brand = "Nissan",
                Model = "Sentra",
                DailyRate = 95.0m,
                Reserved = false,
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            // Act
            var result = await _vehicleApplication.GetVehicleById(vehicle.Idvehicle);

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().NotBeNull();
            result.Data.Brand.Should().Be("Nissan");
            result.Data.Model.Should().Be("Sentra");
        }

        [Fact]
        public async Task UpdateVehicleWhenValidDataUpdatesInDatabase()
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { "ADMIN" } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            var vehicle = new LoccarInfra.ORM.model.Vehicle
            {
                Brand = "Volkswagen",
                Model = "Gol",
                DailyRate = 80.0m,
                Reserved = false,
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            var updatedVehicle = new LoccarDomain.Vehicle.Models.Vehicle
            {
                Idvehicle = vehicle.Idvehicle,
                Brand = "Volkswagen",
                Model = "Gol Updated",
                DailyRate = 85.0m,
                Reserved = false,
            };

            // Act
            var result = await _vehicleApplication.UpdateVehicle(updatedVehicle);

            // Assert
            result.Code.Should().Be("200");
            result.Message.Should().Be("Vehicle updated successfully");

            var vehicleInDb = await _context.Vehicles.FindAsync(vehicle.Idvehicle);
            vehicleInDb.Should().NotBeNull();
            vehicleInDb.Model.Should().Be("Gol Updated");
            vehicleInDb.DailyRate.Should().Be(85.0m);
        }

        [Fact]
        public async Task DeleteVehicleWhenVehicleExistsRemovesFromDatabase()
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { "ADMIN" } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            var vehicle = new LoccarInfra.ORM.model.Vehicle
            {
                Brand = "Chevrolet",
                Model = "Onix",
                DailyRate = 75.0m,
                Reserved = false,
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            // Act
            var result = await _vehicleApplication.DeleteVehicle(vehicle.Idvehicle);

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().BeTrue();
            result.Message.Should().Be("Vehicle deleted successfully.");

            var vehicleInDb = await _context.Vehicles.FindAsync(vehicle.Idvehicle);
            vehicleInDb.Should().BeNull();
        }

        [Theory]
        [InlineData("ADMIN", true)]
        [InlineData("EMPLOYEE", true)]
        [InlineData("COMMON_USER", false)]
        public async Task SetVehicleMaintenanceWithDifferentRolesBehavesCorrectly(string role, bool shouldSucceed)
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { role } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            var vehicle = new LoccarInfra.ORM.model.Vehicle
            {
                Brand = "Hyundai",
                Model = "HB20",
                DailyRate = 70.0m,
                Reserved = false,
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            // Act
            var result = await _vehicleApplication.SetVehicleMaintenance(vehicle.Idvehicle, true);

            // Assert
            if (shouldSucceed)
            {
                result.Code.Should().Be("200");
                result.Data.Should().BeTrue();
            }
            else
            {
                result.Code.Should().Be("401");
                result.Data.Should().BeFalse();
                result.Message.Should().Be("User not authorized.");
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
