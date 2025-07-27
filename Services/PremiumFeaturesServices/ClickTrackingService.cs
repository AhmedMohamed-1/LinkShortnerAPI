using LinkShorterAPI.Models;
using LinkShorterAPI.Repositories.PremiumFeaturesRepository;
using MaxMind.GeoIP2;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Net;
using UAParser;

namespace LinkShorterAPI.Services.PremiumFeaturesServices
{
    public class ClickTrackingService : IClickTrackingService
    {
        private readonly IClickRepository _clickRepository;
        private readonly DatabaseReader _reader;
        private readonly Parser _parser;
        public ClickTrackingService(IClickRepository clickRepository , IWebHostEnvironment env)
        {
            _clickRepository = clickRepository;

            var dbPath = Path.Combine(env.ContentRootPath, "GeoData", "GeoLite2-City.mmdb");
            _reader = new DatabaseReader(dbPath);

            _parser = Parser.GetDefault();
        }
        public async Task<Click> TrackClickAsync(Guid shortLinkId, HttpRequest request)
        {
            string ip = request.Headers["X-Forwarded-For"].FirstOrDefault()
         ?? request.HttpContext.Connection.RemoteIpAddress?.ToString()
         ?? "8.8.8.8"; // fallback IP if everything fails
            var cityResponse = _reader.City(IPAddress.Parse(ip));
            var client = _parser.Parse(request.Headers["User-Agent"]);

            var click = new Click
            {
                ShortLinkId = shortLinkId,
                ClickedAt = DateTime.UtcNow,
                IpAddress = ip,
                UserAgent = request.Headers["User-Agent"].ToString(),
                Referrer = request.Headers["Referer"].ToString(),
                IsBot = client.ToString().Contains("bot", StringComparison.OrdinalIgnoreCase),
                City = cityResponse.City?.Name,
                CountryCode = cityResponse.Country?.IsoCode,
                Device = client.Device.Family,
                Os = client.OS.ToString(),
                Browser = client.UA.ToString()
            };

            await _clickRepository.AddAsync(click);
            return click;
        }
    }
}
