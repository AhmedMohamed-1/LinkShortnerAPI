using LinkShorterAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkShorterAPI.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly LinkShortnerContext _context;
        public UserRepository(LinkShortnerContext context)
        {
            _context = context;
        }
        public async Task<User?> GetUserProfileAsync(Guid userId)
        {
            var user =  await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            return user;
        }

        public async Task<User> RegisterAsync(User user)
        {
            // The PasswordHash field already contains the plain text password from AutoMapper
            // We need to hash it before saving
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true; // Assuming new users are active by default
            user.LastLogin = null; // Set LastLogin to null for new users

            // Ensure the email is unique
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (existingUser != null) {
                throw new InvalidOperationException("User with this email already exists.");
            }
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> SignInAsync(string email, string password)
        {
            // Find the user by email
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (user is null)
                return null;

            // Compare password (BCrypt handles hashing and salt internally)
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordValid)
                return null;

            return user;
        }

        public async Task<User?> UpdateUserProfileAsync(Guid userId, User updatedUser, string? currentPassword = null)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user is null)
                return null;

            // Update properties (only those allowed)
            user.FullName = updatedUser.FullName;

            // If password is being updated, verify current password first
            if (!string.IsNullOrEmpty(updatedUser.PasswordHash) && !string.IsNullOrEmpty(currentPassword))
            {
                // Verify current password
                bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash);
                if (!isCurrentPasswordValid)
                    throw new InvalidOperationException("Current password doesn't match.");

                // Hash new password and update
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatedUser.PasswordHash);
            }

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserByIdAsync(Guid userId , string Password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
                return false;

            // Verify password before deletion
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash);
            if (!isPasswordValid)
                return false;

            // Optionally, you can also delete related entities if needed
            // For example, delete all short links created by this user
            _context.ShortLinks.RemoveRange(_context.ShortLinks.Where(sl => sl.CreatedBy == userId));
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
