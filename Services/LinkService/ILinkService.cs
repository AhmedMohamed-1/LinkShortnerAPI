using LinkShorterAPI.DTOs.LinkDTO;

namespace LinkShorterAPI.Services.LinkService
{
    public interface ILinkService
    {
        Task<ShortLinkDTO> CreateAsync(CreateShortLinkDTO shortLink, Guid userId);
        Task<ShortLinkDTO?> GetByIdAsync(Guid id , Guid userId);
        Task<ShortLinkDTO?> GetByShortCodeAsync(string shortCode , Guid userId);
        Task<IEnumerable<ShortLinkDTO>> GetByUserIdAsync(Guid userId);
        Task<ShortLinkDTO?> UpdateAsync(UpdateShortLinkDTO updatedLink, Guid userId, Guid LinkID);
        Task<bool> DeleteAsync(Guid id, Guid userId);
        Task<bool> ShortCodeExistsAsync(string shortCode);

        Task<RedirectLinkDTO?> Redirects(string shortCode);
    }
}
