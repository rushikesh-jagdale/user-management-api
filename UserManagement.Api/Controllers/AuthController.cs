using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Common;
using UserManagement.Application.Features.Auth.Commands.Login;
using UserManagement.Application.Features.Auth.Commands.Logout;
using UserManagement.Application.Features.Auth.Commands.Refresh;
using UserManagement.Application.Features.Auth.Commands.RegisterUser;

namespace UserManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : BaseApiController
{
    public AuthController(IMediator mediator)
        : base(mediator)
    {
    }

    // =========================
    // ✅ REGISTER
    // =========================
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
    [FromBody] RegisterUserCommand command,
    CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<string>.FailureResponse(result.Error));

        return Created(
            $"api/users/{result.Value}",
            ApiResponse<object>.SuccessResponse(
                new { userId = result.Value },
                "User registered successfully"));
    }

    // =========================
    // ✅ LOGIN
    // =========================
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return Unauthorized(ApiResponse<string>.FailureResponse(result.Error));

        return Ok(ApiResponse<object>.SuccessResponse(
            result.Value,
            "Login successful"));
    }

    // =========================
    // ✅ REFRESH TOKEN
    // =========================
    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return Unauthorized(ApiResponse<string>.FailureResponse(result.Error));

        return Ok(ApiResponse<object>.SuccessResponse(
            result.Value,
            "Token refreshed successfully"));
    }

    // =========================
    // ✅ LOGOUT
    // =========================
    [AllowAnonymous]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<string>.FailureResponse(result.Error));

        return Ok(ApiResponse<string>.SuccessResponse(
            string.Empty,
            "Logout successful"));
    }
}