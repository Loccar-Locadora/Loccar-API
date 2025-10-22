using Xunit;
using Xunit.Abstractions;

namespace LoccarTests.TestSuites
{
    /// <summary>
    /// Suite de testes que executa uma amostra de cada tipo de teste implementado
    /// para demonstrar a funcionalidade completa do sistema de testes.
    /// </summary>
    public class ComprehensiveTestSuite
    {
        private readonly ITestOutputHelper _output;

        public ComprehensiveTestSuite(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        [Trait("Category", "Comprehensive")]
        public async Task RunAllTestTypes()
        {
            _output.WriteLine("=== EXECUTANDO SUITE COMPLETA DE TESTES ===");
            
            await RunUnitTestSample();
            await RunParameterizedTestSample();
            await RunIntegrationTestSample();
            
            _output.WriteLine("=== SUITE COMPLETA EXECUTADA COM SUCESSO ===");
        }

        private async Task RunUnitTestSample()
        {
            _output.WriteLine("\n--- Executando Testes Unitários ---");
            
            var unitTests = new UnitTests.VehicleApplicationTests();
            
            // Teste de autorização
            await unitTests.RegisterVehicle_WhenUserIsNotAuthorized_ReturnsUnauthorized();
            _output.WriteLine("? Teste unitário de autorização executado");
            
            // Teste de exceção
            await unitTests.RegisterVehicle_WhenExceptionOccurs_ReturnsServerError();
            _output.WriteLine("? Teste unitário de tratamento de exceção executado");
            
            _output.WriteLine("--- Testes Unitários Concluídos ---\n");
        }

        private async Task RunParameterizedTestSample()
        {
            _output.WriteLine("--- Executando Testes Parametrizados ---");
            
            var parameterizedTests = new ParameterizedTests.VehicleApplicationParameterizedTests();
            
            // Teste com diferentes roles de usuário
            await parameterizedTests.RegisterVehicle_WithDifferentUserRoles_ReturnsExpectedResult(
                new List<string> { "ADMIN" }, true, "201", "Veículo cadastrado com sucesso");
            _output.WriteLine("? Teste parametrizado com role ADMIN executado");
            
            await parameterizedTests.RegisterVehicle_WithDifferentUserRoles_ReturnsExpectedResult(
                new List<string> { "COMMON_USER" }, false, "401", "Usuário não autorizado.");
            _output.WriteLine("? Teste parametrizado com role COMMON_USER executado");
            
            _output.WriteLine("--- Testes Parametrizados Concluídos ---\n");
        }

        private async Task RunIntegrationTestSample()
        {
            _output.WriteLine("--- Executando Testes de Integração ---");
            
            using var integrationTests = new IntegrationTests.VehicleApplicationIntegrationTests();
            
            // Teste de integração com banco em memória
            await integrationTests.ListAvailableVehicles_WhenVehiclesExist_ReturnsFromDatabase();
            _output.WriteLine("? Teste de integração com banco de dados executado");
            
            _output.WriteLine("--- Testes de Integração Concluídos ---\n");
        }

        [Theory]
        [Trait("Category", "Comprehensive")]
        [InlineData("ADMIN", true)]
        [InlineData("EMPLOYEE", true)]
        [InlineData("COMMON_USER", false)]
        public void DemonstrateParameterizedTesting(string role, bool hasPermission)
        {
            _output.WriteLine($"Testando role: {role}, Permissão esperada: {hasPermission}");
            
            // Simular lógica de verificação de permissão
            var actualPermission = role == "ADMIN" || role == "EMPLOYEE";
            
            Assert.Equal(hasPermission, actualPermission);
            _output.WriteLine($"? Teste parametrizado para {role} passou com sucesso");
        }

        [Fact]
        [Trait("Category", "Performance")]
        public void DemonstratePerformanceConsiderations()
        {
            _output.WriteLine("--- Demonstrando Considerações de Performance ---");
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Simular operação que pode ter problemas de performance
            var vehicles = new List<object>();
            for (int i = 0; i < 10000; i++)
            {
                vehicles.Add(new { Id = i, Name = $"Vehicle{i}" });
            }
            
            stopwatch.Stop();
            
            _output.WriteLine($"Criação de 10.000 objetos levou: {stopwatch.ElapsedMilliseconds}ms");
            
            // Assert que a operação não deve demorar muito
            Assert.True(stopwatch.ElapsedMilliseconds < 1000, "Operação deve ser concluída em menos de 1 segundo");
            
            _output.WriteLine("? Teste de performance passou");
        }

        [Fact]
        [Trait("Category", "DataValidation")]
        public void DemonstrateDataValidation()
        {
            _output.WriteLine("--- Demonstrando Validação de Dados ---");
            
            // Teste de validação de email
            var validEmails = new[] { "test@email.com", "user@domain.co.uk", "admin@company.org" };
            var invalidEmails = new[] { "invalid", "@domain.com", "test@", "" };
            
            foreach (var email in validEmails)
            {
                var isValid = IsValidEmail(email);
                Assert.True(isValid, $"Email {email} deveria ser válido");
                _output.WriteLine($"? Email válido: {email}");
            }
            
            foreach (var email in invalidEmails)
            {
                var isValid = IsValidEmail(email);
                Assert.False(isValid, $"Email {email} deveria ser inválido");
                _output.WriteLine($"? Email inválido detectado: {email}");
            }
            
            _output.WriteLine("--- Validação de Dados Concluída ---");
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
                
            return email.Contains("@") && email.Contains(".") && 
                   email.IndexOf("@") > 0 && 
                   email.LastIndexOf(".") > email.IndexOf("@");
        }
    }
}