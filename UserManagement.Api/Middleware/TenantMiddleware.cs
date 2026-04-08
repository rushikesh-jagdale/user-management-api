using UserManagement.Application.Abstractions.Tenancy;

namespace UserManagement.Api.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITenantContext tenantContext)
    {
        var tenantIdHeader = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();

        if (Guid.TryParse(tenantIdHeader, out var tenantId))
        {
            tenantContext.SetTenantId(tenantId);
        }
        else
        {
            // Optional: fail fast
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Invalid or missing TenantId");
            return;
        }

        await _next(context);
    }
}