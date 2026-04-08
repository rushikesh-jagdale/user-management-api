using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Api.Controllers;
using UserManagement.Application.Common;
using UserManagement.Application.Features.Users.Commands.CreateUser;
using UserManagement.Application.Features.Users.Commands.DeleteUser;
using UserManagement.Application.Features.Users.Commands.UpdateUser;
using UserManagement.Application.Features.Users.Queries.GetCurrentUser;
using UserManagement.Application.Features.Users.Queries.GetUserById;
using UserManagement.Application.Features.Users.Queries.GetUsers;

[ApiController]
[Route("api/users")]
[Authorize(Policy = "TenantAccess")]
[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status403Forbidden)]
public sealed class UsersController : BaseApiController
{
    public UsersController(IMediator mediator)
        : base(mediator)
    {
    }

    // =========================
    // ✅ CREATE USER
    // =========================
    [HttpPost]
    [Authorize(Policy = "Permission:create:user")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<string>.FailureResponse(result.Error));

        return CreatedAtAction(
            nameof(GetById),
            new { userId = result.Value },
            ApiResponse<object>.SuccessResponse(
                new { userId = result.Value },
                "User created successfully"));
    }

    // =========================
    // ✅ GET ALL USERS
    // =========================
    [HttpGet]
    [Authorize(Policy = "Permission:view:user")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(
            new GetUsersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            },
            cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<string>.FailureResponse(result.Error));

        return Ok(ApiResponse<object>.SuccessResponse(
            result.Value,
            "Users fetched successfully"));
    }

    // =========================
    // ✅ GET USER BY ID
    // =========================
    [HttpGet("{userId:guid}")]
    [Authorize(Policy = "UserCanViewOwnProfile")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetUserByIdQuery { UserId = userId },
            cancellationToken);

        if (result.IsFailure)
            return NotFound(ApiResponse<string>.FailureResponse(result.Error));

        return Ok(ApiResponse<object>.SuccessResponse(
            result.Value,
            "User fetched successfully"));
    }

    // =========================
    // ✅ UPDATE USER
    // =========================
    [HttpPut("{userId:guid}")]
    [Authorize(Policy = "Permission:update:user")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid userId,
        [FromBody] UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        command.Id = userId;

        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<string>.FailureResponse(result.Error));

        return Ok(ApiResponse<object>.SuccessResponse(
            new { userId },
            "User updated successfully"));
    }

    // =========================
    // ✅ DELETE USER
    // =========================
    [HttpDelete("{userId:guid}")]
    [Authorize(Policy = "Permission:delete:user")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new DeleteUserCommand { UserId = userId },
            cancellationToken);

        if (result.IsFailure)
            return NotFound(ApiResponse<string>.FailureResponse(result.Error));

        return Ok(ApiResponse<object>.SuccessResponse(
            new { userId },
            "User deleted successfully"));
    }

    // =========================
    // ✅ GET CURRENT USER
    // =========================
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser(
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetCurrentUserQuery(),
            cancellationToken);

        if (result.IsFailure)
            return Unauthorized(ApiResponse<string>.FailureResponse(result.Error));

        return Ok(ApiResponse<object>.SuccessResponse(
            result.Value,
            "Current user fetched successfully"));
    }
}