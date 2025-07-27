using AutoMapper;
using LinkShorterAPI.DTOs.UserDTO;
using LinkShorterAPI.Models;

namespace LinkShorterAPI.Services.UserService
{
    public interface IUserService
    {
        Task<SignInSignUpResponse> RegisterAsync(SignUpDTO user);
        Task<SignInSignUpResponse?> SignInAsync(SignInDTO user);
        Task<GetUserDTO?> GetUserProfileAsync(Guid userId);
        Task<GetUserDTO?> UpdateUserProfileAsync(Guid userId, UpdateUserDTO updatedUser);
        Task<bool> DeleteUserByIdAsync(Guid userId, DeleteUserDTO deleteUser);
    }
}
