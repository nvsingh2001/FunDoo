using ModelLayer.Entities;

namespace DataLogic.Interfaces;

public interface IUserRepository
{
    Task<User> CreateUserAsync(User user);
    
    Task<User> GetUserByIdAsync(int userId);

    Task<User> GetUserByEmailAsync(string email);
    
    Task<User> UpdateUserAsync(User user);
    
    Task<bool> UserExistsAsync(int userId);
    
    Task<bool> EmailExistsAsync(string email,int? excludeUserId = null);
}