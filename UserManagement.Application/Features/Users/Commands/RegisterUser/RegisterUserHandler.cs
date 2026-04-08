using MediatR;
using UserManagement.Application.Abstractions.Security;
using UserManagement.Application.Common;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Features.Auth.Commands.RegisterUser;

public sealed class RegisterUserHandler
    : IRequestHandler<RegisterUserCommand, Result<Guid>> // ✅ FIXED
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<Guid>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = Guid.Parse("A6CB0542-3DE3-4A3C-8DC4-32BD9A93059C");

        // ✅ Check existing user
        var existingUser = await _userRepository
            .GetByEmailAsync(tenantId, request.Email, cancellationToken);

        if (existingUser is not null)
        {
            return Result<Guid>.Failure("User with this email already exists.");
        }

        var passwordHash = _passwordHasher.Hash(request.Password);

        // ✅ Create user
        var user = new User(
            tenantId,
            request.Email,
            passwordHash,
            request.FirstName,
            request.LastName);

        // ✅ Fetch role from DB (SAFE)
        var userRole = await _roleRepository
            .GetByNameAsync(tenantId, "User", cancellationToken);

        if (userRole is null)
        {
            return Result<Guid>.Failure("Default 'User' role not found.");
        }

        user.AssignRole(userRole.Id);

        await _userRepository.AddAsync(user, cancellationToken);

        return Result<Guid>.Success(user.Id);
    }
}