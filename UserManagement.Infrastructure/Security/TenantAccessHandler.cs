using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using UserManagement.Application.Abstractions.Tenancy;

namespace UserManagement.Infrastructure.Security;

public class TenantAccessHandler
    : AuthorizationHandler<TenantAccessRequirement>
{
    private readonly ITenantContext _tenantContext;

    public TenantAccessHandler(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantAccessRequirement requirement)
    {
        // ✅ Tenant from JWT
        var userTenantId = context.User.FindFirst("tenant_id")?.Value;

        if (string.IsNullOrEmpty(userTenantId))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // ✅ Tenant from Middleware
        var requestTenantId = _tenantContext.TenantId.ToString();

        if (string.IsNullOrEmpty(requestTenantId))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // ✅ Compare
        if (userTenantId == requestTenantId)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}