using NBomber.Contracts;
using NBomber.CSharp;
using Xunit;
using Xunit.Abstractions;

namespace LoccarTests.PerformanceTests
{
    public class ApiPerformanceTests
    {
        private readonly ITestOutputHelper _output;

        public ApiPerformanceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(Skip = "Performance test - run manually")]
        public void CustomerRegistration_LoadTest()
        {
            var scenario = Scenario.Create("customer_registration", async context =>
            {
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://localhost:7087"); // Ajustar para sua URL

                var customerData = $$"""
                {
                    "username": "LoadTestUser{{context.ScenarioInfo.ThreadId}}",
                    "email": "loadtest{{context.ScenarioInfo.ThreadId}}@example.com",
                    "cellphone": "11999999999",
                    "driverLicense": "{{Random.Shared.Next(10000000000, 99999999999)}}"
                }
                """;

                using var content = new StringContent(customerData, System.Text.Encoding.UTF8, "application/json");
                
                try
                {
                    var response = await httpClient.PostAsync("/api/Customer/register", content);
                    
                    return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
                }
                catch
                {
                    return Response.Fail();
                }
            })
            .WithLoadSimulations(
                Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromMinutes(1))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            _output.WriteLine($"Total requests: {stats.AllRequestCount}");
            _output.WriteLine($"OK responses: {stats.AllOkCount}");
            _output.WriteLine($"Failed responses: {stats.AllFailCount}");
        }

        [Fact(Skip = "Performance test - run manually")]
        public void VehiclesList_StressTest()
        {
            var scenario = Scenario.Create("list_vehicles", async context =>
            {
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://localhost:7087");
                
                // Adicionar token de autenticação se necessário
                // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your-token");

                try
                {
                    var response = await httpClient.GetAsync("/api/vehicle/list/available");
                    return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
                }
                catch
                {
                    return Response.Fail();
                }
            })
            .WithLoadSimulations(
                Simulation.KeepConstant(copies: 50, during: TimeSpan.FromMinutes(2))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            _output.WriteLine($"Average response time: {stats.ScenarioStats[0].Ok.Response.Mean}ms");
            _output.WriteLine($"95th percentile: {stats.ScenarioStats[0].Ok.Response.Percentile95}ms");
        }

        [Fact(Skip = "Performance test - run manually")]
        public void MixedWorkload_EnduranceTest()
        {
            var customerScenario = Scenario.Create("customers", async context =>
            {
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://localhost:7087");

                var endpoints = new[]
                {
                    "/api/Customer/list/all",
                    "/api/Customer/1",
                    "/api/Customer/2"
                };

                var endpoint = endpoints[Random.Shared.Next(endpoints.Length)];
                
                try
                {
                    var response = await httpClient.GetAsync(endpoint);
                    return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
                }
                catch
                {
                    return Response.Fail();
                }
            })
            .WithWeight(60) // 60% das requisições
            .WithLoadSimulations(
                Simulation.InjectPerSec(rate: 5, during: TimeSpan.FromMinutes(5))
            );

            var vehicleScenario = Scenario.Create("vehicles", async context =>
            {
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://localhost:7087");

                var endpoints = new[]
                {
                    "/api/vehicle/list/available",
                    "/api/vehicle/1",
                    "/api/vehicle/2"
                };

                var endpoint = endpoints[Random.Shared.Next(endpoints.Length)];
                
                try
                {
                    var response = await httpClient.GetAsync(endpoint);
                    return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
                }
                catch
                {
                    return Response.Fail();
                }
            })
            .WithWeight(40) // 40% das requisições
            .WithLoadSimulations(
                Simulation.InjectPerSec(rate: 3, during: TimeSpan.FromMinutes(5))
            );

            var stats = NBomberRunner
                .RegisterScenarios(customerScenario, vehicleScenario)
                .Run();

            foreach (var scenarioStats in stats.ScenarioStats)
            {
                _output.WriteLine($"Scenario: {scenarioStats.ScenarioName}");
                _output.WriteLine($"  Requests: {scenarioStats.Ok.Request.Count}");
                _output.WriteLine($"  Avg Response Time: {scenarioStats.Ok.Response.Mean}ms");
                _output.WriteLine($"  Error Rate: {scenarioStats.Fail.Request.Count / (double)scenarioStats.AllRequestCount * 100:F2}%");
            }
        }
    }
}