# Configuração Docker para Loccar API

## Pré-requisitos

**IMPORTANTE**: Este docker-compose assume que você já tem o banco PostgreSQL rodando em outro container com as seguintes características:
- Nome do container: `loccar-db`
- Banco de dados: `loccardb`
- Usuário/Senha: `postgres/postgres`

## Como usar

### 1. Certifique-se de que o banco está rodando

Antes de executar a aplicação, verifique se o banco PostgreSQL já está ativo:

```bash
docker ps | grep loccar-db
```

### 2. Execute apenas a aplicação

```bash
docker-compose up
```

Ou para rodar em background:

```bash
docker-compose up -d
```

### 3. Verificar logs da aplicação

```bash
docker-compose logs -f loccar-locadora
```

## Configurações importantes:

- **Porta da aplicação**: `http://localhost:5001` (HTTP, não HTTPS)
- **Swagger**: `http://localhost:5001/swagger`
- **Conexão com banco**: Conecta ao container `loccar-db` usando a rede padrão do Docker

⚠️ **Nota**: A aplicação roda em HTTP no container para evitar problemas com certificados SSL. Em produção, configure um proxy reverso (como nginx) para HTTPS.

## Resolução de problemas:

### Se a aplicação não conseguir conectar ao banco:

1. Verifique se o banco está rodando:
   ```bash
   docker ps | grep loccar-db
   ```

2. Verifique se ambos os containers estão na mesma rede:
   ```bash
   docker network ls
   docker inspect loccar-db | grep NetworkMode
   ```

### Se a aplicação não responder na porta 5001:

1. Verifique se o container está rodando:
   ```bash
   docker ps | grep loccar-locadora
   ```

2. Verifique os logs:
   ```bash
   docker-compose logs loccar-locadora
   ```

3. Teste diretamente no container:
   ```bash
   docker exec -it loccar-locadora curl http://localhost:8080/swagger
   ```

### Para parar a aplicação:

```bash
docker-compose down
```

## Conectividade:

- A aplicação escuta na porta 8080 dentro do container
- O Docker mapeia a porta 8080 do container para a porta 5001 do host
- A aplicação se conecta ao banco usando o nome do container `loccar-db`
- Não usa HTTPS para evitar problemas com certificados em containers
