using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Auth.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IBusinessRepository businessRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _businessRepository = businessRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var dto = request.RegisterDto;

        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(dto.Email, cancellationToken))
        {
            throw new InvalidOperationException("Email already registered");
        }

        // Create user
        var user = new User
        {
            Email = dto.Email,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Role = "Owner"
        };

        await _userRepository.AddAsync(user, cancellationToken);

        // Create business
        var business = new Business
        {
            UserId = user.Id,
            Name = dto.BusinessName,
            Currency = dto.Currency
        };

        await _businessRepository.AddAsync(business, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate token
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

