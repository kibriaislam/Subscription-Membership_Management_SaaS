using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IBusinessRepository businessRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _businessRepository = businessRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var dto = request.LoginDto;

        var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (user == null || !_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var business = await _businessRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (business == null)
        {
            throw new InvalidOperationException("Business not found for user");
        }

        var token = _jwtService.GenerateToken(user, business);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = _jwtService.GetTokenExpiry(),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BusinessId = business.Id,
                BusinessName = business.Name
            }
        };
    }
}

