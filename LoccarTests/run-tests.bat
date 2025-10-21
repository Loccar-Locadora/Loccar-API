@echo off
echo ======================================
echo   Executando Testes do Projeto Loccar
echo ======================================

REM Navegar para o diret�rio do projeto de testes
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
    echo ? Falha na compila��o
    pause
    exit /b 1
)
echo ? Compila��o conclu�da com sucesso

echo ?? Executando Testes Unit�rios...
dotnet test --no-build --filter "FullyQualifiedName~UnitTests" --logger "console;verbosity=normal" --collect:"XPlat Code Coverage"

echo ?? Executando Testes de Integra��o...
dotnet test --no-build --filter "FullyQualifiedName~IntegrationTests" --logger "console;verbosity=normal"

echo ?? Executando Testes Parametrizados...
dotnet test --no-build --filter "FullyQualifiedName~ParametrizedTests" --logger "console;verbosity=normal"

echo ?? Executando Todos os Testes com Relat�rio Detalhado...
dotnet test --no-build --logger "trx;LogFileName=TestResults.trx" --logger "html;LogFileName=TestResults.html" --collect:"XPlat Code Coverage" --results-directory ./TestResults

if %ERRORLEVEL% neq 0 (
    echo ? Alguns testes falharam. Verifique os logs acima.
    pause
    exit /b 1
)

echo ? Todos os testes executados com sucesso!
echo ?? Relat�rios salvos em: ./TestResults/

echo ?? Gerando Relat�rio de Cobertura...
REM Instalar ferramenta de relat�rio se n�o existir
dotnet tool install -g dotnet-reportgenerator-globaltool >nul 2>&1

REM Gerar relat�rio de cobertura em HTML
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./TestResults/CoverageReport" -reporttypes:Html

if exist "./TestResults/CoverageReport" (
    echo ?? Relat�rio de cobertura gerado em: ./TestResults/CoverageReport/index.html
)

echo ?? Execu��o de testes conclu�da!
echo ?? Para ver os resultados:
echo    - Relat�rio de testes: ./TestResults/TestResults.html
echo    - Cobertura de c�digo: ./TestResults/CoverageReport/index.html

pause