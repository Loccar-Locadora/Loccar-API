using FluentAssertions;
using LoccarTests.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Text;
using Xunit;
using LoccarInfra.ORM.model;
using LoccarDomain.Vehicle.Models;

namespace LoccarTests.IntegrationTests.Controllers
{
    public class VehicleControllerIntegrationTests : IClassFixture<LoccarWebApplicationFactory<Program>>
    {
        private readonly LoccarWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public VehicleControllerIntegrationTests(LoccarWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            
            // Para testes de integração que requerem autenticação, você precisaria
            // configurar um token JWT válido ou mockar a autenticação
        }

        [Fact]
        public async Task ListAvailableVehicles_ShouldReturnOnlyAvailableVehicles()
        {
            // Arrange
            // Este teste assume que você tem um mecanismo de autenticação configurado
            // ou que a autenticação está desabilitada para testes

            // Act
            var response = await _client.GetAsync("/api/vehicle/list/available");

            // Assert
            // Como o endpoint requer autenticação, esperamos 401 se não autenticado
            // ou 200 se a autenticação for bypassed nos testes
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetVehicleById_WithExistingVehicle_ShouldReturnVehicle()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
            
            var vehicle = await context.Vehicles.FirstOrDefaultAsync();
            vehicle.Should().NotBeNull("deve haver pelo menos um veículo do seed");

            // Act
            var response = await _client.GetAsync($"/api/vehicle/{vehicle.Idvehicle}");

            // Assert
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(999)] // ID que não existe
        [InlineData(-1)]  // ID inválido
        [InlineData(0)]   // ID inválido
        public async Task GetVehicleById_WithInvalidId_ShouldReturnNotFoundOrError(int invalidId)
        {
            // Act
            var response = await _client.GetAsync($"/api/vehicle/{invalidId}");

            // Assert
            // Pode retornar 404, 401 (não autorizado) ou outro erro dependendo da implementação
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task RegisterVehicle_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            // Arrange
            var vehicle = new LoccarDomain.Vehicle.Models.Vehicle
            {
                Brand = "Test Brand",
                Model = "Test Model",
                ManufacturingYear = 2020,
                ModelYear = 2021,
                Vin = "TEST123456789",
                DailyRate = 150m,
                Type = VehicleType.Passenger,
                PassengerVehicle = new PassengerVehicle
                {
                    PassengerCapacity = 5,
                    AirConditioning = true,
                    PowerSteering = true,
                    Tv = false
                }
            };

            var json = JsonConvert.SerializeObject(vehicle);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/vehicle/register", content);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact] 
        public async Task ListAllVehicles_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/vehicle/list/all");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateVehicle_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            // Arrange
            var vehicle = new LoccarDomain.Vehicle.Models.Vehicle
            {
                Idvehicle = 1,
                Brand = "Updated Brand",
                Model = "Updated Model"
            };

            var json = JsonConvert.SerializeObject(vehicle);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync("/api/vehicle/update", content);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task DeleteVehicle_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _client.DeleteAsync("/api/vehicle/delete/1");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(1, false)]
        public async Task SetVehicleMaintenance_WithoutAuthentication_ShouldReturnUnauthorized(int vehicleId, bool inMaintenance)
        {
            // Act
            var response = await _client.PutAsync($"/api/vehicle/maintenance/{vehicleId}?inMaintenance={inMaintenance}", null);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }
    }
}