# 🚀 Exo.WebApi - Gerenciamento de Projetos

Projeto desenvolvido para a atividade prática do curso do **SENAI**, com foco na criação de uma API RESTful completa em **C# / ASP.NET Core** utilizando arquitetura em camadas e integração com banco de dados relacional **SQL Server** através do **Entity Framework Core**.

---

## 📌 Sumário
- [Sobre o Projeto](#-sobre-o-projeto)
- [Arquitetura em Camadas](#-arquitetura-em-camadas)
- [Tecnologias Utilizadas](#-tecnologias-utilizadas)
- [Configuração do Banco de Dados](#-configura%C3%A7%C3%A3o-do-banco-de-dados)
- [Como Executar o Projeto](#-como-executar-o-projeto)
- [Documentação dos Endpoints (CRUD)](#-documenta%C3%A7%C3%A3o-dos-endpoints-crud)
  - [1. Listar Projetos (GET)](#1-listar-projetos-get)
  - [2. Buscar Projeto por ID (GET)](#2-buscar-projeto-por-id-get)
  - [3. Cadastrar Projeto (POST)](#3-cadastrar-projeto-post)
  - [4. Atualizar Projeto (PUT)](#4-atualizar-projeto-put)
  - [5. Deletar Projeto (DELETE)](#5-deletar-projeto-delete)
- [Testes da API](#-testes-da-api)
- [Publicação no GitHub](#-publica%C3%A7%C3%A3o-no-github)

---

## 📖 Sobre o Projeto
O objetivo desta API é fornecer os recursos necessários para o gerenciamento completo do ciclo de vida de projetos institucionais, permitindo:
- **Consulta** da listagem geral ou busca individual por identificador;
- **Inclusão** de novos projetos com validações de dados;
- **Atualização** das informações de projetos existentes;
- **Exclusão** de projetos da base de dados.

---

## 🏗 Arquitetura em Camadas
O projeto foi estruturado seguindo as melhores práticas de desenvolvimento e separação de responsabilidades em camadas:

```
Projeto-ExoApi/
├── Controllers/
│   └── ProjetosController.cs       # Camada de Apresentação / Controladores REST
├── Interfaces/
│   └── IProjetoRepository.cs       # Camada de Contratos / Abstrações
├── Models/
│   └── Projeto.cs                  # Camada de Domínio / Entidades
├── Contexts/
│   └── ExoContext.cs               # Camada de Contexto de Dados (EF Core DbContext)
├── Repositories/
│   └── ProjetoRepository.cs        # Camada de Acesso a Dados / Persistência
├── Scripts/
│   └── ExoApi_Script.sql           # Script SQL Server de Criação e Carga de Dados
├── Properties/
│   └── launchSettings.json         # Configurações de Inicialização e Portas
├── appsettings.json                # Configuração de Conexão com o Banco
├── Exo.WebApi.csproj               # Arquivo de Dependências do Projeto .NET
├── Exo.WebApi.http                 # Coleção de Requisições HTTP para Testes
└── Program.cs                      # Ponto de Entrada, Injeção de Dependências e Pipeline
```

- **Interfaces**: Define os contratos (abstrações) para desacoplamento e inversão de controle.
- **Models**: Contém as entidades de dados do sistema, com anotações de validação (`Data Annotations`).
- **Contexts**: Configuração da sessão do Entity Framework Core (`DbContext`) e mapeamento relacional.
- **Repositories**: Encapsula as operações de banco de dados (CRUD), desacoplando a regra de negócio e persistência dos controladores.
- **Controllers**: Expõe os métodos HTTP (`GET`, `POST`, `PUT`, `DELETE`), realiza a validação das requisições e retorna os códigos de status HTTP semânticos (200, 201, 204, 400, 404).

---

## 🛠 Tecnologias Utilizadas
- **.NET 8 (LTS)** / **C# 12**
- **ASP.NET Core Web API**
- **Entity Framework Core 8** (`Microsoft.EntityFrameworkCore.SqlServer`, `Design`, `Tools`)
- **SQL Server / SQL Server Express**
- **Swagger / OpenAPI** (`Swashbuckle.AspNetCore`) para documentação interativa
- **CORS** habilitado para integração facilitada com frontends (React, Angular, Vue, etc.)

---

## 🗄 Configuração do Banco de Dados

### 1. Script SQL
O script de criação do banco e tabela está localizado em [`Scripts/ExoApi_Script.sql`](file:///c:/Users/Asus/Desktop/API_SENAI_NTERACAO/Projeto-ExoApi/Scripts/ExoApi_Script.sql).

Você pode executá-lo no **SQL Server Management Studio (SSMS)** ou via terminal:
```bash
sqlcmd -S "localhost\SQLEXPRESS" -C -i "Scripts/ExoApi_Script.sql"
```

### 2. String de Conexão
Definida no arquivo [`appsettings.json`](file:///c:/Users/Asus/Desktop/API_SENAI_NTERACAO/Projeto-ExoApi/appsettings.json):
```json
{
  "ConnectionStrings": {
    "ExoApiDatabase": "Server=localhost\\SQLEXPRESS;Database=ExoApi;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

## 🚀 Como Executar o Projeto

1. Clone o repositório ou acerte a pasta do projeto:
   ```bash
   git clone <URL_DO_SEU_REPOSITORIO>
   cd Projeto-ExoApi
   ```

2. Restaure as dependências do .NET:
   ```bash
   dotnet restore
   ```

3. Execute a API:
   ```bash
   dotnet run --launch-profile http
   ```

4. Acesse a documentação interativa do **Swagger** no navegador:
   ```
   http://localhost:5000/
   ```

---

## 📡 Documentação dos Endpoints (CRUD)

| Método | Endpoint | Descrição | Status Sucesso | Status Erro |
|---|---|---|---|---|
| **GET** | `/api/projetos` | Retorna todos os projetos | `200 OK` | `400 Bad Request` |
| **GET** | `/api/projetos/{id}` | Busca projeto por ID | `200 OK` | `404 Not Found` |
| **POST** | `/api/projetos` | Cadastra um novo projeto | `201 Created` | `400 Bad Request` |
| **PUT** | `/api/projetos/{id}` | Atualiza um projeto existente | `204 NoContent` | `404 Not Found` / `400 Bad Request` |
| **DELETE** | `/api/projetos/{id}` | Remove um projeto pelo ID | `204 NoContent` | `404 Not Found` / `400 Bad Request` |

---

### 1. Listar Projetos (GET)
- **Requisição:** `GET /api/projetos`
- **Resposta (`200 OK`):**
```json
[
  {
    "id": 1,
    "nomeDoProjeto": "Portal Educacional",
    "area": "Educação e Tecnologia",
    "status": true
  },
  {
    "id": 2,
    "nomeDoProjeto": "Sistema de Logística Reversa",
    "area": "Sustentabilidade",
    "status": true
  }
]
```

---

### 2. Buscar Projeto por ID (GET)
- **Requisição:** `GET /api/projetos/1`
- **Resposta (`200 OK`):**
```json
{
  "id": 1,
  "nomeDoProjeto": "Portal Educacional",
  "area": "Educação e Tecnologia",
  "status": true
}
```

---

### 3. Cadastrar Projeto (POST)
- **Requisição:** `POST /api/projetos`
- **Headers:** `Content-Type: application/json`
- **Corpo (Body):**
```json
{
  "nomeDoProjeto": "Plataforma de IA na Saúde",
  "area": "Saúde Digital",
  "status": true
}
```
- **Resposta (`201 Created`):**
```json
{
  "id": 4,
  "nomeDoProjeto": "Plataforma de IA na Saúde",
  "area": "Saúde Digital",
  "status": true
}
```

---

### 4. Atualizar Projeto (PUT)
- **Requisição:** `PUT /api/projetos/4`
- **Headers:** `Content-Type: application/json`
- **Corpo (Body):**
```json
{
  "id": 4,
  "nomeDoProjeto": "Plataforma de IA na Saúde Integrada",
  "area": "Saúde e Tecnologia",
  "status": false
}
```
- **Resposta (`204 No Content`)**

---

### 5. Deletar Projeto (DELETE)
- **Requisição:** `DELETE /api/projetos/4`
- **Resposta (`204 No Content`)**

---

## 🧪 Testes da API
Você pode testar a API através de:
1. **Swagger UI**: Acesse `http://localhost:5000/` diretamente no navegador.
2. **Arquivo HTTP**: O arquivo [`Exo.WebApi.http`](file:///c:/Users/Asus/Desktop/API_SENAI_NTERACAO/Projeto-ExoApi/Exo.WebApi.http) já contém todas as requisições prontas para execução com a extensão REST Client no VS Code.

---

## 🌐 Publicação no GitHub
Para publicar o projeto em seu GitHub:

1. Crie um novo repositório **público** no GitHub (ex: `Projeto-ExoApi`).
2. Vincule o repositório remoto e realize o push:
   ```bash
   git remote add origin https://github.com/<SEU_USUARIO>/Projeto-ExoApi.git
   git branch -M main
   git push -u origin main
   ```
3. Lembre-se de certificar-se de que a visibilidade do repositório está definida como **Pública** para avaliação!
