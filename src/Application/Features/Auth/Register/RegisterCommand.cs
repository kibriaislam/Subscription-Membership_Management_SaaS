using Application.DTOs.Auth;
using MediatR;

namespace Application.Features.Auth.Register;

public class RegisterCommand : IRequest<AuthResponseDto>
{
    public RegisterDto RegisterDto { get; set; } = null!;
}

