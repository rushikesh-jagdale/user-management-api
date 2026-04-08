using MediatR;
using UserManagement.Application.Abstractions.Persistence;
using UserManagement.Application.Abstractions.Security;
using UserManagement.Application.Abstractions.Tenancy;
using UserManagement.Application.Common;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserHandler
    : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<Guid>> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;

        var existingUser = await _userRepository.GetByEmailAsync(
            tenantId,
            command.Email,
            cancellationToken);

        if (existingUser is not null)
        {
            return Result<Guid>.Failure(
                "User with this email already exists.");
        }

        var passwordHash = _passwordHasher.Hash(command.Password);

        var user = new User(
            tenantId,
            command.Email,
            passwordHash,
            command.FirstName,
            command.LastName);

        // =========================
        // ✅ Assign Default Role (FIXED)
        // =========================
        var defaultRole = await _roleRepository.GetByNameAsync(
            tenantId,
            "User",
            cancellationToken);

        if (defaultRole is null)
        {
            return Result<Guid>.Failure("Default role 'User' not found.");
        }

        // ✅ Use domain method (IMPORTANT)
        user.AssignRole(defaultRole.Id);

        // =========================

        await _userRepository.AddAsync(user, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(user.Id);
    }
}