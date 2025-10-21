using FluentAssertions;
using LoccarApplication;
using LoccarApplication.Interfaces;
using LoccarDomain;
using LoccarDomain.LoggedUser.Models;
using LoccarDomain.Reservation.Models;
using LoccarInfra.Repositories.Interfaces;
using LoccarTests.Common;
using Moq;
using Xunit;

namespace LoccarTests.UnitTests.Applications
{
    public class ReservationApplicationTests : BaseUnitTest
    {
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IAuthApplication> _authApplicationMock;
        private readonly ReservationApplication _reservationApplication;

        public ReservationApplicationTests()
        {
            _reservationRepositoryMock = new Mock<IReservationRepository>();
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _authApplicationMock = new Mock<IAuthApplication>();
            _reservationApplication = new ReservationApplication(
                _reservationRepositoryMock.Object,
                _vehicleRepositoryMock.Object,
                _customerRepositoryMock.Object,
                _authApplicationMock.Object);
        }

        [Fact]
        public async Task CreateReservation_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
        {
            // Arrange
            var reservation = _fixture.Create<Reservation>();
            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns((LoggedUser)null);

            // Act
            var result = await _reservationApplication.CreateReservation(reservation);

            // Assert
            result.Code.Should().Be("401");
            result.Message.Should().Be("Usuário não autorizado.");
        }

        [Fact]
        public async Task CreateReservation_WhenVehicleNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var reservation = _fixture.Create<Reservation>();
            var loggedUser = _fixture.Create<LoggedUser>();
            
            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _vehicleRepositoryMock.Setup(x => x.GetVehicleById(reservation.Idvehicle))
                .ReturnsAsync((LoccarInfra.ORM.model.Vehicle)null);

            // Act
            var result = await _reservationApplication.CreateReservation(reservation);

            // Assert
            result.Code.Should().Be("404");
            result.Message.Should().Be("Veículo não encontrado.");
        }

        [Fact]
        public async Task CreateReservation_WhenVehicleIsNotAvailable_ShouldReturnBadRequest()
        {
            // Arrange
            var reservation = _fixture.Create<Reservation>();
            var loggedUser = _fixture.Create<LoggedUser>();
            var vehicle = _fixture.Build<LoccarInfra.ORM.model.Vehicle>()
                .With(v => v.Reserved, true)
                .Create();
            
            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _vehicleRepositoryMock.Setup(x => x.GetVehicleById(reservation.Idvehicle))
                .ReturnsAsync(vehicle);

            // Act
            var result = await _reservationApplication.CreateReservation(reservation);

            // Assert
            result.Code.Should().Be("400");
            result.Message.Should().Be("Veículo não está disponível.");
        }

        [Fact]
        public async Task CreateReservation_WhenCustomerNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var reservation = _fixture.Create<Reservation>();
            var loggedUser = _fixture.Create<LoggedUser>();
            var vehicle = _fixture.Build<LoccarInfra.ORM.model.Vehicle>()
                .With(v => v.Reserved, false)
                .Create();
            
            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _vehicleRepositoryMock.Setup(x => x.GetVehicleById(reservation.Idvehicle))
                .ReturnsAsync(vehicle);
            _customerRepositoryMock.Setup(x => x.GetCustomerById(reservation.Idcustomer))
                .ReturnsAsync((LoccarInfra.ORM.model.Customer)null);

            // Act
            var result = await _reservationApplication.CreateReservation(reservation);

