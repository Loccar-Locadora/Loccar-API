using System.Globalization;
using Xunit;

namespace LoccarTests
{
    public class TestAssemblyFixture : IDisposable
    {
        public TestAssemblyFixture()
        {
            // Garantir cultura invariante para todos os testes
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            
            // Configurar o thread atual também
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        }

        public void Dispose()
        {
            // Cleanup se necessário
        }
    }

    [CollectionDefinition("TestCollection")]
    public class TestCollection : ICollectionFixture<TestAssemblyFixture>
    {
        // Esta classe não tem código, apenas serve como marcador
        // para aplicar o fixture a todos os testes
    }
}