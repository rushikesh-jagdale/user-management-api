namespace UserManagement.Application.Abstractions.Security;

public interface ICurrentUserService
{
    Guid UserId { get; }
}