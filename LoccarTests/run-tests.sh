#!/bin/bash

echo "======================================"
echo "  Executando Testes do Projeto Loccar"
echo "======================================"

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Fun��o para imprimir com cor
print_colored() {
    printf "${1}${2}${NC}\n"
}

# Navegar para o diret�rio do projeto de testes
cd LoccarTests

print_colored $BLUE "?? Restaurando pacotes NuGet..."
dotnet restore
if [ $? -eq 0 ]; then
    print_colored $GREEN "? Pacotes restaurados com sucesso"
else
    print_colored $RED "? Falha ao restaurar pacotes"
    exit 1
fi

print_colored $BLUE "???  Compilando projeto de testes..."
dotnet build --no-restore
if [ $? -eq 0 ]; then
    print_colored $GREEN "? Compila��o conclu�da com sucesso"
else
    print_colored $RED "? Falha na compila��o"
    exit 1
fi

print_colored $YELLOW "?? Executando Testes Unit�rios..."
dotnet test --no-build --filter "FullyQualifiedName~UnitTests" --logger "console;verbosity=normal" --collect:"XPlat Code Coverage"

print_colored $YELLOW "?? Executando Testes de Integra��o..."
dotnet test --no-build --filter "FullyQualifiedName~IntegrationTests" --logger "console;verbosity=normal"

print_colored $YELLOW "?? Executando Testes Parametrizados..."
dotnet test --no-build --filter "FullyQualifiedName~ParametrizedTests" --logger "console;verbosity=normal"

print_colored $BLUE "?? Executando Todos os Testes com Relat�rio Detalhado..."
dotnet test --no-build --logger "trx;LogFileName=TestResults.trx" --logger "html;LogFileName=TestResults.html" --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Verificar se h� falhas nos testes
if [ $? -eq 0 ]; then
    print_colored $GREEN "? Todos os testes executados com sucesso!"
    print_colored $BLUE "?? Relat�rios salvos em: ./TestResults/"
else
    print_colored $RED "? Alguns testes falharam. Verifique os logs acima."
    exit 1
fi

print_colored $YELLOW "?? Gerando Relat�rio de Cobertura..."
# Instalar ferramenta de relat�rio se n�o existir
dotnet tool install -g dotnet-reportgenerator-globaltool 2>/dev/null || true

# Gerar relat�rio de cobertura em HTML
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./TestResults/CoverageReport" -reporttypes:Html

if [ -d "./TestResults/CoverageReport" ]; then
    print_colored $GREEN "?? Relat�rio de cobertura gerado em: ./TestResults/CoverageReport/index.html"
fi

print_colored $GREEN "?? Execu��o de testes conclu�da!"
print_colored $BLUE "?? Para ver os resultados:"
print_colored $BLUE "   - Relat�rio de testes: ./TestResults/TestResults.html"
print_colored $BLUE "   - Cobertura de c�digo: ./TestResults/CoverageReport/index.html"