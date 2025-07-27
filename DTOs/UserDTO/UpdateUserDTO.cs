namespace LinkShorterAPI.DTOs.UserDTO
{
    public class UpdateUserDTO
    {
        public string? FullName { get; set; }

        // Optional: allow users to update their password
        public string? NewPassword { get; set; }
        public string? CurrentPassword { get; set; }
    }
}
