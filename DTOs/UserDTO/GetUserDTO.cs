namespace LinkShorterAPI.DTOs.UserDTO
{
    public class GetUserDTO
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public bool? IsActive { get; set; }
        public string Email { get; set; } = null!;
    }
}
