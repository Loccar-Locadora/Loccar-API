# LoccarTests - Testes para o Sistema Loccar

Este projeto cont�m uma su�te completa de testes para o sistema de loca��o de ve�culos Loccar, incluindo testes unit�rios, de integra��o e parametrizados.

## ? Implementa��o Completa

- **Testes Unit�rios**: Testam a l�gica de neg�cio isoladamente usando mocks
- **Testes de Integra��o**: Testam a integra��o com banco de dados usando Entity Framework In-Memory
- **Testes Parametrizados**: Testam m�ltiplos cen�rios com diferentes entradas
- **Testes de Performance**: Benchmarks usando BenchmarkDotNet

## ?? Estrutura do Projeto

```
LoccarTests/
??? UnitTests/                    # Testes unit�rios
?   ??? VehicleApplicationTests.cs
?   ??? CustomerApplicationTests.cs
?   ??? ReservationApplicationTests.cs
??? IntegrationTests/             # Testes de integra��o
?   ??? VehicleApplicationIntegrationTests.cs
?   ??? CustomerApplicationIntegrationTests.cs
?   ??? JwtTokenHelper.cs
??? ParameterizedTests/           # Testes parametrizados
?   ??? VehicleApplicationParameterizedTests.cs
?   ??? ReservationApplicationParameterizedTests.cs
??? PerformanceTests/             # Testes de performance
?   ??? VehicleApplicationBenchmarks.cs
??? TestSuites/                   # Su�tes de teste
?   ??? ComprehensiveTestSuite.cs
??? Configuration/                # Configura��es de teste
    ??? TestConfiguration.cs
```

## ?? Tecnologias Utilizadas

- **xUnit**: Framework de testes principal
- **Moq**: Biblioteca para criar mocks e stubs
- **FluentAssertions**: Biblioteca para asser��es mais leg�veis
- **AutoFixture**: Gera��o autom�tica de dados de teste
- **EntityFrameworkCore.InMemory**: Banco de dados em mem�ria para testes
- **BenchmarkDotNet**: Para testes de performance e benchmarks

## ?? Como Executar os Testes

### Executar Todos os Testes
```bash
dotnet test
```

### Executar Testes com Sa�da Detalhada
```bash
dotnet test --verbosity detailed
```

### Executar Testes por Categoria
```bash
# Testes unit�rios
dotnet test --filter "FullyName~UnitTests"

# Testes de integra��o
dotnet test --filter "FullyName~IntegrationTests"

# Testes parametrizados
dotnet test --filter "FullyName~ParameterizedTests"
```

### Executar com Cobertura de C�digo
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## ?? Cobertura de Testes

### Cen�rios Implementados

#### ? Testes Unit�rios
- [x] Autoriza��o de usu�rios (Admin, Employee, Common User)
- [x] Valida��o de dados de entrada
- [x] Tratamento de exce��es
- [x] Opera��es CRUD completas
- [x] L�gica de neg�cio espec�fica

#### ? Testes de Integra��o
- [x] Integra��o com Entity Framework
- [x] Persist�ncia de dados no banco
- [x] Consultas e atualiza��es
- [x] Transa��es de banco de dados

#### ? Testes Parametrizados
- [x] Diferentes roles de usu�rio
- [x] M�ltiplos tipos de ve�culo
- [x] C�lculos de custo com diferentes par�metros
- [x] Valida��o de dados com m�ltiplos cen�rios
- [x] Tratamento de diferentes tipos de exce��o

#### ? Testes de Performance
- [x] Benchmarks de opera��es principais
- [x] Medi��o de tempo de execu��o
- [x] Medi��o de uso de mem�ria
- [x] Testes de carga

## ?? Principais Classes Testadas

### VehicleApplication
- Cadastro de ve�culos
- Listagem de ve�culos dispon�veis
- Opera��es CRUD completas
- Controle de manuten��o

### CustomerApplication
- Cadastro de clientes
- Opera��es CRUD de clientes
- Valida��o de dados

### ReservationApplication
- Cria��o de reservas
- C�lculo de custos
- Hist�rico de reservas
- Cancelamento de reservas

## ?? Padr�es Implementados

### Arrange-Act-Assert (AAA)
Todos os testes seguem o padr�o AAA para clareza:
```csharp
[Fact]
public async Task Method_Scenario_ExpectedResult()
{
    // Arrange - Configurar dados de teste
    var input = CreateTestData();
    
    // Act - Executar a a��o
    var result = await _service.Method(input);
    
    // Assert - Verificar resultado
    result.Should().BeEquivalentTo(expected);
}
```

### Test Data Builders
Uso do AutoFixture para gera��o autom�tica de dados:
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

## ?? M�tricas de Qualidade

- **Cobertura de C�digo**: Alta cobertura das camadas de Application
- **Testes por Cen�rio**: M�ltiplos cen�rios por funcionalidade
- **Isolamento**: Cada teste � independente
- **Performance**: Benchmarks documentam performance atual

## ?? Configura��o de CI/CD

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
1. **Warnings de Nullability**: S�o esperados em projetos de teste
2. **Banco em Mem�ria**: Cada teste usa uma inst�ncia isolada
3. **Mocks**: Verificar se todos os setups est�o corretos

### Logs Detalhados
```bash
dotnet test --logger "console;verbosity=detailed"
```

## ?? Pr�ximos Passos

- [ ] Adicionar mais testes de API com WebApplicationFactory
- [ ] Implementar testes de stress
- [ ] Adicionar testes de seguran�a
- [ ] Configurar m�tricas de cobertura autom�ticas

## ?? Como Contribuir

1. Siga os padr�es existentes
2. Adicione testes para nova funcionalidade
3. Use as mesmas bibliotecas
4. Execute todos os testes antes do commit
5. Documente cen�rios complexos

---

**Status**: ? Implementa��o completa com testes unit�rios, integra��o, parametrizados e performance funcionando corretamente.