# Script PowerShell para executar análise de código completa
Write-Host "=== Loccar Code Analysis Script ===" -ForegroundColor Cyan
Write-Host ""

# Função para imprimir mensagens coloridas
function Write-Info {
    param($Message)
    Write-Host "[INFO] $Message" -ForegroundColor Blue
}

function Write-Success {
    param($Message)
    Write-Host "[SUCCESS] $Message" -ForegroundColor Green
}

function Write-Warning {
    param($Message)
    Write-Host "[WARNING] $Message" -ForegroundColor Yellow
}

function Write-Error {
    param($Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

# Verifica se o .NET está instalado
try {
    $dotnetVersion = dotnet --version
    Write-Info "Versão do .NET: $dotnetVersion"
    Write-Host ""
}
catch {
    Write-Error ".NET SDK não encontrado. Instale o .NET 8 SDK primeiro."
    exit 1
}

# Restaura dependências
Write-Info "Restaurando dependências..."
$restoreResult = dotnet restore
if ($LASTEXITCODE -eq 0) {
    Write-Success "Dependências restauradas com sucesso"
}
else {
    Write-Error "Falha ao restaurar dependências"
    exit 1
}
Write-Host ""

# Verifica formatação do código (projetos principais apenas)
Write-Info "Verificando formatação do código (projetos principais)..."
$formatSuccess = $true
$projects = @("LoccarDomain", "LoccarApplication", "LoccarInfra", "LoccarLocadora")

foreach ($project in $projects) {
    Write-Info "Verificando formatação do projeto: $project"
    $formatCheck = dotnet format --include $project --verify-no-changes --verbosity diagnostic 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Projeto $project não está formatado corretamente"
        $formatSuccess = $false
    }
    else {
        Write-Success "Projeto $project está formatado corretamente"
    }
}

if ($formatSuccess) {
    Write-Success "Todos os projetos principais estão formatados corretamente"
}
else {
    Write-Warning "Alguns projetos não estão formatados corretamente"
    Write-Info "Execute 'dotnet format' para corrigir automaticamente"
}
Write-Host ""

# Build com análise de código
Write-Info "Compilando com análise de código..."
$buildOutput = dotnet build --configuration Release --verbosity normal 2>&1
$buildExitCode = $LASTEXITCODE

if ($buildExitCode -eq 0) {
    Write-Success "Build realizado com sucesso"
}
else {
    Write-Error "Build falhou"
}

# Extrai e exibe avisos de análise de código
Write-Host ""
Write-Info "=== Resultados da Análise de Código ==="
$codeAnalysisWarnings = $buildOutput | Select-String -Pattern "(warning|error) (CA|SA|CS)\d+"
if ($codeAnalysisWarnings) {
    $criticalWarnings = $codeAnalysisWarnings | Select-String -Pattern "(error|warning) (CA|SA)"
    if ($criticalWarnings) {
        Write-Warning "Avisos críticos de análise encontrados:"
        $criticalWarnings | ForEach-Object { Write-Host $_.Line -ForegroundColor Yellow }
    }
    else {
        Write-Info "Apenas avisos menores de análise de código encontrados"
    }
}
else {
    Write-Info "Nenhum aviso crítico de análise de código encontrado"
}
Write-Host ""

# Executa testes
Write-Info "Executando testes..."
$testResult = dotnet test --configuration Release --verbosity normal --no-build
if ($LASTEXITCODE -eq 0) {
    Write-Success "Todos os testes passaram"
}
else {
    Write-Warning "Alguns testes falharam"
}
Write-Host ""

# Resumo
Write-Info "=== Resumo da Análise ==="
if ($buildExitCode -eq 0) {
    Write-Success "Análise concluída com sucesso!"
    Write-Host ""
    Write-Host "Comandos úteis:" -ForegroundColor Gray
    Write-Host "  - Formatar código: dotnet format" -ForegroundColor Gray
    Write-Host "  - Verificar formatação: dotnet format --verify-no-changes" -ForegroundColor Gray
    Write-Host "  - Build detalhado: dotnet build --verbosity detailed" -ForegroundColor Gray
    Write-Host "  - Executar apenas testes: dotnet test" -ForegroundColor Gray
}
else {
    Write-Error "Análise concluída com erros. Verifique os problemas acima."
    exit 1
}
