using MediatR;
using System.Text.Json.Serialization;
using UserManagement.Application.Common;

namespace UserManagement.Application.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommand : IRequest<Result<bool>>
{
    [JsonIgnore] // ✅ Hides from Swagger & request body
    public Guid Id { get; set; }
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
}