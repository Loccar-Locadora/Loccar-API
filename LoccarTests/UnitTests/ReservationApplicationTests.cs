using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using LoccarApplication;
using LoccarApplication.Interfaces;
using LoccarDomain;
using LoccarDomain.LoggedUser.Models;
using LoccarDomain.Reservation.Models;
using LoccarInfra.Repositories.Interfaces;
using Moq;
using Xunit;

namespace LoccarTests.UnitTests
{
    public class ReservationApplicationTests
    {
        private readonly Mock<IReservationRepository> _mockReservationRepository;
        private readonly Mock<IVehicleRepository> _mockVehicleRepository;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<IAuthApplication> _mockAuthApplication;
        private readonly ReservationApplication _reservationApplication;
        private readonly Fixture _fixture;

        public ReservationApplicationTests()
        {
            _mockReservationRepository = new Mock<IReservationRepository>();
            _mockVehicleRepository = new Mock<IVehicleRepository>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockAuthApplication = new Mock<IAuthApplication>();
            _reservationApplication = new ReservationApplication(
                _mockReservationRepository.Object,
                _mockVehicleRepository.Object,
                _mockCustomerRepository.Object,
                _mockAuthApplication.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task CreateReservationWhenUserNotAuthenticatedReturnsUnauthorized()
        {
            // Arrange
            var reservation = new Reservation
            {
                Idcustomer = 1,
                Idvehicle = 1,
                RentalDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(5),
            };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns((LoggedUser)null);

            // Act
            var result = await _reservationApplication.CreateReservation(reservation);

            // Assert
            result.Code.Should().Be("401");
            result.Message.Should().Be("User not authorized.");
        }

        [Fact]
        public async Task CreateReservationWhenVehicleNotFoundReturnsNotFound()
        {
            // Arrange
            var reservation = new Reservation
            {
                Idcustomer = 1,
                Idvehicle = 1,
                RentalDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(5),
            };
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.GetVehicleById(reservation.Idvehicle))
                .ReturnsAsync((LoccarInfra.ORM.model.Vehicle)null);

            // Act
            var result = await _reservationApplication.CreateReservation(reservation);

            // Assert
            result.Code.Should().Be("404");
            result.Message.Should().Be("Vehicle not found.");
        }

        [Fact]
        public async Task CreateReservationWhenVehicleNotAvailableReturnsBadRequest()
        {
            // Arrange
            var reservation = new Reservation
            {
                Idcustomer = 1,
                Idvehicle = 1,
                RentalDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(5),
            };
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            var vehicle = new LoccarInfra.ORM.model.Vehicle { Reserved = true };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.GetVehicleById(reservation.Idvehicle))
                .ReturnsAsync(vehicle);

            // Act
            var result = await _reservationApplication.CreateReservation(reservation);

            // Assert
            result.Code.Should().Be("400");
            result.Message.Should().Be("Vehicle is not available.");
        }

        [Fact]
        public async Task CreateReservationWhenCustomerNotFoundReturnsNotFound()
        {
            // Arrange
            var reservation = new Reservation
            {
                Idcustomer = 1,
                Idvehicle = 1,
                RentalDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(5),
            };
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            var vehicle = new LoccarInfra.ORM.model.Vehicle { Reserved = false };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.GetVehicleById(reservation.Idvehicle))
                .ReturnsAsync(vehicle);
            _mockCustomerRepository.Setup(x => x.GetCustomerById(reservation.Idcustomer))
                .ReturnsAsync((LoccarInfra.ORM.model.Customer)null);

            // Act
            var result = await _reservationApplication.CreateReservation(reservation);

            // Assert
            result.Code.Should().Be("404");
            result.Message.Should().Be("Customer not found.");
        }

        [Fact]
        public async Task CreateReservationWhenValidDataCreatesReservationSuccessfully()
        {
            // Arrange
            var reservation = new Reservation
            {
                Idcustomer = 1,
                Idvehicle = 1,
                RentalDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(5),
                RentalDays = 5,
                DailyRate = 100.0m,
            };
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            var vehicle = new LoccarInfra.ORM.model.Vehicle { Reserved = false };
            var customer = new LoccarInfra.ORM.model.Customer { Idcustomer = reservation.Idcustomer };
            var tbReservation = new LoccarInfra.ORM.model.Reservation
            {
                Reservationnumber = 123456,
                Idcustomer = reservation.Idcustomer,
                Idvehicle = reservation.Idvehicle,
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.GetVehicleById(reservation.Idvehicle))
                .ReturnsAsync(vehicle);
            _mockCustomerRepository.Setup(x => x.GetCustomerById(reservation.Idcustomer))
                .ReturnsAsync(customer);
            _mockReservationRepository.Setup(x => x.CreateReservation(It.IsAny<LoccarInfra.ORM.model.Reservation>()))
                .ReturnsAsync(tbReservation);
            _mockVehicleRepository.Setup(x => x.UpdateVehicle(It.IsAny<LoccarInfra.ORM.model.Vehicle>()))
                .ReturnsAsync(vehicle);

            // Act
            var result = await _reservationApplication.CreateReservation(reservation);

            // Assert
            result.Code.Should().Be("201");
            result.Message.Should().Be("Reservation created successfully.");
            result.Data.Should().NotBeNull();
            result.Data.Reservationnumber.Should().Be(123456);
        }

