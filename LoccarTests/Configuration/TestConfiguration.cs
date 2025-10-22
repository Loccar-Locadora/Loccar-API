using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LoccarTests.Configuration
{
    public static class TestConfiguration
    {
        public static IConfiguration Configuration { get; private set; }

        static TestConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Jwt:Key", "test-key-for-loccar-integration-tests-must-be-long-enough"},
                    {"Jwt:Issuer", "TestIssuer"},
                    {"Jwt:Audience", "TestAudience"},
                    {"ConnectionStrings:DefaultConnection", "InMemoryTestDatabase"}
                });

            Configuration = builder.Build();
        }

        public static IServiceCollection AddTestServices(this IServiceCollection services)
        {
            services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
            return services;
        }
    }

    public static class TestConstants
    {
        public const string AdminUserId = "1";
        public const string EmployeeUserId = "2";
        public const string CommonUserId = "3";
        
        public const string AdminEmail = "admin@test.com";
        public const string EmployeeEmail = "employee@test.com";
        public const string CommonUserEmail = "user@test.com";

        public const string AdminRole = "ADMIN";
        public const string EmployeeRole = "EMPLOYEE";
        public const string CommonUserRole = "COMMON_USER";

        public static class TestData
        {
            public static class Vehicles
            {
                public const string DefaultBrand = "Toyota";
                public const string DefaultModel = "Corolla";
                public const int DefaultYear = 2023;
                public const decimal DefaultDailyRate = 100.0m;
                public const string DefaultVin = "1HGBH41JXMN109186";
            }

            public static class Customers
            {
                public const string DefaultName = "Test Customer";
                public const string DefaultEmail = "test@email.com";
                public const string DefaultPhone = "11999999999";
                public const string DefaultDriverLicense = "12345678901";
            }

            public static class Reservations
            {
                public const int DefaultReservationNumber = 123456;
                public const int DefaultRentalDays = 5;
                public const decimal DefaultInsuranceVehicle = 50.0m;
                public const decimal DefaultInsuranceThirdParty = 30.0m;
                public const decimal DefaultTaxAmount = 20.0m;
            }
        }
    }
}