-- ==============================================================
-- Script de Criação da Base de Dados e Tabela para a Exo.WebApi
-- Curso Técnico de Desenvolvimento de Sistemas - SENAI
-- ==============================================================

-- 1. Criação do Banco de Dados
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ExoApi')
BEGIN
    CREATE DATABASE ExoApi;
END
GO

USE ExoApi;
GO

-- 2. Criação da Tabela de Projetos
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Projetos' AND xtype='U')
BEGIN
    CREATE TABLE Projetos (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        NomeDoProjeto VARCHAR(150) NOT NULL,
        Area VARCHAR(100) NOT NULL,
        Status BIT NOT NULL
    );
END
GO

-- 3. Carga Inicial de Dados (Seed Data) para Testes da API
IF NOT EXISTS (SELECT 1 FROM Projetos)
BEGIN
    INSERT INTO Projetos (NomeDoProjeto, Area, Status) VALUES
    ('Portal Educacional Conectado', 'Educação e Tecnologia', 1),
    ('Sistema de Logística Reversa Inteligente', 'Sustentabilidade', 1),
    ('Automação de Linha de Produção Industrial', 'Mecatrônica', 0),
    ('Monitoramento de Qualidade do Ar', 'Meio Ambiente', 1);
END
GO

-- 4. Consulta de Verificação
SELECT * FROM Projetos;
GO