        [Theory]
        [InlineData(100.0, 5, 500.0)]
        [InlineData(80.0, 3, 240.0)]
        [InlineData(150.0, 1, 150.0)]
        public async Task CalculateTotalCostWithDifferentRatesCalculatesCorrectly(
            decimal dailyRate, int rentalDays, decimal expectedTotal)
        {
            // Arrange
            var tbReservation = new LoccarInfra.ORM.model.Reservation
            {
                Idvehicle = 1,
                RentalDays = rentalDays,
                DailyRate = dailyRate,
                InsuranceVehicle = null,
                InsuranceThirdParty = null,
                TaxAmount = null,
            };
            var vehicle = new LoccarInfra.ORM.model.Vehicle { DailyRate = dailyRate };

            _mockReservationRepository.Setup(x => x.GetReservationById(1))
                .ReturnsAsync(tbReservation);
            _mockVehicleRepository.Setup(x => x.GetVehicleById(1))
                .ReturnsAsync(vehicle);

            // Act
            var result = await _reservationApplication.CalculateTotalCost(1);

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().Be(expectedTotal);
        }

        [Fact]
        public async Task CalculateTotalCostWhenReservationNotFoundReturnsNotFound()
        {
            // Arrange
            _mockReservationRepository.Setup(x => x.GetReservationById(1))
                .ReturnsAsync((LoccarInfra.ORM.model.Reservation)null);

            // Act
            var result = await _reservationApplication.CalculateTotalCost(1);

            // Assert
            result.Code.Should().Be("404");
            result.Message.Should().Be("Reservation not found.");
        }

        [Theory]
        [InlineData("ADMIN", true)]
        [InlineData("EMPLOYEE", true)]
        [InlineData("COMMON_USER", false)]
        public async Task RegisterDamageWithDifferentRolesReturnsExpectedResult(
            string role, bool shouldSucceed)
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { role } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            if (shouldSucceed)
            {
                _mockReservationRepository.Setup(x => x.RegisterDamage(It.IsAny<int>(), It.IsAny<string>()))
                    .ReturnsAsync(true);
            }

            // Act
            var result = await _reservationApplication.RegisterDamage(123456, "Damage on bumper");

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
        public async Task CancelReservationWhenUserAuthenticatedCancelsSuccessfully()
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockReservationRepository.Setup(x => x.CancelReservation(123456))
                .ReturnsAsync(true);

            // Act
            var result = await _reservationApplication.CancelReservation(123456);

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().BeTrue();
            result.Message.Should().Be("Reservation cancelled successfully.");
        }

        [Fact]
        public async Task GetReservationHistoryWhenCustomerHasReservationsReturnsHistory()
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            var tbReservations = new List<LoccarInfra.ORM.model.Reservation>
            {
                new LoccarInfra.ORM.model.Reservation
                {
                    Reservationnumber = 123456,
                    Idcustomer = 1,
                    Idvehicle = 1,
                    RentalDate = DateTime.Now.AddDays(-10),
                    ReturnDate = DateTime.Now.AddDays(-5),
                },
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockReservationRepository.Setup(x => x.GetReservationHistory(1))
                .ReturnsAsync(tbReservations);

            // Act
            var result = await _reservationApplication.GetReservationHistory(1);

            // Assert
            result.Code.Should().Be("200");
            result.Message.Should().Be("Reservation history obtained successfully.");
            result.Data.Should().HaveCount(1);
            result.Data.First().Reservationnumber.Should().Be(123456);
        }

        [Fact]
        public async Task UpdateReservationWhenAuthorizedUserUpdatesSuccessfully()
        {
            // Arrange
            var reservation = new Reservation
            {
                Reservationnumber = 123456,
                Idcustomer = 1,
                Idvehicle = 1,
                RentalDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(5),
                RentalDays = 5,
                DailyRate = 100.0m,
            };
            var loggedUser = new LoggedUser { Roles = new List<string> { "ADMIN" } };
            var updatedTbReservation = new LoccarInfra.ORM.model.Reservation
            {
                Reservationnumber = reservation.Reservationnumber,
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockReservationRepository.Setup(x => x.UpdateReservation(It.IsAny<LoccarInfra.ORM.model.Reservation>()))
                .ReturnsAsync(updatedTbReservation);

            // Act
            var result = await _reservationApplication.UpdateReservation(reservation);

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().NotBeNull();
            result.Message.Should().Be("Reservation updated successfully.");
        }

        [Fact]
        public async Task UpdateReservationWhenUnauthorizedUserReturnsUnauthorized()
        {
            // Arrange
            var reservation = new Reservation
            {
                Reservationnumber = 123456,
                Idcustomer = 1,
                Idvehicle = 1,
                RentalDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(5),
            };
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Act
            var result = await _reservationApplication.UpdateReservation(reservation);

            // Assert
            result.Code.Should().Be("401");
            result.Message.Should().Be("User not authorized.");
        }
    }
}
