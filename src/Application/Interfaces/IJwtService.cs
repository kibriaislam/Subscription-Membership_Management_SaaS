using Domain.Entities;

namespace Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user, Business business);
    DateTime GetTokenExpiry();
}

