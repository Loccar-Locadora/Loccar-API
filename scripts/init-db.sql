-- Script de inicialização do banco de dados
-- Este arquivo será executado quando o container PostgreSQL for criado pela primeira vez

-- Criar extensões necessárias (se houver)
-- CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Configurações específicas do banco
-- SET timezone = 'America/Sao_Paulo';

-- Log da inicialização
SELECT 'Database loccardb initialized successfully' as status;
