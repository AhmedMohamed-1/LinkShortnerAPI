namespace LinkShorterAPI.DTOs.UserDTO
{
    public class DeleteUserDTO
    {
        /// <summary>
        /// The password of the user to confirm their identity before deletion.
        /// </summary>
        public string Password { get; set; } = null!;
    }
}
