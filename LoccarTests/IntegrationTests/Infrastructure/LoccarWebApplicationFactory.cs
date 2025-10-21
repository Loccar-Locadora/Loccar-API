using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LoccarInfra.ORM.model;

namespace LoccarTests.IntegrationTests.Infrastructure
{
    public class LoccarWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove o serviço de banco de dados existente
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DataBaseContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Adiciona banco de dados em memória para testes
                services.AddDbContext<DataBaseContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Garante que o banco de dados seja criado
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<DataBaseContext>();
                var logger = scopedServices.GetRequiredService<ILogger<LoccarWebApplicationFactory<TStartup>>>();

                db.Database.EnsureCreated();

                try
                {
                    // Seed do banco de dados para testes
                    SeedDatabase(db);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erro ao fazer seed do banco de dados durante os testes. Erro: {Message}", ex.Message);
                }
            });

            builder.UseEnvironment("Testing");
        }

        private static void SeedDatabase(DataBaseContext context)
        {
            // Limpa os dados existentes
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Seed de dados para testes
            var customers = new[]
            {
                new Customer { Name = "Test Customer 1", Email = "test1@example.com", Phone = "11999999999", DriverLicense = "12345678901", Created = DateTime.Now },
                new Customer { Name = "Test Customer 2", Email = "test2@example.com", Phone = "11888888888", DriverLicense = "12345678902", Created = DateTime.Now }
            };

            var vehicles = new[]
            {
                new Vehicle { Brand = "Toyota", Model = "Corolla", ManufacturingYear = 2020, ModelYear = 2021, Vin = "1HGBH41JXMN109186", DailyRate = 100m, Reserved = false },
                new Vehicle { Brand = "Honda", Model = "Civic", ManufacturingYear = 2019, ModelYear = 2020, Vin = "1HGBH41JXMN109187", DailyRate = 120m, Reserved = true }
            };

            context.Customers.AddRange(customers);
            context.Vehicles.AddRange(vehicles);
            context.SaveChanges();
        }
    }
}