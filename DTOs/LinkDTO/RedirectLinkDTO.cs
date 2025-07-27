namespace LinkShorterAPI.DTOs.LinkDTO
{
    public class RedirectLinkDTO
    {
        public Guid Id { get; set; }
        public string? DestinationURL { get; set; } 
        public bool? IsActive { get; set; }
    }
}
