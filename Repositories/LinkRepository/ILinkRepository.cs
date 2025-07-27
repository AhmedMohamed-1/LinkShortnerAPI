using LinkShorterAPI.Models;

namespace LinkShorterAPI.Repositories.LinkRepository
{
    public interface ILinkRepository
    {
        Task<ShortLink> CreateAsync(ShortLink shortLink, Guid userId);
        Task<ShortLink?> GetByIdAsync(Guid id);
        Task<ShortLink?> GetByShortCodeAsync(string shortCode);
        Task<IEnumerable<ShortLink>> GetByUserIdAsync(Guid userId);
        Task<ShortLink?> UpdateAsync(ShortLink updatedLink);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ShortCodeExistsAsync(string shortCode);
        Task<Guid?> GetDefualtDomain(string Domain);
    }
}
