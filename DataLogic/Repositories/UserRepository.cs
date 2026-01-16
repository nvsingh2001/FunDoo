using DataLogic.Data;
using DataLogic.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Entities;

namespace DataLogic.Repositories;

public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{

    public async Task<User> CreateUserAsync(User user)
    {
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() ==  email.ToLower());
    }

    public async Task<bool> UserExistsAsync(int userId)
    {
        return await dbContext.Users.AnyAsync(u => u.UserId == userId);
    }
    
    public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
    {
        var query = dbContext.Users
            .Where(u => u.Email.ToLower() == email.ToLower());

        if (excludeUserId is not null)
            query = query.Where(u => u.UserId != excludeUserId.Value);

        return await query.AnyAsync();
    }
}