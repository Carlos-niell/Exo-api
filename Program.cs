using Exo.WebApi.Contexts;
using Exo.WebApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração da string de conexão com o SQL Server
var connectionString = builder.Configuration.GetConnectionString("ExoApiDatabase") 
    ?? "Server=localhost\\SQLEXPRESS;Database=ExoApi;Trusted_Connection=True;TrustServerCertificate=True;";

// 1. Injeção de Dependência do Contexto do Entity Framework Core
builder.Services.AddDbContext<ExoContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// 2. Injeção de Dependência da Camada de Repositório
builder.Services.AddTransient<ProjetoRepository>();

// 3. Configuração dos Controladores
builder.Services.AddControllers();

// 4. Configuração de CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 5. Configuração do Swagger/OpenAPI para documentação e testes
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Exo.WebApi - SENAI",
        Version = "v1",
        Description = "API RESTful desenvolvida em camadas para gerenciamento de projetos com ASP.NET Core e SQL Server."
    });
});

var app = builder.Build();

// Garantir que o banco de dados e as tabelas estejam criados na inicialização
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ExoContext>();
    try
    {
        context.Database.EnsureCreated();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
