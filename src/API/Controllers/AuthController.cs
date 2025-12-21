using Application.DTOs.Auth;
using Application.Features.Auth.Login;
using Application.Features.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Authentication controller for user registration and login operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Authentication")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user and creates their business profile.
    /// </summary>
    /// <remarks>
    /// This endpoint creates a new user account along with an associated business profile.
    /// Upon successful registration, a JWT token is returned that can be used for authenticated requests.
    /// The email address must be unique across the system.
    /// </remarks>
    /// <param name="registerDto">The registration data containing user and business information.</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 200 OK response with authentication token and user details on success,
    /// or a 400 Bad Request response if validation fails or email already exists.
    /// </returns>
    [HttpPost("register")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto, CancellationToken cancellationToken)
    {
        var command = new RegisterCommand { RegisterDto = registerDto };
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <remarks>
    /// This endpoint validates user credentials and returns a JWT token upon successful authentication.
    /// The token should be included in the Authorization header for subsequent API requests.
    /// Token format: "Bearer {token}"
    /// </remarks>
    /// <param name="loginDto">The login credentials containing email and password.</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 200 OK response with authentication token and user details on success,
    /// or a 401 Unauthorized response if credentials are invalid.
    /// </returns>
    [HttpPost("login")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
    {
        var command = new LoginCommand { LoginDto = loginDto };
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}

