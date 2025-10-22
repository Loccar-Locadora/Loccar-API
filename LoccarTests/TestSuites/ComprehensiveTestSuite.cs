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
            _output.WriteLine("\n--- Executando Testes Unit�rios ---");
            
            var unitTests = new UnitTests.VehicleApplicationTests();
            
            // Teste de autoriza��o
            await unitTests.RegisterVehicle_WhenUserIsNotAuthorized_ReturnsUnauthorized();
            _output.WriteLine("? Teste unit�rio de autoriza��o executado");
            
            // Teste de exce��o
            await unitTests.RegisterVehicle_WhenExceptionOccurs_ReturnsServerError();
            _output.WriteLine("? Teste unit�rio de tratamento de exce��o executado");
            
            _output.WriteLine("--- Testes Unit�rios Conclu�dos ---\n");
        }

        private async Task RunParameterizedTestSample()
        {
            _output.WriteLine("--- Executando Testes Parametrizados ---");
            
            var parameterizedTests = new ParameterizedTests.VehicleApplicationParameterizedTests();
            
            // Teste com diferentes roles de usu�rio
            await parameterizedTests.RegisterVehicle_WithDifferentUserRoles_ReturnsExpectedResult(
                new List<string> { "ADMIN" }, true, "201", "Ve�culo cadastrado com sucesso");
            _output.WriteLine("? Teste parametrizado com role ADMIN executado");
            
            await parameterizedTests.RegisterVehicle_WithDifferentUserRoles_ReturnsExpectedResult(
                new List<string> { "COMMON_USER" }, false, "401", "Usu�rio n�o autorizado.");
            _output.WriteLine("? Teste parametrizado com role COMMON_USER executado");
            
            _output.WriteLine("--- Testes Parametrizados Conclu�dos ---\n");
        }

        private async Task RunIntegrationTestSample()
        {
            _output.WriteLine("--- Executando Testes de Integra��o ---");
            
            using var integrationTests = new IntegrationTests.VehicleApplicationIntegrationTests();
            
            // Teste de integra��o com banco em mem�ria
            await integrationTests.ListAvailableVehicles_WhenVehiclesExist_ReturnsFromDatabase();
            _output.WriteLine("? Teste de integra��o com banco de dados executado");
            
            _output.WriteLine("--- Testes de Integra��o Conclu�dos ---\n");
        }

        [Theory]
        [Trait("Category", "Comprehensive")]
        [InlineData("ADMIN", true)]
        [InlineData("EMPLOYEE", true)]
        [InlineData("COMMON_USER", false)]
        public void DemonstrateParameterizedTesting(string role, bool hasPermission)
        {
            _output.WriteLine($"Testando role: {role}, Permiss�o esperada: {hasPermission}");
            
            // Simular l�gica de verifica��o de permiss�o
            var actualPermission = role == "ADMIN" || role == "EMPLOYEE";
            
            Assert.Equal(hasPermission, actualPermission);
            _output.WriteLine($"? Teste parametrizado para {role} passou com sucesso");
        }

        [Fact]
        [Trait("Category", "Performance")]
        public void DemonstratePerformanceConsiderations()
        {
            _output.WriteLine("--- Demonstrando Considera��es de Performance ---");
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Simular opera��o que pode ter problemas de performance
            var vehicles = new List<object>();
            for (int i = 0; i < 10000; i++)
            {
                vehicles.Add(new { Id = i, Name = $"Vehicle{i}" });
            }
            
            stopwatch.Stop();
            
            _output.WriteLine($"Cria��o de 10.000 objetos levou: {stopwatch.ElapsedMilliseconds}ms");
            
            // Assert que a opera��o n�o deve demorar muito
            Assert.True(stopwatch.ElapsedMilliseconds < 1000, "Opera��o deve ser conclu�da em menos de 1 segundo");
            
            _output.WriteLine("? Teste de performance passou");
        }

        [Fact]
        [Trait("Category", "DataValidation")]
        public void DemonstrateDataValidation()
        {
            _output.WriteLine("--- Demonstrando Valida��o de Dados ---");
            
            // Teste de valida��o de email
            var validEmails = new[] { "test@email.com", "user@domain.co.uk", "admin@company.org" };
            var invalidEmails = new[] { "invalid", "@domain.com", "test@", "" };
            
            foreach (var email in validEmails)
            {
                var isValid = IsValidEmail(email);
                Assert.True(isValid, $"Email {email} deveria ser v�lido");
                _output.WriteLine($"? Email v�lido: {email}");
            }
            
            foreach (var email in invalidEmails)
            {
                var isValid = IsValidEmail(email);
                Assert.False(isValid, $"Email {email} deveria ser inv�lido");
                _output.WriteLine($"? Email inv�lido detectado: {email}");
            }
            
            _output.WriteLine("--- Valida��o de Dados Conclu�da ---");
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