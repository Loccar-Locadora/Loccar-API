# LoccarTests - Testes para o Sistema Loccar

Este projeto contém uma suíte completa de testes para o sistema de locação de veículos Loccar, incluindo testes unitários, de integração e parametrizados.

## ? Implementação Completa

- **Testes Unitários**: Testam a lógica de negócio isoladamente usando mocks
- **Testes de Integração**: Testam a integração com banco de dados usando Entity Framework In-Memory
- **Testes Parametrizados**: Testam múltiplos cenários com diferentes entradas
- **Testes de Performance**: Benchmarks usando BenchmarkDotNet

## ?? Estrutura do Projeto

```
LoccarTests/
??? UnitTests/                    # Testes unitários
?   ??? VehicleApplicationTests.cs
?   ??? CustomerApplicationTests.cs
?   ??? ReservationApplicationTests.cs
??? IntegrationTests/             # Testes de integração
?   ??? VehicleApplicationIntegrationTests.cs
?   ??? CustomerApplicationIntegrationTests.cs
?   ??? JwtTokenHelper.cs
??? ParameterizedTests/           # Testes parametrizados
?   ??? VehicleApplicationParameterizedTests.cs
?   ??? ReservationApplicationParameterizedTests.cs
??? PerformanceTests/             # Testes de performance
?   ??? VehicleApplicationBenchmarks.cs
??? TestSuites/                   # Suítes de teste
?   ??? ComprehensiveTestSuite.cs
??? Configuration/                # Configurações de teste
    ??? TestConfiguration.cs
```

## ?? Tecnologias Utilizadas

- **xUnit**: Framework de testes principal
- **Moq**: Biblioteca para criar mocks e stubs
- **FluentAssertions**: Biblioteca para asserções mais legíveis
- **AutoFixture**: Geração automática de dados de teste
- **EntityFrameworkCore.InMemory**: Banco de dados em memória para testes
- **BenchmarkDotNet**: Para testes de performance e benchmarks

## ?? Como Executar os Testes

### Executar Todos os Testes
```bash
dotnet test
```

### Executar Testes com Saída Detalhada
```bash
dotnet test --verbosity detailed
```

### Executar Testes por Categoria
```bash
# Testes unitários
dotnet test --filter "FullyName~UnitTests"

# Testes de integração
dotnet test --filter "FullyName~IntegrationTests"

# Testes parametrizados
dotnet test --filter "FullyName~ParameterizedTests"
```

### Executar com Cobertura de Código
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## ?? Cobertura de Testes

### Cenários Implementados

#### ? Testes Unitários
- [x] Autorização de usuários (Admin, Employee, Common User)
- [x] Validação de dados de entrada
- [x] Tratamento de exceções
- [x] Operações CRUD completas
- [x] Lógica de negócio específica

#### ? Testes de Integração
- [x] Integração com Entity Framework
- [x] Persistência de dados no banco
- [x] Consultas e atualizações
- [x] Transações de banco de dados

#### ? Testes Parametrizados
- [x] Diferentes roles de usuário
- [x] Múltiplos tipos de veículo
- [x] Cálculos de custo com diferentes parâmetros
- [x] Validação de dados com múltiplos cenários
- [x] Tratamento de diferentes tipos de exceção

#### ? Testes de Performance
- [x] Benchmarks de operações principais
- [x] Medição de tempo de execução
- [x] Medição de uso de memória
- [x] Testes de carga

## ?? Principais Classes Testadas

### VehicleApplication
- Cadastro de veículos
- Listagem de veículos disponíveis
- Operações CRUD completas
- Controle de manutenção

### CustomerApplication
- Cadastro de clientes
- Operações CRUD de clientes
- Validação de dados

### ReservationApplication
- Criação de reservas
- Cálculo de custos
- Histórico de reservas
- Cancelamento de reservas

## ?? Padrões Implementados

### Arrange-Act-Assert (AAA)
Todos os testes seguem o padrão AAA para clareza:
```csharp
[Fact]
public async Task Method_Scenario_ExpectedResult()
{
    // Arrange - Configurar dados de teste
    var input = CreateTestData();
    
    // Act - Executar a ação
    var result = await _service.Method(input);
    
    // Assert - Verificar resultado
    result.Should().BeEquivalentTo(expected);
}
```

### Test Data Builders
Uso do AutoFixture para geração automática de dados:
```csharp
var vehicle = _fixture.Create<Vehicle>();
```

### Mocking Strategy
```csharp
_mockRepository.Setup(x => x.Method(It.IsAny<Type>()))
              .ReturnsAsync(expectedResult);
```

## ????? Executando Benchmarks

Para executar os testes de performance:
```csharp
// Na classe de teste
VehicleApplicationBenchmarks.RunBenchmarks();
```

## ?? Métricas de Qualidade

- **Cobertura de Código**: Alta cobertura das camadas de Application
- **Testes por Cenário**: Múltiplos cenários por funcionalidade
- **Isolamento**: Cada teste é independente
- **Performance**: Benchmarks documentam performance atual

## ?? Configuração de CI/CD

### GitHub Actions
```yaml
name: Tests
on: [push, pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: 8.0.x
      - name: Run tests
        run: dotnet test --verbosity normal
```

## ?? Troubleshooting

### Problemas Comuns
1. **Warnings de Nullability**: São esperados em projetos de teste
2. **Banco em Memória**: Cada teste usa uma instância isolada
3. **Mocks**: Verificar se todos os setups estão corretos

### Logs Detalhados
```bash
dotnet test --logger "console;verbosity=detailed"
```

## ?? Próximos Passos

- [ ] Adicionar mais testes de API com WebApplicationFactory
- [ ] Implementar testes de stress
- [ ] Adicionar testes de segurança
- [ ] Configurar métricas de cobertura automáticas

## ?? Como Contribuir

1. Siga os padrões existentes
2. Adicione testes para nova funcionalidade
3. Use as mesmas bibliotecas
4. Execute todos os testes antes do commit
5. Documente cenários complexos

---

**Status**: ? Implementação completa com testes unitários, integração, parametrizados e performance funcionando corretamente.