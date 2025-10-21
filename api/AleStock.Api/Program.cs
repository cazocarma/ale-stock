using System.Text;
using System.Text.Json.Serialization;
using AleStock.Api.Data;
using AleStock.Api.Models.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Configuración general
builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        options.SerializerSettings.Formatting = Formatting.Indented;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Base de datos
builder.Services.AddDbContext<AleStockDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSection);

var jwtSettings = jwtSection.Get<JwtSettings>();
var key = Encoding.UTF8.GetBytes(jwtSettings!.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AleStockDbContext>();

    // Intentar conexión varias veces antes de migrar
    var maxRetries = 10;
    var delay = TimeSpan.FromSeconds(3);
    var connected = false;

    for (var i = 0; i < maxRetries; i++)
    {
        try
        {
            db.Database.CanConnect();
            connected = true;
            break;
        }
        catch
        {
            Console.WriteLine($"[INIT] Esperando conexión a la base de datos... intento {i + 1}/{maxRetries}");
            Thread.Sleep(delay);
        }
    }

    if (!connected)
    {
        Console.WriteLine("[INIT] No se pudo conectar a la base de datos. Abortando inicialización.");
        throw new Exception("No se pudo conectar a PostgreSQL después de varios intentos.");
    }

    // Ejecutar migraciones y seeders
    db.Database.Migrate();
    DbInitializer.Seed(db);
}


// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
