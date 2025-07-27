namespace LinkShorterAPI.DTOs.UserDTO
{
    public class SignInSignUpResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
    }
}
