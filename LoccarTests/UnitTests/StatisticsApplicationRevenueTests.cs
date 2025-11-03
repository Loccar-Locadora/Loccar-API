using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LoccarApplication;
using LoccarApplication.Interfaces;
using LoccarDomain.LoggedUser.Models;
using LoccarDomain.Statistics.Models;
using LoccarInfra.ORM.model;
using LoccarInfra.Repositories.Interfaces;
using Moq;
using Xunit;

namespace LoccarTests.UnitTests
{
    public class StatisticsApplicationRevenueTests
    {
        private readonly Mock<IAuthApplication> _mockAuthApplication;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<IVehicleRepository> _mockVehicleRepository;
        private readonly Mock<IReservationRepository> _mockReservationRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly StatisticsApplication _statisticsApplication;

        public StatisticsApplicationRevenueTests()
        {
            _mockAuthApplication = new Mock<IAuthApplication>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockVehicleRepository = new Mock<IVehicleRepository>();
            _mockReservationRepository = new Mock<IReservationRepository>();
            _mockUserRepository = new Mock<IUserRepository>();

            _statisticsApplication = new StatisticsApplication(
                _mockAuthApplication.Object,
                _mockCustomerRepository.Object,
                _mockVehicleRepository.Object,
                _mockReservationRepository.Object,
                _mockUserRepository.Object);
        }

        [Fact]
        public async Task GetMonthlyRevenueWhenAuthorizedUserReturnsRevenue()
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };
            
            var mockReservations = new List<Reservation>
            {
                new Reservation 
                { 
                    RentalDate = new DateTime(2024, 1, 1),
                    ReturnDate = new DateTime(2024, 1, 5),
                    RentalDays = 4,
                    DailyRate = 100m,
                    InsuranceVehicle = 50m,
                    TaxAmount = 20m
                },
                new Reservation
                {
                    RentalDate = new DateTime(2024, 1, 10),
                    ReturnDate = new DateTime(2024, 1, 13),
                    RentalDays = 3,
                    DailyRate = 150m,
                    InsuranceThirdParty = 30m
                }
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockReservationRepository.Setup(x => x.GetReservationsByMonth(2024, 1))
                .ReturnsAsync(mockReservations);
            _mockReservationRepository.Setup(x => x.GetMonthlyRevenue(2024, 1))
                .ReturnsAsync(920m); // (4*100 + 50 + 20) + (3*150 + 30) = 470 + 480 = 950

            // Act
            var result = await _statisticsApplication.GetMonthlyRevenue(2024, 1);

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().NotBeNull();
            result.Data.Year.Should().Be(2024);
            result.Data.Month.Should().Be(1);
            result.Data.MonthName.Should().Be("January");
            result.Data.TotalRevenue.Should().Be(920m);
            result.Data.TotalReservations.Should().Be(2);
            result.Data.AverageRevenuePerReservation.Should().Be(460m);
        }

