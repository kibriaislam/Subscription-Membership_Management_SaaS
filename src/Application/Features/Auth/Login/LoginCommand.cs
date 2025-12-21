using Application.DTOs.Auth;
using MediatR;

namespace Application.Features.Auth.Login;

public class LoginCommand : IRequest<AuthResponseDto>
{
    public LoginDto LoginDto { get; set; } = null!;
}

