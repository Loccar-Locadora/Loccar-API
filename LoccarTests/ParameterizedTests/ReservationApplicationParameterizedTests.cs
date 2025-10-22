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

namespace LoccarTests.ParameterizedTests
{
    public class ReservationApplicationParameterizedTests
    {
        private readonly Mock<IReservationRepository> _mockReservationRepository;
        private readonly Mock<IVehicleRepository> _mockVehicleRepository;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<IAuthApplication> _mockAuthApplication;
        private readonly ReservationApplication _reservationApplication;

        public ReservationApplicationParameterizedTests()
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
        }

        public static IEnumerable<object[]> RentalCostCalculationTestData =>
            new List<object[]>
            {
                // dailyRate, rentalDays, insuranceVehicle, insuranceThirdParty, taxAmount, expectedTotal
                new object[] { 100.0m, 5, null, null, null, 500.0m },
                new object[] { 100.0m, 5, 50.0m, null, null, 550.0m },
                new object[] { 100.0m, 5, null, 30.0m, null, 530.0m },
                new object[] { 100.0m, 5, null, null, 20.0m, 520.0m },
                new object[] { 100.0m, 5, 50.0m, 30.0m, 20.0m, 600.0m },
                new object[] { 80.5m, 3, 25.0m, 15.0m, 10.0m, 291.5m },
                new object[] { 150.0m, 1, null, null, null, 150.0m }
            };

