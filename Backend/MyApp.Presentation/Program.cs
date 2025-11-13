using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyApp.Application;
using MyApp.Application.DTOs.Common;
using MyApp.Infrastructure;
using MyApp.Infrastructure.Context;
using MyApp.Presentation.MiddlewaresAndFilters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MyApp.Shared.DTOs;
using System.Net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

#region 🔧 Configuración base
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
#endregion

#region 🗄️ Configuración de cadena de conexión
string? connectionString = null;

if (builder.Environment.IsDevelopment())
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}
else
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

    if (!string.IsNullOrWhiteSpace(databaseUrl))
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        connectionString =
            $"Host={uri.Host};Port={uri.Port};Username={userInfo[0]};Password={userInfo[1]};" +
            $"Database={uri.AbsolutePath.TrimStart('/')};SSL Mode=Require;Trust Server Certificate=true";
    }
}

if (string.IsNullOrWhiteSpace(connectionString)) throw new InvalidOperationException("No se encontró la cadena de conexión a la base de datos.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
#endregion

#region 🧩 Servicios principales
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
#endregion

#region 🔐 Configuración de autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var secretKey = builder.Configuration["JwtSettings:SecretKey"];
    if (string.IsNullOrEmpty(secretKey))
        throw new InvalidOperationException("No se encontró la clave secreta JWT.");

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };

    options.Events = new JwtBearerEvents
    {
        // Manejo de errores cuando el token es inválido o expiró
        OnAuthenticationFailed = async context =>
        {
            context.NoResult(); // Evita que el pipeline siga
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";

            var message = context.Exception is SecurityTokenExpiredException
                ? "El token ha expirado. Por favor, inicia sesión nuevamente."
                : "Token inválido. El token proporcionado no es válido o ha sido manipulado.";

            var response = new ErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                ErrorMessage = message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        },

        // Manejo de respuesta cuando no se envía ningún token
        OnChallenge = async context =>
        {
            context.HandleResponse(); // Evita que ASP.NET genere su propia respuesta

            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = "application/json";

                var response = new ErrorResponse
                {
                    StatusCode = 401,
                    ErrorMessage = "Acceso no autorizado. Debes iniciar sesión para obtener un token válido."
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    };
});
#endregion

#region 🧱 Inyección de dependencias por capas
builder.Services.AddInfrastructureDependencies();
builder.Services.AddApplicationUseCasesDependencies();
#endregion

#region ⚙️ Configuración adicional
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("HasPermission", policy =>
        policy.Requirements.Add(new PermissionRequirement()));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
#endregion

#region 📘 Swagger con autenticación JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Clean arquitecture API",
        Version = "v1",
        Description = "Esta API sigue los principios de la arquitectura limpia, " +
        "separando responsabilidades en capas y promoviendo la mantenibilidad, " +
        "escalabilidad y testabilidad. Incluye autenticación JWT y autorización para proteger los recursos."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingrese un token válido en el formato Bearer {token}",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    // Registra el filtro personalizado para rutas públicas
    // Si son públicas, elimina la necesidad de un token. Si no, agrega el esquema de seguridad.
    options.OperationFilter<PublicFilter>();
});
#endregion

#region 🚀 Construcción y ejecución
builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(5229));

var app = builder.Build();

app.UseCors("AllowAll");

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clean arquitecture API");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
#endregion