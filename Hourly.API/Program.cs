using System.Text;
using Hourly.API.Extensions;
using Hourly.API.Middleware;
using Hourly.Application.Auth.Interfaces;
using Hourly.Application.Auth.Services;
using Hourly.Application.Common.Interfaces;
using Hourly.Application.Organizations.Interfaces;
using Hourly.Application.Organizations.Services;
using Hourly.Application.Users.Interfaces;
using Hourly.Application.Users.Services;
using Hourly.Core.Entities.Auth;
using Hourly.Core.Interfaces.Repositories;
using Hourly.Core.Interfaces.Services;
using Hourly.Infrastructure.Identity;
using Hourly.Infrastructure.Persistence;
using Hourly.Infrastructure.Persistence.Repositories;
using Hourly.Infrastructure.Storage;
using Hourly.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration de la base de données
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Hourly.Infrastructure")));

// Services d'authentification et autorisation
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});

// Enregistrement des services Core
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();

// Enregistrement des repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();

// Enregistrement des services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IDateTimeService, DateTimeService>();

// Configuration de la base de données et des services
builder.Services
    .AddDatabase(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddApplicationServices()
    .AddOpenApi()
    .AddCorsPolicy()
    .AddHttpContextAccessor();

// Configuration des controllers et de l'API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuration de Swagger avec authentification
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hourly API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// Configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configuration des middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middlewares personnalisés pour la gestion des exceptions et le logging
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed de données initiales si nécessaire
// await SeedData(app.Services);

app.Run();

// Fonction de seed si nécessaire
// async Task SeedData(IServiceProvider serviceProvider)
// {
//     using var scope = serviceProvider.CreateScope();
//     var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//     await context.Database.MigrateAsync();
//     // Ajouter ici la logique de seed des données initiales si nécessaire
// }