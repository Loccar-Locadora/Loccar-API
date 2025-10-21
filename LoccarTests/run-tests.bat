@echo off
echo ======================================
echo   Executando Testes do Projeto Loccar
echo ======================================

REM Navegar para o diretório do projeto de testes
cd LoccarTests

echo ?? Restaurando pacotes NuGet...
dotnet restore
if %ERRORLEVEL% neq 0 (
    echo ? Falha ao restaurar pacotes
    pause
    exit /b 1
)
echo ? Pacotes restaurados com sucesso

echo ??? Compilando projeto de testes...
dotnet build --no-restore
if %ERRORLEVEL% neq 0 (
    echo ? Falha na compilação
    pause
    exit /b 1
)
echo ? Compilação concluída com sucesso

echo ?? Executando Testes Unitários...
dotnet test --no-build --filter "FullyQualifiedName~UnitTests" --logger "console;verbosity=normal" --collect:"XPlat Code Coverage"

echo ?? Executando Testes de Integração...
dotnet test --no-build --filter "FullyQualifiedName~IntegrationTests" --logger "console;verbosity=normal"

echo ?? Executando Testes Parametrizados...
dotnet test --no-build --filter "FullyQualifiedName~ParametrizedTests" --logger "console;verbosity=normal"

echo ?? Executando Todos os Testes com Relatório Detalhado...
dotnet test --no-build --logger "trx;LogFileName=TestResults.trx" --logger "html;LogFileName=TestResults.html" --collect:"XPlat Code Coverage" --results-directory ./TestResults

if %ERRORLEVEL% neq 0 (
    echo ? Alguns testes falharam. Verifique os logs acima.
    pause
    exit /b 1
)

echo ? Todos os testes executados com sucesso!
echo ?? Relatórios salvos em: ./TestResults/

echo ?? Gerando Relatório de Cobertura...
REM Instalar ferramenta de relatório se não existir
dotnet tool install -g dotnet-reportgenerator-globaltool >nul 2>&1

REM Gerar relatório de cobertura em HTML
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./TestResults/CoverageReport" -reporttypes:Html

if exist "./TestResults/CoverageReport" (
    echo ?? Relatório de cobertura gerado em: ./TestResults/CoverageReport/index.html
)

echo ?? Execução de testes concluída!
echo ?? Para ver os resultados:
echo    - Relatório de testes: ./TestResults/TestResults.html
echo    - Cobertura de código: ./TestResults/CoverageReport/index.html

pause