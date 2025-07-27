namespace LinkShorterAPI.DTOs.LinkDTO
{
    public class UpdateShortLinkDTO
    {
        public Guid Id { get; }
        public string? DestinationUrl { get; set; }
        public string? Title { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
