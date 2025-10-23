#!/bin/bash

# Script para executar análise de código completa
echo "=== Loccar Code Analysis Script ==="
echo ""

# Função para imprimir mensagens coloridas
print_info() {
    echo -e "\033[1;34m[INFO]\033[0m $1"
}

print_success() {
    echo -e "\033[1;32m[SUCCESS]\033[0m $1"
}

print_warning() {
    echo -e "\033[1;33m[WARNING]\033[0m $1"
}

print_error() {
    echo -e "\033[1;31m[ERROR]\033[0m $1"
}

# Verifica se o .NET está instalado
if ! command -v dotnet &> /dev/null; then
    print_error ".NET SDK não encontrado. Instale o .NET 8 SDK primeiro."
    exit 1
fi

print_info "Versão do .NET:"
dotnet --version
echo ""

# Restaura dependências
print_info "Restaurando dependências..."
if dotnet restore; then
    print_success "Dependências restauradas com sucesso"
else
    print_error "Falha ao restaurar dependências"
    exit 1
fi
echo ""

# Verifica formatação do código (projetos principais apenas)
print_info "Verificando formatação do código (projetos principais)..."
FORMAT_SUCCESS=true
PROJECTS=("LoccarDomain" "LoccarApplication" "LoccarInfra" "LoccarLocadora")

for project in "${PROJECTS[@]}"; do
    print_info "Verificando formatação do projeto: $project"
    if dotnet format --include "$project" --verify-no-changes --verbosity diagnostic > /dev/null 2>&1; then
        print_success "Projeto $project está formatado corretamente"
    else
        print_warning "Projeto $project não está formatado corretamente"
        FORMAT_SUCCESS=false
    fi
done

if [ "$FORMAT_SUCCESS" = true ]; then
    print_success "Todos os projetos principais estão formatados corretamente"
else
    print_warning "Alguns projetos não estão formatados corretamente"
    print_info "Execute 'dotnet format' para corrigir automaticamente"
fi
echo ""

# Build com análise de código
print_info "Compilando com análise de código..."
BUILD_OUTPUT=$(dotnet build --configuration Release --verbosity normal 2>&1)
BUILD_EXIT_CODE=$?

if [ $BUILD_EXIT_CODE -eq 0 ]; then
    print_success "Build realizado com sucesso"
else
    print_error "Build falhou"
fi

# Extrai e exibe avisos de análise de código
echo ""
print_info "=== Resultados da Análise de Código ==="
CRITICAL_WARNINGS=$(echo "$BUILD_OUTPUT" | grep -E "(warning|error) (CA|SA)[0-9]+" || true)
if [ -n "$CRITICAL_WARNINGS" ]; then
    print_warning "Avisos de análise encontrados:"
    echo "$CRITICAL_WARNINGS"
else
    print_info "Nenhum aviso crítico de análise de código encontrado"
fi
echo ""

# Executa testes
print_info "Executando testes..."
if dotnet test --configuration Release --verbosity normal --no-build; then
    print_success "Todos os testes passaram"
else
    print_warning "Alguns testes falharam"
fi
echo ""

# Resumo
print_info "=== Resumo da Análise ==="
if [ $BUILD_EXIT_CODE -eq 0 ]; then
    print_success "Análise concluída com sucesso!"
    echo ""
    echo -e "\033[0;37mComandos úteis:\033[0m"
    echo -e "\033[0;37m  - Formatar código: dotnet format\033[0m"
    echo -e "\033[0;37m  - Verificar formatação: dotnet format --verify-no-changes\033[0m"
    echo -e "\033[0;37m  - Build detalhado: dotnet build --verbosity detailed\033[0m"
    echo -e "\033[0;37m  - Executar apenas testes: dotnet test\033[0m"
else
    print_error "Análise concluída com erros. Verifique os problemas acima."
    exit 1
fi
