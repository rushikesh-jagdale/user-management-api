using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Application.Abstractions.Persistence;
using UserManagement.Application.Abstractions.Security;
using UserManagement.Application.Abstractions.Tenancy;
using UserManagement.Domain.Interfaces;
using UserManagement.Infrastructure.Persistence;
using UserManagement.Infrastructure.Persistence.DbContext;
using UserManagement.Infrastructure.Persistence.Repositories;
using UserManagement.Infrastructure.Security;
using UserManagement.Infrastructure.Tenancy;

namespace UserManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<UserManagementDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IPermissionRepository, PermissionRepository>();

        // ✅ CORRECT: single scoped registration
        services.AddScoped<ITenantContext, TenantContext>();

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();


        return services;
    }
}
