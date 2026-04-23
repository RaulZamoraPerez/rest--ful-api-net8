
using System.Text;
using ApiEcommerce.Constants;
using ApiEcommerce.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// �️ CLEAN ARCHITECTURE IMPORTS
using ApiEcommerce.Application.Interfaces;
using ApiEcommerce.Infrastructure.Persistence;
using ApiEcommerce.Infrastructure.Services;
using ApiEcommerce.Application.UseCases.Categories;
using ApiEcommerce.Application.UseCases.Products;
using ApiEcommerce.Application.UseCases.Auth;

var builder = WebApplication.CreateBuilder(args);

var dbConnectionString = builder.Configuration.GetConnectionString("ConexionSql");

#region Response Caching

builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024;
    options.UseCaseSensitivePaths = true;
});

#endregion

#region Database Configuration

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(dbConnectionString));

#endregion

#region Clean Architecture - Dependency Injection

// 🏗️ INFRASTRUCTURE - Repositorios Clean Architecture
builder.Services.AddScoped<ApiEcommerce.Application.Interfaces.ICategoryRepository, ApiEcommerce.Infrastructure.Persistence.CategoryRepository>();
builder.Services.AddScoped<ApiEcommerce.Application.Interfaces.IProductRepository, ApiEcommerce.Infrastructure.Persistence.ProductRepository>();
builder.Services.AddScoped<ApiEcommerce.Application.Interfaces.IUserRepository, ApiEcommerce.Infrastructure.Persistence.UserRepository>();

// ⚡ APPLICATION - Use Cases para Categorías
builder.Services.AddScoped<GetAllCategoriesUseCase>();
builder.Services.AddScoped<CreateCategoryUseCase>();
builder.Services.AddScoped<DeleteCategoryUseCase>();
builder.Services.AddScoped<GetCategoryByIdUseCase>();

// ⚡ APPLICATION - Use Cases para Productos
builder.Services.AddScoped<GetAllProductsUseCase>();
builder.Services.AddScoped<GetProductByIdUseCase>();
builder.Services.AddScoped<CreateProductUseCase>();
builder.Services.AddScoped<UpdateProductUseCase>();
builder.Services.AddScoped<UpdateProductStockUseCase>();
builder.Services.AddScoped<DeleteProductUseCase>();
builder.Services.AddScoped<GetProductsPagedUseCase>();

// ⚡ APPLICATION - Use Cases para Autenticación
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<RegisterUseCase>();

// 🏗️ INFRASTRUCTURE - Servicios Clean Architecture
builder.Services.AddScoped<IIdentityService, IdentityService>();

#endregion

#region AutoMapper Configuration

builder.Services.AddAutoMapper(cfg =>
{
    // Escanea todos los perfiles en el ensamblado
    cfg.AddMaps(typeof(Program).Assembly);
});

#endregion

#region Identity Configuration

builder.Services.AddIdentity<AplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configurar políticas de contraseña
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;
});

#endregion

#region JWT Authentication Configuration

var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");
if (string.IsNullOrEmpty(secretKey))
{
    throw new Exception("Secret key is not configured.");
}

// Obtener valores JWT con fallbacks
var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? "ApiEcommerce";
var jwtAudience = builder.Configuration["JWT:Audience"] ?? "ApiEcommerceUsers";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

#endregion
#region API Versioning

builder.Services.AddApiVersioning(option =>
{
    option.AssumeDefaultVersionWhenUnspecified = true;
    option.DefaultApiVersion = new ApiVersion(1, 0);
    option.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

#endregion

#region Controllers

builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add(CacheProfiles.Default10, CacheProfiles.Profile10);
    options.CacheProfiles.Add(CacheProfiles.Default20, CacheProfiles.Profile20);
});

#endregion

#region Swagger Configuration

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Clean Architecture API utiliza JWT. Ingresa: Bearer {tu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Clean Architecture Ecommerce API v1",
        Description = "API con Clean Architecture para gestionar productos y usuarios",
        Contact = new OpenApiContact
        {
            Name = "Clean Architecture Team",
            Email = "clean@architecture.com"
        }
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2",
        Title = "Clean Architecture Ecommerce API v2",
        Description = "API avanzada con Clean Architecture",
        Contact = new OpenApiContact
        {
            Name = "Clean Architecture Team",
            Email = "clean@architecture.com"
        }
    });
});

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy(PolicyNames.AllowSpecificOrigin, builder =>
    {
        builder.WithOrigins("http://localhost:3000", "http://localhost:5173")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

#endregion

// 🏗️ BUILD THE APP
var app = builder.Build();

#region Development Configuration

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clean Architecture API V1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Clean Architecture API V2");
        c.RoutePrefix = string.Empty; // Para que Swagger sea la página principal
    });
}

#endregion

#region Middleware Pipeline

// CORS debe ir antes de Authentication
app.UseCors(PolicyNames.AllowSpecificOrigin);

// Response caching
app.UseResponseCaching();

// HTTPS redirection
app.UseHttpsRedirection();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Static files
app.UseStaticFiles();

// Controllers
app.MapControllers();

#endregion

#region Database Seeding (Opcional)

// 🌱 SEED DATA PARA DEVELOPMENT
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        
        await SeedData.InitializeAsync(userManager, roleManager);
    }
}

#endregion

app.Run();

#region Seed Data Class

public static class SeedData
{
    public static async Task InitializeAsync(UserManager<AplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Crear roles si no existen
        string[] roles = { "Admin", "User" };
        
        foreach (string role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Crear usuario admin si no existe
        if (await userManager.FindByEmailAsync("admin@clean.com") == null)
        {
            var adminUser = new AplicationUser
            {
                UserName = "admin@clean.com",
                Email = "admin@clean.com",
                Name = "Administrator"
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}

#endregion
