using LinkShorterAPI.DTOs.UserDTO;

namespace LinkShorterAPI.Authentication.Token
{
    public interface IUserToken
    {
        string GenerateJwtToken(SignInSignUpResponse user);
    }
}
