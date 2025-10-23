# Loccar API - Análise de Código e Linting

Este documento descreve como usar as ferramentas de análise de código e linting configuradas no projeto Loccar API.

## 🔧 Configuração

O projeto está configurado com as seguintes ferramentas de análise:

- **.NET Analyzers**: Análise nativa do .NET para detectar problemas de código
- **StyleCop Analyzers**: Verificação de estilo e convenções de codificação
- **SonarAnalyzer**: Análise adicional de qualidade e segurança
- **EditorConfig**: Configurações de formatação e estilo

### Arquivos de Configuração

- `.editorconfig` - Regras de formatação e estilo de código (global)
- `LoccarTests/.editorconfig` - Configurações específicas para testes (mais permissivas)
- `Directory.Build.props` - Configurações globais de análise
- `loccar.ruleset` - Regras específicas de análise de código
- `stylecop.json` - Configurações do StyleCop
- `.globalconfig` - Configuração global de severidade dos warnings

## 🚀 Como Executar a Análise

### 1. Executar Scripts Automatizados

#### Windows (PowerShell)
```powershell
.\scripts\run-lint.ps1
```

#### Linux/macOS (Bash)
```bash
chmod +x scripts/run-lint.sh
./scripts/run-lint.sh
```

### 2. Comandos Manuais

#### Verificar Formatação (Projetos Principais)
```bash
# Verificar projetos específicos
dotnet format --include LoccarDomain --verify-no-changes
dotnet format --include LoccarApplication --verify-no-changes
dotnet format --include LoccarInfra --verify-no-changes
dotnet format --include LoccarLocadora --verify-no-changes

# Formatar automaticamente todos os projetos
dotnet format
```

#### Build com Análise de Código
```bash
# Build com análise completa
dotnet build --configuration Release --verbosity normal

# Build com análise detalhada
dotnet build --verbosity detailed
```

#### Executar Testes
```bash
# Executar todos os testes
dotnet test --configuration Release
```

## 📊 Tipos de Verificações

### 1. Formatação de Código
- Indentação consistente
- Espaçamento adequado
- Quebras de linha
- Organização de using statements

### 2. Convenções de Nomenclatura
- PascalCase para classes, métodos, propriedades
- camelCase para variáveis locais e parâmetros
- Interfaces com prefixo 'I'
- Constantes em PascalCase

### 3. Qualidade de Código
- Detecção de código morto
- Verificações de performance
- Práticas de segurança
- Padrões de design

### 4. Análise de Segurança
- Vulnerabilidades conhecidas
- Práticas inseguras
- Validação de entrada
- Gerenciamento de recursos

## ⚙️ Configuração no IDE

### Visual Studio
1. As regras são aplicadas automaticamente
2. Warnings aparecem na janela Error List
3. Formatação automática com Ctrl+K, Ctrl+D

### Visual Studio Code
1. Instale a extensão C# Dev Kit
2. Configure o arquivo settings.json:
```json
{
  "omnisharp.enableEditorConfigSupport": true,
  "omnisharp.enableRoslynAnalyzers": true
}
```

## 🔄 Integração Contínua

O GitHub Actions está configurado para executar automaticamente:

1. **Verificação de Formatação**: Verifica projetos principais (não testes)
2. **Build com Análise**: Compila o projeto com todas as verificações
3. **Execução de Testes**: Garante que os testes passem
4. **Relatórios**: Gera artefatos com resultados

### Workflow do GitHub Actions

O arquivo `.github/workflows/dotnet.yml` contém dois jobs:

- **build**: Execução principal com testes
- **lint**: Verificação específica de qualidade de código

## 📝 Supressão de Regras

### Regras Suprimidas para Testes
Os projetos de teste têm regras mais permissivas:
- `S4487` - Campos privados não utilizados
- `S1481` - Variáveis locais não utilizadas
- `CA2201` - Exceções genéricas
- `S6608` - Uso de First() vs indexação
- `S6781` - Divulgação de segredos JWT (para testes)

### Nível de Arquivo
```csharp
#pragma warning disable CA1062 // Validate arguments of public methods
// código aqui
#pragma warning restore CA1062
```

### Nível de Método/Classe
```csharp
[SuppressMessage("Style", "CA1062:Validate arguments of public methods")]
public void MinhaFunção(string parametro)
{
    // código aqui
}
```

### Global (no .globalconfig)
```ini
dotnet_diagnostic.CA1062.severity = suggestion
```

## 🛠️ Comandos Úteis

```bash
# Restaurar dependências
dotnet restore

# Formatar código específico
dotnet format --include LoccarApplication

# Build limpo
dotnet clean && dotnet build

# Executar análise detalhada
dotnet build --verbosity detailed > analysis.log 2>&1

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 📋 Checklist para Pull Requests

Antes de criar um PR, certifique-se de que:

- [ ] `.\scripts\run-lint.ps1` executa sem erros críticos
- [ ] `dotnet build --configuration Release` compila sem falhas
- [ ] `dotnet test` executa todos os testes com sucesso
- [ ] Projetos principais passam na verificação de formatação
- [ ] Documentação foi atualizada se necessário

## 🎯 Metas de Qualidade

- **Zero** warnings críticos de formatação nos projetos principais
- **Mínimo** de warnings de análise de código
- **100%** dos testes passando
- **Consistência** na aplicação das regras

## 📞 Suporte

Para dúvidas sobre as regras de linting ou problemas na configuração:

1. Verifique os logs de build detalhados
2. Execute `.\scripts\run-lint.ps1` para diagnóstico completo
3. Consulte a documentação das ferramentas utilizadas
4. Abra uma issue no repositório

## 📈 Melhorias Recentes

### ✅ Problemas Resolvidos
- **Testes corrigidos**: 7 testes parameterizados foram corrigidos
- **Formatação funcional**: `dot
