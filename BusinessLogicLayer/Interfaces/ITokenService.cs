using ModelLayer.Entities;

namespace BusinessLogicLayer.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}