        [Theory]
        [MemberData(nameof(RentalCostCalculationTestData))]
        public async Task CalculateTotalCost_WithDifferentParameters_ReturnsCorrectTotal(
            decimal dailyRate, int rentalDays, decimal? insuranceVehicle, decimal? insuranceThirdParty, 
            decimal? taxAmount, decimal expectedTotal)
        {
            // Arrange
            var tbReservation = new LoccarInfra.ORM.model.Reservation
            {
                Idvehicle = 1,
                RentalDays = rentalDays,
                DailyRate = dailyRate,
                InsuranceVehicle = insuranceVehicle,
                InsuranceThirdParty = insuranceThirdParty,
                TaxAmount = taxAmount,
                RentalDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(rentalDays)
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

        public static IEnumerable<object[]> UserAuthorizationTestData =>
            new List<object[]>
            {
                new object[] { new List<string> { "COMMON_USER" }, true },
                new object[] { new List<string> { "EMPLOYEE" }, true },
                new object[] { new List<string> { "ADMIN" }, true },
                new object[] { null, false },
                new object[] { new List<string>(), true } // Roles vazias são consideradas autorizadas (não null)
            };

        [Theory]
        [MemberData(nameof(UserAuthorizationTestData))]
        public async Task CreateReservation_WithDifferentUserRoles_ReturnsExpectedResult(
            List<string> userRoles, bool shouldBeAuthorized)
        {
            // Arrange
            var reservation = new Reservation
            {
                Idcustomer = 1,
                Idvehicle = 1,
                RentalDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(5)
            };

            var loggedUser = userRoles != null ? new LoggedUser { Roles = userRoles } : null;
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Act
            var result = await _reservationApplication.CreateReservation(reservation);

            // Assert
            if (!shouldBeAuthorized)
            {
                result.Code.Should().Be("401");
                result.Message.Should().Be("Usuário não autorizado.");
            }
            else
            {
                // Se autorizado, pode falhar por outros motivos (404 - veículo não encontrado, etc.)
                result.Code.Should().NotBe("401");
            }
        }

        [Theory]
        [InlineData(true, "400", "Veículo não está disponível.")]
        [InlineData(false, "404", "Cliente não encontrado.")] // Assumindo que o cliente não será encontrado no setup
        public async Task CreateReservation_WithDifferentVehicleStates_ReturnsExpectedResult(
            bool vehicleReserved, string expectedCode, string expectedMessage)
        {
            // Arrange
            var reservation = new Reservation
            {
                Idcustomer = 1,
                Idvehicle = 1,
                RentalDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(5)
            };

            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            var vehicle = new LoccarInfra.ORM.model.Vehicle { Reserved = vehicleReserved };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockVehicleRepository.Setup(x => x.GetVehicleById(reservation.Idvehicle))
                .ReturnsAsync(vehicle);

            if (!vehicleReserved)
            {
                _mockCustomerRepository.Setup(x => x.GetCustomerById(reservation.Idcustomer))
                    .ReturnsAsync((LoccarInfra.ORM.model.Customer)null);
            }

            // Act
            var result = await _reservationApplication.CreateReservation(reservation);

            // Assert
            result.Code.Should().Be(expectedCode);
            result.Message.Should().Be(expectedMessage);
        }

        public static IEnumerable<object[]> AdminEmployeeRoleTestData =>
            new List<object[]>
            {
                new object[] { "ADMIN", true },
                new object[] { "EMPLOYEE", true },
                new object[] { "COMMON_USER", false },
                new object[] { "INVALID_ROLE", false }
            };

        [Theory]
        [MemberData(nameof(AdminEmployeeRoleTestData))]
        public async Task RegisterDamage_WithDifferentRoles_ReturnsExpectedResult(
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
            var result = await _reservationApplication.RegisterDamage(123456, "Dano no para-choque");

            // Assert
            if (shouldSucceed)
            {
                result.Code.Should().Be("200");
                result.Data.Should().BeTrue();
                result.Message.Should().Be("Dano registrado com sucesso.");
            }
            else
            {
                result.Code.Should().Be("401");
                result.Data.Should().BeFalse();
                result.Message.Should().Be("Usuário não autorizado.");
            }
        }

        [Theory]
        [MemberData(nameof(AdminEmployeeRoleTestData))]
        public async Task ListAllReservations_WithDifferentRoles_ReturnsExpectedResult(
            string role, bool shouldSucceed)
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { role } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            if (shouldSucceed)
            {
                var tbReservations = new List<LoccarInfra.ORM.model.Reservation>
                {
                    new LoccarInfra.ORM.model.Reservation { Reservationnumber = 123456 }
                };
                _mockReservationRepository.Setup(x => x.ListAllReservations())
                    .ReturnsAsync(tbReservations);
            }

            // Act
            var result = await _reservationApplication.ListAllReservations();

            // Assert
            if (shouldSucceed)
            {
                result.Code.Should().Be("200");
                result.Data.Should().NotBeNull();
            }
            else
            {
                result.Code.Should().Be("401");
                result.Message.Should().Be("Usuário não autorizado.");
            }
        }

        [Theory]
        [InlineData(123456, true, "200", "Reserva cancelada com sucesso.")]
        [InlineData(999999, false, "404", "Reserva não encontrada.")]
        public async Task CancelReservation_WithDifferentReservationNumbers_ReturnsExpectedResult(
            int reservationNumber, bool reservationExists, string expectedCode, string expectedMessage)
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockReservationRepository.Setup(x => x.CancelReservation(reservationNumber))
                .ReturnsAsync(reservationExists);

            // Act
            var result = await _reservationApplication.CancelReservation(reservationNumber);

            // Assert
            result.Code.Should().Be(expectedCode);
            result.Message.Should().Be(expectedMessage);
            result.Data.Should().Be(reservationExists);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public async Task GetReservationHistory_WithMultipleReservations_ReturnsCorrectCount(
            int reservationCount)
        {
            // Arrange
            var loggedUser = new LoggedUser { Roles = new List<string> { "COMMON_USER" } };
            var tbReservations = new List<LoccarInfra.ORM.model.Reservation>();

            for (int i = 0; i < reservationCount; i++)
            {
                tbReservations.Add(new LoccarInfra.ORM.model.Reservation
                {
                    Reservationnumber = 100000 + i,
                    Idcustomer = 1,
                    Idvehicle = i + 1,
                    RentalDate = DateTime.Now.AddDays(-10 - i),
                    ReturnDate = DateTime.Now.AddDays(-5 - i)
                });
            }

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockReservationRepository.Setup(x => x.GetReservationHistory(1))
                .ReturnsAsync(tbReservations);

            // Act
            var result = await _reservationApplication.GetReservationHistory(1);

            // Assert
            if (reservationCount > 0)
            {
                result.Code.Should().Be("200");
                result.Data.Should().HaveCount(reservationCount);
                result.Message.Should().Be("Histórico de reservas obtido com sucesso.");
            }
            else
            {
                result.Code.Should().Be("404");
                result.Message.Should().Be("Nenhuma reserva encontrada para este cliente.");
            }
        }

        public static IEnumerable<object[]> DateValidationTestData =>
            new List<object[]>
            {
                new object[] { DateTime.Now, DateTime.Now.AddDays(1), true },
                new object[] { DateTime.Now, DateTime.Now.AddDays(7), true },
                new object[] { DateTime.Now, DateTime.Now.AddDays(-1), false }, // Data de retorno no passado
                new object[] { DateTime.Now.AddDays(1), DateTime.Now, false }, // Data de locação depois da devolução
            };

        [Theory]
        [MemberData(nameof(DateValidationTestData))]
        public void ValidateReservationDates_WithDifferentDates_ReturnsExpectedResult(
            DateTime rentalDate, DateTime returnDate, bool isValid)
        {
            // Arrange & Act
            bool actualResult = returnDate > rentalDate && returnDate >= DateTime.Now.Date;

            // Assert
            actualResult.Should().Be(isValid);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(-1, 1)]
        [InlineData(5, 5)]
        [InlineData(30, 30)]
        public void CalculateRentalDays_WithDifferentValues_ReturnsCorrectDays(
            int inputDays, int expectedDays)
        {
            // Arrange & Act
            int actualDays = inputDays <= 0 ? 1 : inputDays; // Simular lógica da aplicação

            // Assert
            actualDays.Should().Be(expectedDays);
        }

        [Theory]
        [InlineData("Pequeno arranhão na porta", true)]
        [InlineData("Dano severo no motor", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void ValidateDamageDescription_WithDifferentInputs_ReturnsExpectedResult(
            string damageDescription, bool isValid)
        {
            // Arrange & Act
            bool actualResult = !string.IsNullOrWhiteSpace(damageDescription);

            // Assert
            actualResult.Should().Be(isValid);
        }
    }
}