        [Fact]
        public async Task GetMonthlyRevenueWhenUnauthorizedUserReturnsUnauthorized()
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = new List<string> { "CLIENT_USER" },
                Authenticated = true
            };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Act
            var result = await _statisticsApplication.GetMonthlyRevenue(2024, 1);

            // Assert
            result.Code.Should().Be("401");
            result.Message.Should().Be("User not authorized.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(13)]
        [InlineData(-1)]
        public async Task GetMonthlyRevenueWhenInvalidMonthReturnsBadRequest(int month)
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };
            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Act
            var result = await _statisticsApplication.GetMonthlyRevenue(2024, month);

            // Assert
            result.Code.Should().Be("400");
            result.Message.Should().Be("Invalid month. Must be between 1 and 12.");
        }

        [Fact]
        public async Task GetMonthlyRevenueDetailedWhenAuthorizedUserReturnsDetailedBreakdown()
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = new List<string> { "CLIENT_EMPLOYEE" },
                Authenticated = true
            };

            var mockReservations = new List<Reservation>
            {
                new Reservation 
                { 
                    RentalDate = new DateTime(2024, 1, 1),
                    ReturnDate = new DateTime(2024, 1, 5),
                    RentalDays = 4,
                    DailyRate = 100m,
                    InsuranceVehicle = 50m,
                    InsuranceThirdParty = 25m,
                    TaxAmount = 20m,
                    IdVehicleNavigation = new Vehicle { DailyRate = 100m }
                }
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockReservationRepository.Setup(x => x.GetReservationsByMonth(2024, 1))
                .ReturnsAsync(mockReservations);

            // Act
            var result = await _statisticsApplication.GetMonthlyRevenueDetailed(2024, 1);

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().NotBeNull();
            result.Data.Revenue.Should().NotBeNull();
            result.Data.Revenue.BaseRevenue.Should().Be(400m); // 4 * 100
            result.Data.Revenue.InsuranceRevenue.Should().Be(75m); // 50 + 25
            result.Data.Revenue.TaxRevenue.Should().Be(20m);
            result.Data.Revenue.TotalRevenue.Should().Be(495m); // 400 + 75 + 20
        }

        [Fact]
        public async Task GetCurrentMonthRevenueWhenAuthorizedUserReturnsCurrentMonth()
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };

            var now = DateTime.Now;
            var mockReservations = new List<Reservation>();

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockReservationRepository.Setup(x => x.GetReservationsByMonth(now.Year, now.Month))
                .ReturnsAsync(mockReservations);
            _mockReservationRepository.Setup(x => x.GetMonthlyRevenue(now.Year, now.Month))
                .ReturnsAsync(0m);

            // Act
            var result = await _statisticsApplication.GetCurrentMonthRevenue();

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().NotBeNull();
            result.Data.Year.Should().Be(now.Year);
            result.Data.Month.Should().Be(now.Month);
        }

        [Fact]
        public async Task GetYearRevenueWhenAuthorizedUserReturnsYearlyTotal()
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockReservationRepository.Setup(x => x.GetYearRevenue(2024))
                .ReturnsAsync(50000m);

            // Act
            var result = await _statisticsApplication.GetYearRevenue(2024);

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().Be(50000m);
            result.Message.Should().Be("Year 2024 revenue retrieved successfully.");
        }

        [Fact]
        public async Task GetYearlyRevenueBreakdownWhenAuthorizedUserReturnsMonthlyBreakdown()
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);

            // Setup mocks para cada mês
            for (int month = 1; month <= 12; month++)
            {
                _mockReservationRepository.Setup(x => x.GetReservationsByMonth(2024, month))
                    .ReturnsAsync(new List<Reservation>());
                _mockReservationRepository.Setup(x => x.GetMonthlyRevenue(2024, month))
                    .ReturnsAsync(1000m * month); // Receita crescente por mês
            }

            // Act
            var result = await _statisticsApplication.GetYearlyRevenueBreakdown(2024);

            // Assert
            result.Code.Should().Be("200");
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(12);
            result.Data[0].Month.Should().Be(1);
            result.Data[0].TotalRevenue.Should().Be(1000m);
            result.Data[11].Month.Should().Be(12);
            result.Data[11].TotalRevenue.Should().Be(12000m);
        }

        [Fact]
        public async Task GetMonthlyRevenueWhenExceptionOccursReturnsError()
        {
            // Arrange
            var loggedUser = new LoggedUser 
            { 
                Roles = new List<string> { "CLIENT_ADMIN" },
                Authenticated = true
            };

            _mockAuthApplication.Setup(x => x.GetLoggedUser()).Returns(loggedUser);
            _mockReservationRepository.Setup(x => x.GetReservationsByMonth(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act
            var result = await _statisticsApplication.GetMonthlyRevenue(2024, 1);

            // Assert
            result.Code.Should().Be("500");
            result.Message.Should().StartWith("An unexpected error occurred:");
        }
    }
}
