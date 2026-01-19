using AutoMapper;
using BusinessLogicLayer.Interfaces;
using DataLogic.Interfaces;
using ModelLayer.DTOs;
using ModelLayer.Entities;
using static BCrypt.Net.BCrypt;

namespace BusinessLogicLayer.Services;

public class UserServices(IUserRepository userRepository, IMapper mapper,  ITokenService tokenService, IEmailService emailService): IUserService
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

        var message = $"Hello {user.FirstName}!\nWelcome to FunDoo App!";
        
        await  emailService.SendEmailAsync(user.Email, "FunDoo App", message);
        
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

    public async Task ForgotPassword(string email)
    {
        var user = await userRepository.GetUserByEmailAsync(email);
        
        if(user is null)
            throw new InvalidOperationException($"User with email:{email} does not exist");
        
        var jwtToken = tokenService.GenerateResetToken(user);

        var message = $"Click here to reset: http://localhost:7196/reset-password?token={jwtToken}";
        
        await emailService.SendEmailAsync(email, "Reset Password", message);
    }

    public async Task ResetPassword(int userId, string newPassword)
    {
        var user = await userRepository.GetUserByIdAsync(userId);
        
        if (user is null)
            throw new InvalidOperationException($"User with id:{userId} does not exist");
        
        var hashedPassword = EnhancedHashPassword(newPassword);
        user.Password = hashedPassword;
        
        await userRepository.UpdateUserAsync(user);
    }
}