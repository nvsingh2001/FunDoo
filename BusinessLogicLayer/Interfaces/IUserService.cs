using ModelLayer.DTOs;
using ModelLayer.Utilities;

namespace BusinessLogicLayer.Interfaces;

public interface IUserService
{
    Task<UserResponseDto> RegisterUserAsync(UserRequestDto userDto);
    Task<LoginResponseDto> LoginUserAsync(LoginRequestDto loginDto);
    Task<UserResponseDto> GetUserByIdAsync(int userId);
}
