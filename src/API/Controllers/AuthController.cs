using Application.DTOs.Auth;
using Application.Features.Auth.Login;
using Application.Features.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto, CancellationToken cancellationToken)
    {
        var command = new RegisterCommand { RegisterDto = registerDto };
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
    {
        var command = new LoginCommand { LoginDto = loginDto };
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}

