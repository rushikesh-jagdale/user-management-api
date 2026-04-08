using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UserManagement.Application.Abstractions.Security;

namespace UserManagement.Infrastructure.Security;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userId = _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst("user_id")?.Value;

            return Guid.TryParse(userId, out var id)
                ? id
                : Guid.Empty;
        }
    }
}