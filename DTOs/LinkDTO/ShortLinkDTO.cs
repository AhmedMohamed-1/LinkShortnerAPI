namespace LinkShorterAPI.DTOs.LinkDTO
{
    public class ShortLinkDTO
    {
        public Guid Id { get; set; }
        public string DestinationUrl { get; set; } = null!;
        public string ShortCode { get; set; } = null!;
        public string? Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int ClickCount { get; set; }
        public bool IsActive { get; set; }
        public string? Domain { get; set; } // optional, for branded domains

        public string ShortUrl => $"https://{Domain}/{ShortCode}";
    }

}
