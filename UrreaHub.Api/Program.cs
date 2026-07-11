using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Auth;
using UrreaHub.Application.Requisiciones;
using UrreaHub.Infrastructure;
using UrreaHub.Infrastructure.Persistence;
using UrreaHub.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("UrreaHubDb")
    ?? throw new InvalidOperationException("Connection string 'UrreaHubDb' not found.");

builder.Services.AddInfrastructure(builder.Configuration, connectionString);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "URREA Hub API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        var raw = builder.Configuration["Frontend:Url"] ?? "http://localhost:3000";
        // Supports multiple origins separated by commas: "https://app.vercel.app,http://localhost:3000"
        var origins = raw.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UrreaHubDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");

    for (var attempt = 1; attempt <= 10; attempt++)
    {
        try
        {
            await db.Database.MigrateAsync();
            break;
        }
        catch (Exception ex) when (attempt < 10)
        {
            logger.LogWarning(ex, "MigrateAsync intento {Attempt}/10 — reintentando en 3s…", attempt);
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
    }

    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await DbSeeder.SeedAsync(db, hasher);
    await SecuritySeed.SeedAsync(db);
    await CatalogSeed.SeedAsync(db);
    await TiPlatformSeed.SeedAsync(db);
    await PermisosCatalog.UpsertTiposAsync(db);
    await AttendanceSeed.SeedAsync(db);
    await OnboardingSeed.SeedAsync(db);
    await CompensacionSeed.SeedAsync(db);
    var requisitionAdmin = scope.ServiceProvider.GetRequiredService<IRequisitionAdminService>();
    await TalentAcquisitionSeed.SeedAsync(db, requisitionAdmin);
    await EquipoDemoSeed.SeedAsync(db);
    await PortalDevSeed.SeedAsync(db);
}

app.Run();
