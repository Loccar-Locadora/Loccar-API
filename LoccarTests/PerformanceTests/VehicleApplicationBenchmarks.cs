using AutoFixture;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using FluentAssertions;
using LoccarApplication;
using LoccarApplication.Interfaces;
using LoccarDomain.LoggedUser.Models;
using LoccarDomain.Vehicle.Models;
using LoccarInfra.Repositories.Interfaces;
using Moq;
using Xunit;

namespace LoccarTests.PerformanceTests
{
    [Collection("TestCollection")]
    public class VehicleApplicationBenchmarks
    {
        private VehicleApplication _vehicleApplication;
        private Mock<IAuthApplication> _mockAuthApplication;
        private Mock<IVehicleRepository> _mockVehicleRepository;
        private List<LoccarInfra.ORM.model.Vehicle> _vehiclesList;
        private Fixture _fixture;

        public VehicleApplicationBenchmarks()
        {
            Setup();
        }

        private void Setup()
        {
            _fixture = new Fixture();
            _mockAuthApplication = new Mock<IAuthApplication>();
            _mockVehicleRepository = new Mock<IVehicleRepository>();
            _vehicleApplication = new VehicleApplication(_mockAuthApplication.Object, _mockVehicleRepository.Object);

            // Setup common mocks
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Create test data
            _vehiclesList = new List<LoccarInfra.ORM.model.Vehicle>();
            for (int i = 0; i < 1000; i++)
            {
                _vehiclesList.Add(new LoccarInfra.ORM.model.Vehicle
                {
                    IdVehicle = i + 1,
                    Brand = $"Brand{i}",
                    Model = $"Model{i}",
                    ManufacturingYear = 2020 + (i % 4),
                    ModelYear = 2020 + (i % 4),
                    DailyRate = 100 + (i % 100),
                    Reserved = i % 10 == 0, // 10% reserved
                });
            }
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task ListAvailableVehiclesPerformance(int vehicleCount)
        {
            // Reduzir carga em ambiente CI
            if (Environment.GetEnvironmentVariable("CI") == "true")
            {
                vehicleCount = Math.Min(vehicleCount, 10);
            }

            // Arrange
            var vehicles = _vehiclesList.Take(vehicleCount).ToList();
            _mockVehicleRepository.Setup(x => x.ListAvailableVehicles())
                .ReturnsAsync(vehicles);

            // Act
            var result = await _vehicleApplication.ListAvailableVehicles();

            // Assert
            result.Should().NotBeNull();
            result.Code.Should().Be("200");
        }

        [Fact]
        public async Task RegisterVehicleSingleVehicle()
        {
            // Arrange
            var adminUser = new LoggedUser { Roles = new List<string> { "CLIENT_ADMIN" } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(adminUser);

            var vehicle = new Vehicle
            {
                Brand = "Toyota",
                Model = "Corolla",
                Type = VehicleType.Passenger,
                PassengerVehicle = new PassengerVehicle(),
            };

            var tbVehicle = new LoccarInfra.ORM.model.Vehicle { IdVehicle = 1 };
            var tbPassengerVehicle = new LoccarInfra.ORM.model.PassengerVehicle { IdVehicle = 1 };

            _mockVehicleRepository.Setup(x => x.RegisterVehicle(It.IsAny<LoccarInfra.ORM.model.Vehicle>()))
                .ReturnsAsync(tbVehicle);
            _mockVehicleRepository.Setup(x => x.RegisterPassengerVehicle(It.IsAny<LoccarInfra.ORM.model.PassengerVehicle>()))
                .ReturnsAsync(tbPassengerVehicle);

            // Act
            var result = await _vehicleApplication.RegisterVehicle(vehicle);

            // Assert
            result.Should().NotBeNull();
            result.Code.Should().Be("201");
        }

        [Theory]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        public async Task GetVehicleByIdMultipleCalls(int callCount)
        {
            // Reduzir carga em ambiente CI
            if (Environment.GetEnvironmentVariable("CI") == "true")
            {
                callCount = Math.Min(callCount, 10);
            }

            // Arrange
            var vehicle = _vehiclesList.FirstOrDefault();
            vehicle.Should().NotBeNull(); // Garantir que temos dados

            _mockVehicleRepository.Setup(x => x.GetVehicleById(It.IsAny<int>()))
                .ReturnsAsync(vehicle);

            // Act
            var tasks = new List<Task>();
            for (int i = 0; i < callCount; i++)
            {
                tasks.Add(_vehicleApplication.GetVehicleById(1));
            }

            await Task.WhenAll(tasks);

            // Assert - apenas verificar que não houve exceção
            Assert.True(true);
        }

        [Fact]
        public void CreateVehicleObjectPerformance()
        {
            // Reduzir iterações em ambiente CI
            int iterations = Environment.GetEnvironmentVariable("CI") == "true" ? 100 : 1000;

            // Act
            for (int i = 0; i < iterations; i++)
            {
                var vehicle = new Vehicle
                {
                    Brand = $"Brand{i}",
                    Model = $"Model{i}",
                    ManufacturingYear = 2023,
                    ModelYear = 2023,
                    DailyRate = 100.0m,
                    Type = VehicleType.Passenger,
                    PassengerVehicle = new PassengerVehicle
                    {
                        PassengerCapacity = 5,
                        AirConditioning = true,
                        PowerSteering = true,
                    },
                };
            }

            // Assert - apenas verificar que não houve exceção
            Assert.True(true);
        }
    }

    // Classe para executar os benchmarks reais (BenchmarkDotNet)
    [MemoryDiagnoser]
    [SimpleJob]
    public class ActualBenchmarks
    {
        private VehicleApplication _vehicleApplication;
        private Mock<IAuthApplication> _mockAuthApplication;
        private Mock<IVehicleRepository> _mockVehicleRepository;
        private List<LoccarInfra.ORM.model.Vehicle> _vehiclesList;

        [GlobalSetup]
        public void Setup()
        {
            _mockAuthApplication = new Mock<IAuthApplication>();
            _mockVehicleRepository = new Mock<IVehicleRepository>();
            _vehicleApplication = new VehicleApplication(_mockAuthApplication.Object, _mockVehicleRepository.Object);

            // Setup common mocks
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Create test data
            _vehiclesList = new List<LoccarInfra.ORM.model.Vehicle>();
            for (int i = 0; i < 1000; i++)
            {
                _vehiclesList.Add(new LoccarInfra.ORM.model.Vehicle
                {
                    IdVehicle = i + 1,
                    Brand = $"Brand{i}",
                    Model = $"Model{i}",
                    ManufacturingYear = 2020 + (i % 4),
                    ModelYear = 2020 + (i % 4),
                    DailyRate = 100 + (i % 100),
                    Reserved = i % 10 == 0,
                });
            }

            _mockVehicleRepository.Setup(x => x.ListAvailableVehicles())
                .ReturnsAsync(_vehiclesList);
        }

        [Benchmark]
        public async Task<LoccarDomain.BaseReturn<List<Vehicle>>> ListAvailableVehiclesBenchmark()
        {
            return await _vehicleApplication.ListAvailableVehicles();
        }
    }

    // Classe para executar os benchmarks manualmente
    public static class BenchmarkRunnerHelper
    {
        public static void RunBenchmarks()
        {
            if (Environment.GetEnvironmentVariable("CI") != "true")
            {
                var summary = BenchmarkRunner.Run<ActualBenchmarks>();
            }
        }
    }
}
