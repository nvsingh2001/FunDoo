using AutoMapper;
using BusinessLogicLayer.Interfaces;
using DataLogic.Interfaces;
using ModelLayer.DTOs;
using ModelLayer.Entities;
using static BCrypt.Net.BCrypt;

namespace BusinessLogicLayer.Services;

public class UserServices(IUserRepository userRepository, IMapper mapper,  ITokenService tokenService): IUserService
{
    public async Task<UserResponseDto> RegisterUserAsync(UserRequestDto userDto)
    {
        var existingUser = await userRepository.GetUserByEmailAsync(userDto.Email);
        
        if (existingUser != null)
            throw new InvalidOperationException("Email already exists");
        
        var hashedPassword = EnhancedHashPassword(userDto.Password);

        var user = new User()
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email,
            Password = hashedPassword,
        };
        
        var result = await userRepository.CreateUserAsync(user);
        
        return mapper.Map<UserResponseDto>(result);
    }

    public async Task<LoginResponseDto> LoginUserAsync(LoginRequestDto loginDto)
    {
        var user = await userRepository.GetUserByEmailAsync(loginDto.Email);
        
        if(user is null)
            throw new InvalidOperationException($"User with email:{loginDto.Email} does not exist");

        if (!EnhancedVerify(loginDto.Password, user.Password))
            throw new InvalidOperationException("Invalid Password");
        
        var jwtToken = tokenService.GenerateToken(user);

        return new LoginResponseDto()
        {
            Token = jwtToken,
            Email = user.Email,
            UserId = user.UserId
        };
    }

    public async Task<UserResponseDto> GetUserByIdAsync(int userId)
    {
        var user = await userRepository.GetUserByIdAsync(userId);
        
        return user is null ? 
            throw new InvalidOperationException($"User with id:{userId} does not exist") 
            : mapper.Map<UserResponseDto>(user);
    }
}