using LinkShorterAPI.Models;

namespace LinkShorterAPI.Repositories.PremiumFeaturesRepository
{
    public interface IClickRepository
    {
        Task<Click> AddAsync(Click click);
    }
}
