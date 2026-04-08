using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using UserManagement.Application.Security;

namespace UserManagement.Infrastructure.Security;

public sealed class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
      AuthorizationHandlerContext context,
      PermissionRequirement requirement)
    {
        var permissions = context.User.FindAll("permission")
                                      .Select(c => c.Value)
                                      .ToList();

        Console.WriteLine("USER PERMISSIONS:");
        foreach (var p in permissions)
        {
            Console.WriteLine(p);
        }

        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}