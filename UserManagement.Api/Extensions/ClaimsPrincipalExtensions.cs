using System.Security.Claims;

namespace UserManagement.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetTenantId(this ClaimsPrincipal user)
    {
        var tenantId = user.FindFirst("tenant_id")?.Value;

        if (tenantId is null)
            throw new UnauthorizedAccessException("Tenant not found in token.");

        return Guid.Parse(tenantId);
    }
}
