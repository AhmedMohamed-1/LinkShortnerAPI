using FluentValidation;
using LinkShorterAPI.Authentication.Token;
using LinkShorterAPI.DTOs.UserDTO;
using LinkShorterAPI.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LinkShorterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserToken _userToken;
        private readonly IValidator<SignUpDTO> _signUpValidator;
        private readonly IValidator<SignInDTO> _signInValidator;
        private readonly IValidator<UpdateUserDTO> _updateUserValidator;
        private readonly IValidator<DeleteUserDTO> _deleteUserValidator;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService, 
            IUserToken userToken, 
            IValidator<SignUpDTO> signUpValidator,
            IValidator<SignInDTO> signInValidator,
            IValidator<UpdateUserDTO> updateUserValidator,
            IValidator<DeleteUserDTO> deleteUserValidator,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _userToken = userToken;
            _signUpValidator = signUpValidator;
            _signInValidator = signInValidator;
            _updateUserValidator = updateUserValidator;
            _deleteUserValidator = deleteUserValidator;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="signUpDto">User registration data</param>
        /// <returns>JWT token for authentication</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] SignUpDTO signUpDto)
        {
            _logger.LogInformation("Registration request received for email: {Email}", signUpDto.Email);

            // Validate the request
            var validationResult = await _signUpValidator.ValidateAsync(signUpDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Registration validation failed for email {Email}: {Errors}", 
                    signUpDto.Email, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
            }

            try
            {
                var userResponse = await _userService.RegisterAsync(signUpDto);
                var token = _userToken.GenerateJwtToken(userResponse);

                var response = new
                {
                    token = token,
                    message = "User registered successfully"
                };

                _logger.LogInformation("User registered successfully with ID: {UserId}", userResponse.Id);
                return CreatedAtAction(nameof(GetProfile), new { id = userResponse.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Registration failed for email {Email}: {Message}", signUpDto.Email, ex.Message);
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration for email: {Email}", signUpDto.Email);
                return StatusCode(500, new { error = "An unexpected error occurred during registration." });
            }
        }

        /// <summary>
        /// Sign in an existing user
        /// </summary>
        /// <param name="signInDto">User sign-in credentials</param>
        /// <returns>JWT token for authentication</returns>
        [HttpPost("signin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SignIn([FromBody] SignInDTO signInDto)
        {
            _logger.LogInformation("Sign-in request received for email: {Email}", signInDto.Email);

            // Validate the request
            var validationResult = await _signInValidator.ValidateAsync(signInDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Sign-in validation failed for email {Email}: {Errors}", 
                    signInDto.Email, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
            }

            try
            {
                var userResponse = await _userService.SignInAsync(signInDto);
                
                if (userResponse == null)
                {
                    _logger.LogWarning("Invalid credentials for email: {Email}", signInDto.Email);
                    return Unauthorized(new { error = "Invalid email or password." });
                }

                var token = _userToken.GenerateJwtToken(userResponse);

                var response = new
                {
                    token = token,
                    message = "Sign-in successful"
                };

                _logger.LogInformation("User signed in successfully with ID: {UserId}", userResponse.Id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during sign-in for email: {Email}", signInDto.Email);
                return StatusCode(500, new { error = "An unexpected error occurred during sign-in." });
            }
        }

        /// <summary>
        /// Get user profile
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User profile information</returns>
        [HttpGet("profile/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile(Guid id)
        {
            _logger.LogInformation("Profile request received for user ID: {UserId}", id);

            try
            {
                var user = await _userService.GetUserProfileAsync(id);
                
                if (user == null)
                {
                    _logger.LogWarning("Profile not found for user ID: {UserId}", id);
                    return NotFound(new { error = "User profile not found." });
                }

                var userDto = new GetUserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsActive = user.IsActive
                };

                _logger.LogInformation("Profile retrieved successfully for user ID: {UserId}", id);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving profile for user ID: {UserId}", id);
                return StatusCode(500, new { error = "An unexpected error occurred while retrieving the profile." });
            }
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="updateUserDto">Updated user information</param>
        /// <returns>Updated user profile</returns>
        [HttpPut("profile/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateUserDTO updateUserDto)
        {
            _logger.LogInformation("Profile update request received for user ID: {UserId}", id);

            // Validate the request
            var validationResult = await _updateUserValidator.ValidateAsync(updateUserDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Profile update validation failed for user ID {UserId}: {Errors}", 
                    id, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
            }

            try
            {
                var updatedUser = await _userService.UpdateUserProfileAsync(id, updateUserDto);
                
                if (updatedUser == null)
                {
                    _logger.LogWarning("Profile update failed - user not found for ID: {UserId}", id);
                    return NotFound(new { error = "User profile not found." });
                }

                var userDto = new GetUserDTO
                {
                    Id = updatedUser.Id,
                    Email = updatedUser.Email,
                    FullName = updatedUser.FullName,
                    IsActive = updatedUser.IsActive
                };

                _logger.LogInformation("Profile updated successfully for user ID: {UserId}", id);
                return Ok(userDto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Profile update failed for user ID {UserId}: {Message}", id, ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during profile update for user ID: {UserId}", id);
                return StatusCode(500, new { error = "An unexpected error occurred while updating the profile." });
            }
        }

        /// <summary>
        /// Get current user profile (extracts user ID from JWT token)
        /// </summary>
        /// <returns>Current user profile information</returns>
        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                _logger.LogWarning("Invalid or missing user ID in JWT token");
                return Unauthorized(new { error = "Invalid authentication token." });
            }

            _logger.LogInformation("Current user profile request received for user ID: {UserId}", userId);

            try
            {
                var user = await _userService.GetUserProfileAsync(userId);
                
                if (user == null)
                {
                    _logger.LogWarning("Profile not found for user ID: {UserId}", userId);
                    return NotFound(new { error = "User profile not found." });
                }

                var userDto = new GetUserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsActive = user.IsActive
                };

                _logger.LogInformation("Current user profile retrieved successfully for user ID: {UserId}", userId);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving current user profile for user ID: {UserId}", userId);
                return StatusCode(500, new { error = "An unexpected error occurred while retrieving the profile." });
            }
        }

        /// <summary>
        /// Update current user profile (extracts user ID from JWT token)
        /// </summary>
        /// <param name="updateUserDto">Updated user information</param>
        /// <returns>Updated user profile</returns>
        [HttpPut("profile")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCurrentUserProfile([FromBody] UpdateUserDTO updateUserDto)
        {
            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                _logger.LogWarning("Invalid or missing user ID in JWT token");
                return Unauthorized(new { error = "Invalid authentication token." });
            }

            _logger.LogInformation("Current user profile update request received for user ID: {UserId}", userId);

            // Validate the request
            var validationResult = await _updateUserValidator.ValidateAsync(updateUserDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Profile update validation failed for user ID {UserId}: {Errors}", 
                    userId, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
            }

            try
            {
                var updatedUser = await _userService.UpdateUserProfileAsync(userId, updateUserDto);
                
                if (updatedUser == null)
                {
                    _logger.LogWarning("Profile update failed - user not found for ID: {UserId}", userId);
                    return NotFound(new { error = "User profile not found." });
                }

                var userDto = new GetUserDTO
                {
                    Id = updatedUser.Id,
                    Email = updatedUser.Email,
                    FullName = updatedUser.FullName,
                    IsActive = updatedUser.IsActive
                };

                _logger.LogInformation("Current user profile updated successfully for user ID: {UserId}", userId);
                return Ok(userDto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Profile update failed for user ID {UserId}: {Message}", userId, ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during current user profile update for user ID: {UserId}", userId);
                return StatusCode(500, new { error = "An unexpected error occurred while updating the profile." });
            }
        }


        [HttpDelete("profile")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCurrentUserProfile([FromBody] DeleteUserDTO deleteUser)
        {
            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                _logger.LogWarning("Invalid or missing user ID in JWT token");
                return Unauthorized(new { error = "Invalid authentication token." });
            }
            _logger.LogInformation("Delete profile request received for user ID: {UserId}", userId);
            try
            {
                // Validate the delete request
                var ValidationRequest = await _deleteUserValidator.ValidateAsync(deleteUser);
                if (!ValidationRequest.IsValid)
                {
                    _logger.LogWarning("Profile deletion validation failed for user ID {UserId}: {Errors}",
                        userId, string.Join(", ", ValidationRequest.Errors.Select(e => e.ErrorMessage)));
                    return BadRequest(new { errors = ValidationRequest.Errors.Select(e => e.ErrorMessage) });
                }

                var isDeleted = await _userService.DeleteUserByIdAsync(userId , deleteUser);

                if (!isDeleted)
                {
                    _logger.LogWarning("Profile deletion failed - user not found for ID: {UserId}", userId);
                    return NotFound(new { error = "User profile not found." });
                }
                _logger.LogInformation("Profile deleted successfully for user ID: {UserId}", userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during profile deletion for user ID: {UserId}", userId);
                return StatusCode(500, new { error = "An unexpected error occurred while deleting the profile." });
            }
        }
    }
}
