using AutoMapper;
using LinkShorterAPI.Authentication.Token;
using LinkShorterAPI.DTOs.UserDTO;
using LinkShorterAPI.Models;
using LinkShorterAPI.Repositories.UserRepository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LinkShorterAPI.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;
        
        public UserService(IMapper mapper, ILogger<UserService> logger, IUserRepository userRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<SignInSignUpResponse> RegisterAsync(SignUpDTO user)
        {
            _logger.LogInformation("Starting user registration for email: {Email}", user.Email);
            
            try
            {
                var userModel = _mapper.Map<User>(user);
                _logger.LogDebug("Mapped SignUpDTO to User model for email: {Email}", user.Email);

                var createdUser = await _userRepository.RegisterAsync(userModel);
                _logger.LogInformation("Successfully registered user with ID: {UserId} for email: {Email}", 
                    createdUser.Id, createdUser.Email);

                var response = _mapper.Map<SignInSignUpResponse>(createdUser);
                _logger.LogDebug("Mapped User to SignInSignUpResponse for user ID: {UserId}", createdUser.Id);

                return response;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Registration failed for email {Email}: {Message}", user.Email, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during user registration for email: {Email}", user.Email);
                throw;
            }
        }

        public async Task<SignInSignUpResponse?> SignInAsync(SignInDTO user)
        {
            _logger.LogInformation("Starting sign-in process for email: {Email}", user.Email);
            
            try
            {
                var userModel = await _userRepository.SignInAsync(user.Email, user.Password);
                
                if (userModel is null)
                {
                    _logger.LogWarning("Invalid login attempt for email: {Email}", user.Email);
                    return null;
                }

                _logger.LogInformation("Successful sign-in for user ID: {UserId} with email: {Email}", 
                    userModel.Id, userModel.Email);

                var response = _mapper.Map<SignInSignUpResponse>(userModel);
                _logger.LogDebug("Mapped User to SignInSignUpResponse for user ID: {UserId}", userModel.Id);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during sign-in for email: {Email}", user.Email);
                throw;
            }
        }

        public async Task<GetUserDTO?> GetUserProfileAsync(Guid userId)
        {
            _logger.LogInformation("Retrieving user profile for user ID: {UserId}", userId);
            
            try
            {
                var user = await _userRepository.GetUserProfileAsync(userId);
                
                if (user is null)
                {
                    _logger.LogWarning("User profile not found for user ID: {UserId}", userId);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved user profile for user ID: {UserId} with email: {Email}", 
                    userId, user.Email);

                var response = _mapper.Map<GetUserDTO>(user);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving user profile for user ID: {UserId}", userId);
                throw;
            }
        }

        public async Task<GetUserDTO?> UpdateUserProfileAsync(Guid userId, UpdateUserDTO updatedUser)
        {
            _logger.LogInformation("Starting profile update for user ID: {UserId}", userId);
            
            try
            {
                var userModel = _mapper.Map<User>(updatedUser);
                _logger.LogDebug("Mapped UpdateUserDTO to User model for user ID: {UserId}", userId);

                var updated = await _userRepository.UpdateUserProfileAsync(userId, userModel, updatedUser.CurrentPassword);
                
                if (updated is null)
                {
                    _logger.LogWarning("Failed to update profile for user ID: {UserId}", userId);
                    return null;
                }

                _logger.LogInformation("Successfully updated user profile for user ID: {UserId} with email: {Email}", 
                    userId, updated.Email);

                var response = _mapper.Map<GetUserDTO>(updated);
                return response;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Profile update failed for user ID {UserId}: {Message}", userId, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during profile update for user ID: {UserId}", userId);
                throw;
            }
        }
    
        public async Task<bool> DeleteUserByIdAsync(Guid userId, DeleteUserDTO deleteUser)
        {
            _logger.LogInformation("Starting deletion process for user ID: {UserId}", userId);

            try
            {
                var result = await _userRepository.DeleteUserByIdAsync(userId, deleteUser.Password);

                if (!result)
                {
                    _logger.LogWarning("Failed to delete user with ID: {UserId}", userId);
                    return false;
                }
                _logger.LogInformation("Successfully deleted user with ID: {UserId}", userId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during deletion for user ID: {UserId}", userId);
                throw;
            }
        }
    }
}