            // Assert
            result.Code.Should().Be("404");
            result.Message.Should().Be("Cliente não encontrado.");
        }

        [Fact]
        public async Task CreateReservation_WhenAllValidationsPass_ShouldCreateReservation()
        {
            // Arrange
            var reservation = _fixture.Create<Reservation>();
            var loggedUser = _fixture.Create<LoggedUser>();
            var vehicle = _fixture.Build<LoccarInfra.ORM.model.Vehicle>()
                .With(v => v.Reserved, false)
                .Create();
            var customer = _fixture.Create<LoccarInfra.ORM.model.Customer>();
            var createdReservation = _fixture.Create<LoccarInfra.ORM.model.Reservation>();
            
            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _vehicleRepositoryMock.Setup(x => x.GetVehicleById(reservation.Idvehicle))
                .ReturnsAsync(vehicle);
            _customerRepositoryMock.Setup(x => x.GetCustomerById(reservation.Idcustomer))
                .ReturnsAsync(customer);
            _reservationRepositoryMock.Setup(x => x.CreateReservation(It.IsAny<LoccarInfra.ORM.model.Reservation>()))
                .ReturnsAsync(createdReservation);
            _vehicleRepositoryMock.Setup(x => x.UpdateVehicle(It.IsAny<LoccarInfra.ORM.model.Vehicle>()))
                .ReturnsAsync(vehicle);

            // Act
            var result = await _reservationApplication.CreateReservation(reservation);

            // Assert
            result.Code.Should().Be("201");
            result.Message.Should().Be("Reserva criada com sucesso.");
            result.Data.Should().NotBeNull();
            result.Data.Reservationnumber.Should().Be(createdReservation.Reservationnumber);

            _reservationRepositoryMock.Verify(x => x.CreateReservation(It.IsAny<LoccarInfra.ORM.model.Reservation>()), Times.Once);
            _vehicleRepositoryMock.Verify(x => x.UpdateVehicle(It.Is<LoccarInfra.ORM.model.Vehicle>(v => v.Reserved == true)), Times.Once);
        }

        [Theory]
        [InlineData(1, 100, 100)]
        [InlineData(7, 100, 700)]
        [InlineData(0, 100, 100)] // Minimum 1 day
        public async Task CalculateTotalCost_WithDifferentRentalDays_ShouldCalculateCorrectly(
            int rentalDays, decimal dailyRate, decimal expectedCost)
        {
            // Arrange
            const int reservationId = 1;
            var reservation = _fixture.Build<LoccarInfra.ORM.model.Reservation>()
                .With(r => r.RentalDays, rentalDays)
                .With(r => r.DailyRate, dailyRate)
                .With(r => r.RentalDate, DateTime.Today)
                .With(r => r.ReturnDate, DateTime.Today.AddDays(Math.Max(rentalDays, 1)))
                .Without(r => r.InsuranceVehicle)
                .Without(r => r.InsuranceThirdParty)
                .Without(r => r.TaxAmount)
                .Create();

            var vehicle = _fixture.Build<LoccarInfra.ORM.model.Vehicle>()
                .With(v => v.DailyRate, dailyRate)
                .Create();

            _reservationRepositoryMock.Setup(x => x.GetReservationById(reservationId))
                .ReturnsAsync(reservation);
            _vehicleRepositoryMock.Setup(x => x.GetVehicleById(reservation.Idvehicle))
                .ReturnsAsync(vehicle);

            // Act
            var result = await _reservationApplication.CalculateTotalCost(reservationId);

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().Be(expectedCost);
            result.Message.Should().Be("Custo calculado com sucesso.");
        }

        [Fact]
        public async Task CalculateTotalCost_WithInsurances_ShouldIncludeInTotal()
        {
            // Arrange
            const int reservationId = 1;
            const decimal dailyRate = 100m;
            const decimal vehicleInsurance = 50m;
            const decimal thirdPartyInsurance = 30m;
            const decimal taxAmount = 20m;
            const int rentalDays = 3;

            var reservation = _fixture.Build<LoccarInfra.ORM.model.Reservation>()
                .With(r => r.RentalDays, rentalDays)
                .With(r => r.DailyRate, dailyRate)
                .With(r => r.InsuranceVehicle, vehicleInsurance)
                .With(r => r.InsuranceThirdParty, thirdPartyInsurance)
                .With(r => r.TaxAmount, taxAmount)
                .Create();

            var vehicle = _fixture.Build<LoccarInfra.ORM.model.Vehicle>()
                .With(v => v.DailyRate, dailyRate)
                .Create();

            _reservationRepositoryMock.Setup(x => x.GetReservationById(reservationId))
                .ReturnsAsync(reservation);
            _vehicleRepositoryMock.Setup(x => x.GetVehicleById(reservation.Idvehicle))
                .ReturnsAsync(vehicle);

            var expectedTotal = (rentalDays * dailyRate) + vehicleInsurance + thirdPartyInsurance + taxAmount;

            // Act
            var result = await _reservationApplication.CalculateTotalCost(reservationId);

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().Be(expectedTotal);
        }

        [Theory]
        [InlineData("ADMIN")]
        [InlineData("EMPLOYEE")]
        public async Task RegisterDamage_WhenUserHasPermission_ShouldRegisterDamage(string role)
        {
            // Arrange
            const int reservationNumber = 123;
            const string damageDescription = "Scratched door";
            var loggedUser = new LoggedUser { Roles = new List<string> { role } };

            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _reservationRepositoryMock.Setup(x => x.RegisterDamage(reservationNumber, damageDescription))
                .ReturnsAsync(true);

            // Act
            var result = await _reservationApplication.RegisterDamage(reservationNumber, damageDescription);

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().BeTrue();
            result.Message.Should().Be("Dano registrado com sucesso.");
        }

        [Fact]
        public async Task RegisterDamage_WhenUserDoesNotHavePermission_ShouldReturnUnauthorized()
        {
            // Arrange
            const int reservationNumber = 123;
            const string damageDescription = "Scratched door";
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };

            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Act
            var result = await _reservationApplication.RegisterDamage(reservationNumber, damageDescription);

            // Assert
            result.Code.Should().Be("401");
            result.Data.Should().BeFalse();
            result.Message.Should().Be("Usuário não autorizado.");
        }

        [Theory]
        [InlineData(true, "200", "Reserva cancelada com sucesso.")]
        [InlineData(false, "404", "Reserva não encontrada.")]
        public async Task CancelReservation_WithDifferentScenarios_ShouldReturnExpectedResult(
            bool repositoryResult, string expectedCode, string expectedMessage)
        {
            // Arrange
            const int reservationNumber = 123;
            var loggedUser = _fixture.Create<LoggedUser>();

            _authApplicationMock.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _reservationRepositoryMock.Setup(x => x.CancelReservation(reservationNumber))
                .ReturnsAsync(repositoryResult);

            // Act
            var result = await _reservationApplication.CancelReservation(reservationNumber);

            // Assert
            result.Code.Should().Be(expectedCode);
            result.Data.Should().Be(repositoryResult);
            result.Message.Should().Be(expectedMessage);
        }
    }
}