using FluentValidation;

namespace UserManagement.Application.Features.Users.Commands.DeleteUser;

public sealed class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}