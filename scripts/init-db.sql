-- Script de inicialização do banco de dados
-- Este script é executado quando o container do SQL Server é iniciado

USE master;
GO

-- Criar banco de dados se não existir
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'FiapProjetoGames')
BEGIN
    CREATE DATABASE FiapProjetoGames;
END
GO

USE FiapProjetoGames;
GO

-- Verificar se as tabelas já existem
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuarios')
BEGIN
    -- As tabelas serão criadas automaticamente pelo Entity Framework
    -- Este script serve apenas para garantir que o banco existe
    PRINT 'Banco de dados FiapProjetoGames criado com sucesso!';
    PRINT 'As tabelas serão criadas automaticamente pelo Entity Framework.';
END
ELSE
BEGIN
    PRINT 'Banco de dados FiapProjetoGames já existe e está pronto para uso.';
END
GO 