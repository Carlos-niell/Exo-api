using Exo.WebApi.Contexts;
using Exo.WebApi.Interfaces;
using Exo.WebApi.Models;
using Exo.WebApi.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configuração da string de conexão com o SQL Server
var connectionString = builder.Configuration.GetConnectionString("ExoApiDatabase")
    ?? "Server=localhost\\SQLEXPRESS;Database=ExoApi;Trusted_Connection=True;TrustServerCertificate=True;";

// 1. Injeção de Dependência do Contexto do Entity Framework Core
builder.Services.AddDbContext<ExoContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// 2. Injeção de Dependência da Camada de Repositório (Projetos e Usuários)
builder.Services.AddTransient<IProjetoRepository, ProjetoRepository>();
builder.Services.AddTransient<ProjetoRepository>();
builder.Services.AddTransient<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddTransient<UsuarioRepository>();

// 3. Configuração dos Controladores
builder.Services.AddControllers();

// 4. Configuração de Autenticação JWT (JSON Web Token)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
.AddJwtBearer("JwtBearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Valida quem está solicitando (Emissor)
        ValidateIssuer = true,
        // Valida quem está recebendo (Destinatário)
        ValidateAudience = true,
        // Define se o tempo de expiração será validado
        ValidateLifetime = true,
        // Valida a chave de assinatura do token
        ValidateIssuerSigningKey = true,
        // Chave de criptografia simétrica
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("exoapi-chave-autenticacao-senai-2026-segura")),
        // Tempo de tolerância para expiração do token
        ClockSkew = TimeSpan.FromMinutes(5),
        // Nome do issuer (de onde está vindo)
        ValidIssuer = "exoapi.webapi",
        // Nome do audience (para onde está indo)
        ValidAudience = "exoapi.webapi"
    };

    // Respostas padronizadas JSON para falhas de Autenticação (401) e Autorização (403)
    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var erro = new ApiErrorResponse
            {
                Status = StatusCodes.Status401Unauthorized,
                Erro = "Não Autenticado",
                Mensagem = "Acesso negado. É necessário informar um token JWT válido no cabeçalho Authorization (Bearer token) para acessar este endpoint."
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(erro, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        },
        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var erro = new ApiErrorResponse
            {
                Status = StatusCodes.Status403Forbidden,
                Erro = "Acesso Proibido (Sem Permissão)",
                Mensagem = "Usuário autenticado, porém sem permissão (perfil Admin requerido) para realizar a exclusão deste registro."
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(erro, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }
    };
});

// 5. Configuração de Autorização
builder.Services.AddAuthorization();

// 6. Configuração de CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 7. Configuração do Swagger/OpenAPI com suporte a Autenticação Bearer JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Exo.WebApi - SENAI",
        Version = "v1",
        Description = "API RESTful desenvolvida em camadas para gerenciamento de projetos com Autenticação e Autorização via JWT (ASP.NET Core e SQL Server)."
    });

    // Configuração do botão Authorize (JWT Bearer) no Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT gerado no endpoint de login (não precisa digitar 'Bearer ' antes do token quando usar este campo)."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Garantir que o banco de dados, as tabelas (Projetos e Usuarios) e os dados iniciais estejam criados na inicialização
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ExoContext>();
    try
    {
        context.Database.EnsureCreated();

        // Garante a criação da tabela Usuarios mesmo se o banco já existia antes desta atividade
        context.Database.ExecuteSqlRaw(@"
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
        ");

        // Carga inicial de usuários para testes imediatos de Autenticação e Autorização
        if (!context.Usuarios.Any())
        {
            context.Usuarios.AddRange(
                new Usuario { Email = "admin@exoapi.com", Senha = "admin123", Perfil = "Admin" },
                new Usuario { Email = "email@email.com", Senha = "1234", Perfil = "Admin" },
                new Usuario { Email = "usuario@exoapi.com", Senha = "user123", Perfil = "Comum" }
            );
            context.SaveChanges();
        }

        // Carga inicial de projetos caso a tabela esteja vazia
        if (!context.Projetos.Any())
        {
            context.Projetos.AddRange(
                new Projeto { NomeDoProjeto = "Portal Educacional Conectado", Area = "Educação e Tecnologia", Status = true },
                new Projeto { NomeDoProjeto = "Sistema de Logística Reversa Inteligente", Area = "Sustentabilidade", Status = true },
                new Projeto { NomeDoProjeto = "Automação de Linha de Produção Industrial", Area = "Mecatrônica", Status = false },
                new Projeto { NomeDoProjeto = "Monitoramento de Qualidade do Ar", Area = "Meio Ambiente", Status = true }
            );
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "Aviso ao verificar banco de dados durante a inicialização.");
    }
}

// Configuração do Pipeline HTTP
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExoApi v1");
    c.RoutePrefix = string.Empty; // Swagger abre diretamente na raiz: http://localhost:PORT/
});

app.UseHttpsRedirection();

app.UseCors("DefaultPolicy");

// Habilita a Autenticação (identificação do usuário via Token JWT)
app.UseAuthentication();

// Habilita a Autorização (verificação de permissões / [Authorize])
app.UseAuthorization();

app.MapControllers();

app.Run();
