using LinkShorterAPI.Authentication.Token;
using LinkShorterAPI.DTOs.UserDTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LinkShorterAPI.Authentication.Token
{
    public class UserToken : IUserToken
    {
        private readonly IConfiguration _config;
        private readonly ILogger<UserToken> _logger;

        public UserToken(IConfiguration config, ILogger<UserToken> logger)
        {
            _config = config;
            _logger = logger;
        }

        public string GenerateJwtToken(SignInSignUpResponse user)
        {
            try
            {
                // Validate configuration
                var jwtKey = _config["Jwt:Key"];
                var jwtIssuer = _config["Jwt:Issuer"];
                var jwtAudience = _config["Jwt:Audience"];

                if (string.IsNullOrEmpty(jwtKey))
                {
                    _logger.LogError("JWT Key is not configured");
                    throw new InvalidOperationException("JWT Key is not configured");
                }

                if (string.IsNullOrEmpty(jwtIssuer))
                {
                    _logger.LogError("JWT Issuer is not configured");
                    throw new InvalidOperationException("JWT Issuer is not configured");
                }

                if (string.IsNullOrEmpty(jwtAudience))
                {
                    _logger.LogError("JWT Audience is not configured");
                    throw new InvalidOperationException("JWT Audience is not configured");
                }

                // Validate key length (should be at least 32 characters for HS256)
                if (jwtKey.Length < 32)
                {
                    _logger.LogWarning("JWT Key length is less than 32 characters, which may not be secure for HS256");
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(7),
                    notBefore: DateTime.UtcNow,
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                _logger.LogDebug("JWT token generated successfully for user ID: {UserId}", user.Id);
                
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token for user ID: {UserId}", user.Id);
                throw;
            }
        }
    }
}
