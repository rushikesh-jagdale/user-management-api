using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;
using System.Text;
using UserManagement.Api.Middleware;
using UserManagement.Application;
using UserManagement.Application.Abstractions.Tenancy;
using UserManagement.Infrastructure;
using UserManagement.Infrastructure.Persistence;
using UserManagement.Infrastructure.Persistence.DbContext;
using UserManagement.Infrastructure.Security;
using UserManagement.Infrastructure.Tenancy;

// =========================
// 🔥 SERILOG CONFIGURATION
// =========================
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// ✅ Attach Serilog
builder.Host.UseSerilog();

// =========================
// Controllers
// =========================
builder.Services.AddControllers();

// =========================
// ✅ HEALTH CHECKS
// =========================
builder.Services.AddHealthChecks();

// =========================
// ✅ RATE LIMITING
// =========================
builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            success = false,
            message = "Too many requests. Please try again later."
        }, token);
    };

    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 100;
        opt.QueueLimit = 0;
    });
});

// =========================
// Tenant + Authorization
// =========================
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<IAuthorizationHandler, TenantAccessHandler>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

// =========================
// Layers
// =========================
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// =========================
// HTTP Context
// =========================
builder.Services.AddHttpContextAccessor();

// =========================
// 🔐 Authentication (JWT)
// =========================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

            RoleClaimType = ClaimTypes.Role,
            NameClaimType = "sub",
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var tokenType = context.Principal?
                    .FindFirst("token_type")?.Value;

                if (tokenType != "access")
                {
                    context.Fail("Invalid token type");
                }

                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                return context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "Unauthorized"
                });
            },

            OnForbidden = context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";

                return context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "Forbidden"
                });
            }
        };
    });

// =========================
// 🔐 Authorization
// =========================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TenantAccess", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new TenantAccessRequirement());
    });

    options.AddPolicy("UserCanViewOwnProfile", policy =>
    {
        policy.RequireAssertion(context =>
        {
            if (context.User.IsInRole("Admin"))
                return true;

            var userIdClaim = context.User.FindFirst("user_id")?.Value;

            if (context.Resource is not HttpContext httpContext)
                return false;

            var routeUserId = httpContext.Request.RouteValues["userId"]?.ToString();

            return !string.IsNullOrEmpty(userIdClaim)
                && !string.IsNullOrEmpty(routeUserId)
                && string.Equals(userIdClaim, routeUserId, StringComparison.OrdinalIgnoreCase);
        });
    });
});

// =========================
// API Versioning
// =========================
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// =========================
// Swagger
// =========================
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "User Management API",
        Version = "v1"
    });

    options.OperationFilter<UserManagement.Api.Swagger.TenantHeaderOperationFilter>();

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
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

// =========================
// 🚀 PIPELINE
// =========================
var app = builder.Build();

// ✅ 🔥 IMPORTANT: Render Port Binding
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

// ✅ Database Seeder
try
{
    await DbInitializer.SeedAsync(app.Services);
}
catch (Exception ex)
{
    Log.Error(ex, "Database seeding failed");
}

// ✅ 🔥 Swagger ENABLED FOR PRODUCTION
app.UseSwagger();
app.UseSwaggerUI();

// Logging
app.UseSerilogRequestLogging();

// Middlewares
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<TenantMiddleware>();

// Rate limiter
app.UseRateLimiter();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers().RequireRateLimiting("fixed");

// Health
app.MapHealthChecks("/health");

app.Run();