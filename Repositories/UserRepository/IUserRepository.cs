using LinkShorterAPI.Models;

namespace LinkShorterAPI.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User> RegisterAsync(User user);
        Task<User?> SignInAsync(string email, string password);
        Task<User?> GetUserProfileAsync(Guid userId);
        Task<User?> UpdateUserProfileAsync(Guid userId, User updatedUser, string? currentPassword = null);
        Task<bool> DeleteUserByIdAsync(Guid userId , string Password);
    }
}
