# Configuração CORS - Loccar API

Este documento descreve a configuração do CORS (Cross-Origin Resource Sharing) implementada na API Loccar.

## Configuração Implementada

### Políticas CORS

A API implementa duas políticas CORS distintas:

#### 1. **LoccarCorsPolicy** (Produção)
- **Uso**: Ambiente de produção
- **Origens**: Configuradas via `appsettings.json` 
- **Métodos**: Todos (`AllowAnyMethod()`)
- **Headers**: Todos (`AllowAnyHeader()`)
- **Credenciais**: Permitidas (`AllowCredentials()`)
- **Subdomínios**: Suporte a wildcards (`SetIsOriginAllowedToAllowWildcardSubdomains()`)

#### 2. **DevelopmentCorsPolicy** (Desenvolvimento)
- **Uso**: Ambiente de desenvolvimento
- **Origens**: Todas (`AllowAnyOrigin()`)
- **Métodos**: Todos (`AllowAnyMethod()`)
- **Headers**: Todos (`AllowAnyHeader()`)
- **Credenciais**: Não permitidas (devido ao `AllowAnyOrigin()`)

## Configuração nos arquivos

### appsettings.json (Desenvolvimento)
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:4200", 
      "http://localhost:5173",
      "http://localhost:8080",
      "https://localhost:3000",
      "https://localhost:4200",
      "https://localhost:5173",
      "https://localhost:8080"
    ]
  }
}
```

### appsettings.Production.json (Produção)
```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://loccar-frontend.com",
      "https://www.loccar-frontend.com",
      "http://localhost:3000",
      "http://localhost:4200",
      "http://localhost:5173"
    ]
  }
}
```

## Implementação no Código

### Program.cs
```csharp
// Configuração das políticas CORS
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
    ?? new[] { "http://localhost:3000", "http://localhost:4200", "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    // Política para produção
    options.AddPolicy("LoccarCorsPolicy", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetIsOriginAllowedToAllowWildcardSubdomains();
    });

    // Política para desenvolvimento
    options.AddPolicy("DevelopmentCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Aplicação da política baseada no ambiente
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentCorsPolicy");
}
else
{
    app.UseCors("LoccarCorsPolicy");
}
```

## Origens Suportadas

### Desenvolvimento
- **React**: `http://localhost:3000`, `https://localhost:3000`
- **Angular**: `http://localhost:4200`, `https://localhost:4200`
- **Vite/Vue**: `http://localhost:5173`, `https://localhost:5173`
- **Aplicação**: `http://localhost:8080`, `https://localhost:8080`

### Produção
- **Frontend Principal**: `https://loccar-frontend.com`
- **WWW**: `https://www.loccar-frontend.com`
- **Desenvolvimento Local**: Mantidas para testes

## Ordem de Middleware

É importante notar que o CORS está configurado **antes** dos middlewares de autenticação e autorização:

```csharp
app.UseCors(); // ✅ Primeiro
app.UseAuthentication();
app.UseAuthorization();
```

## Como Adicionar Novas Origens

### Para Desenvolvimento
1. Editar `appsettings.json`
2. Adicionar nova origem no array `Cors:AllowedOrigins`

### Para Produção
1. Editar `appsettings.Production.json`
2. Adicionar nova origem no array `Cors:AllowedOrigins`

### Exemplo
```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://nova-origem.com",
      "https://app.cliente.com"
    ]
  }
}
```

## Testes de CORS

### Verificação via Browser
```javascript
fetch('http://localhost:8080/api/statistics/system', {
  method: 'GET',
  headers: {
    'Authorization': 'Bearer <token>',
    'Content-Type': 'application/json'
  },
  credentials: 'include'
})
.then(response => response.json())
.then(data => console.log(data));
```

### Verificação via cURL
```bash
curl -H "Origin: http://localhost:3000" \
     -H "Access-Control-Request-Method: GET" \
     -H "Access-Control-Request-Headers: authorization,content-type" \
     -X OPTIONS \
     http://localhost:8080/api/statistics/system
```

## Troubleshooting

### Problemas Comuns

1. **Erro**: `Access to fetch at '...' from origin '...' has been blocked by CORS policy`
   - **Solução**: Verificar se a origem está listada em `AllowedOrigins`

2. **Erro**: `Credentials include but CORS not configured for credentials`
   - **Solução**: Verificar se `AllowCredentials()` está configurado

3. **Erro**: `CORS error in production but works in development`
   - **Solução**: Verificar `appsettings.Production.json`

### Debug CORS
Para debug detalhado, adicionar logs:

```csharp
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.SetMinimumLevel(LogLevel.Debug);
});
```

## Segurança

### Boas Práticas Implementadas
- ✅ Origens específicas em produção (não usa `AllowAnyOrigin`)
- ✅ Suporte a HTTPS em produção
- ✅ Configuração separada por ambiente
- ✅ Fallback para origens padrão se configuração falhar

### Considerações de Segurança
- Evitar `AllowAnyOrigin` em produção
- Usar HTTPS sempre que possível
- Manter lista de origens atualizada
- Monitorar logs para tentativas de acesso não autorizadas
