-- ==============================================================
-- Script de Criação da Base de Dados e Tabelas para a Exo.WebApi
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

-- 3. Criação da Tabela de Usuários (Autenticação e Autorização)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Usuarios' AND xtype='U')
BEGIN
    CREATE TABLE Usuarios (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Email VARCHAR(150) NOT NULL,
        Senha VARCHAR(150) NOT NULL,
        Perfil VARCHAR(50) NOT NULL DEFAULT 'Admin'
    );
END
ELSE IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name = 'Perfil')
BEGIN
    ALTER TABLE Usuarios ADD Perfil VARCHAR(50) NOT NULL DEFAULT 'Admin';
END
GO

-- 4. Carga Inicial de Projetos (Seed Data)
IF NOT EXISTS (SELECT 1 FROM Projetos)
BEGIN
    INSERT INTO Projetos (NomeDoProjeto, Area, Status) VALUES
    ('Portal Educacional Conectado', 'Educação e Tecnologia', 1),
    ('Sistema de Logística Reversa Inteligente', 'Sustentabilidade', 1),
    ('Automação de Linha de Produção Industrial', 'Mecatrônica', 0),
    ('Monitoramento de Qualidade do Ar', 'Meio Ambiente', 1);
END
GO

-- 5. Carga Inicial de Usuários (Seed Data para Autenticação e Autorização)
IF NOT EXISTS (SELECT 1 FROM Usuarios)
BEGIN
    INSERT INTO Usuarios (Email, Senha, Perfil) VALUES
    ('admin@exoapi.com', 'admin123', 'Admin'),
    ('email@email.com', '1234', 'Admin'),
    ('usuario@exoapi.com', 'user123', 'Comum');
END
GO

-- 6. Consultas de Verificação
SELECT * FROM Projetos;
SELECT * FROM Usuarios;
GO
