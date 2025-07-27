namespace LinkShorterAPI.DTOs.LinkDTO
{
    public class CreateShortLinkDTO
    {
        public string DestinationUrl { get; set; } = null!;
        public string? CustomAlias { get; set; } // optional
        public string? Title { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public Guid? DomainId { get; set; } // optional for branded domains
    }

}
