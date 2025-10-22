using AutoFixture;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using LoccarApplication;
using LoccarApplication.Interfaces;
using LoccarDomain.LoggedUser.Models;
using LoccarDomain.Vehicle.Models;
using LoccarInfra.Repositories.Interfaces;
using Moq;

namespace LoccarTests.PerformanceTests
{
    [MemoryDiagnoser]
    [SimpleJob]
    public class VehicleApplicationBenchmarks
    {
        private VehicleApplication _vehicleApplication;
        private Mock<IAuthApplication> _mockAuthApplication;
        private Mock<IVehicleRepository> _mockVehicleRepository;
        private List<LoccarInfra.ORM.model.Vehicle> _vehiclesList;
        private Fixture _fixture;

        [GlobalSetup]
        public void Setup()
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
                    Idvehicle = i + 1,
                    Brand = $"Brand{i}",
                    Model = $"Model{i}",
                    ManufacturingYear = 2020 + (i % 4),
                    ModelYear = 2020 + (i % 4),
                    DailyRate = 100 + (i % 100),
                    Reserved = i % 10 == 0 // 10% reserved
                });
            }
        }

        [Benchmark]
        [Arguments(10)]
        [Arguments(100)]
        [Arguments(1000)]
        public async Task ListAvailableVehicles_Performance(int vehicleCount)
        {
            // Arrange
            var vehicles = _vehiclesList.Take(vehicleCount).ToList();
            _mockVehicleRepository.Setup(x => x.ListAvailableVehicles())
                .ReturnsAsync(vehicles);

            // Act
            var result = await _vehicleApplication.ListAvailableVehicles();
        }

        [Benchmark]
        public async Task RegisterVehicle_SingleVehicle()
        {
            // Arrange
            var adminUser = new LoggedUser { Roles = new List<string> { "ADMIN" } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(adminUser);
            
            var vehicle = new Vehicle
            {
                Brand = "Toyota",
                Model = "Corolla",
                Type = VehicleType.Passenger,
                PassengerVehicle = new PassengerVehicle()
            };

            var tbVehicle = new LoccarInfra.ORM.model.Vehicle { Idvehicle = 1 };
            var tbPassengerVehicle = new LoccarInfra.ORM.model.PassengerVehicle { Idvehicle = 1 };

            _mockVehicleRepository.Setup(x => x.RegisterVehicle(It.IsAny<LoccarInfra.ORM.model.Vehicle>()))
                .ReturnsAsync(tbVehicle);
            _mockVehicleRepository.Setup(x => x.RegisterPassengerVehicle(It.IsAny<LoccarInfra.ORM.model.PassengerVehicle>()))
                .ReturnsAsync(tbPassengerVehicle);

            // Act
            var result = await _vehicleApplication.RegisterVehicle(vehicle);
        }

        [Benchmark]
        [Arguments(10)]
        [Arguments(50)]
        [Arguments(100)]
        public async Task GetVehicleById_MultipleCalls(int callCount)
        {
            // Arrange
            var vehicle = _vehiclesList.First();
            _mockVehicleRepository.Setup(x => x.GetVehicleById(It.IsAny<int>()))
                .ReturnsAsync(vehicle);

            // Act
            var tasks = new List<Task>();
            for (int i = 0; i < callCount; i++)
            {
                tasks.Add(_vehicleApplication.GetVehicleById(1));
            }
            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public void CreateVehicleObject_Performance()
        {
            // Act
            for (int i = 0; i < 1000; i++)
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
                        PowerSteering = true
                    }
                };
            }
        }
    }

    // Classe para executar os benchmarks
    public class BenchmarkRunnerHelper
    {
        public static void RunBenchmarks()
        {
            var summary = BenchmarkRunner.Run<VehicleApplicationBenchmarks>();
        }
    }
}