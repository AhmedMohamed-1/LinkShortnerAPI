using LinkShorterAPI.Models;

namespace LinkShorterAPI.Services.PremiumFeaturesServices
{
    public interface IClickTrackingService
    {
        Task<Click> TrackClickAsync(Guid shortLinkId, HttpRequest request);
    }
}
