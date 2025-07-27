using LinkShorterAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkShorterAPI.Repositories.LinkRepository
{
    public class LinkRepository : ILinkRepository
    {
        private readonly LinkShortnerContext _context;
        public LinkRepository(LinkShortnerContext context)
        {
            _context = context;
        }

        public async Task<ShortLink> CreateAsync(ShortLink shortLink, Guid userId)
        {
            shortLink.CreatedBy = userId;
            await _context.ShortLinks.AddAsync(shortLink);
            await _context.SaveChangesAsync();
            return shortLink;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var link = await _context.ShortLinks.FirstOrDefaultAsync(l => l.Id == id);
            if (link == null)
            {
                return false;
            }

            _context.ShortLinks.Remove(link);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ShortLink?> GetByIdAsync(Guid id)
        {
            var link = await _context.ShortLinks
                .Include(l => l.Domain)
                .FirstOrDefaultAsync(l => l.Id == id);
            return link;
        }

        public async Task<ShortLink?> GetByShortCodeAsync(string shortCode)
        {
            var link = await _context.ShortLinks
                .Include(l => l.Domain)
                .FirstOrDefaultAsync(l => l.Slug == shortCode);
            return link;
        }

        public async Task<IEnumerable<ShortLink>> GetByUserIdAsync(Guid userId)
        {
            return await _context.ShortLinks
                .Where(l => l.CreatedBy == userId)
                .Include(l => l.Domain)
                .ToListAsync();
        }

        public async Task<bool> ShortCodeExistsAsync(string shortCode)
        {
            return await _context.ShortLinks.AnyAsync(l => l.Slug == shortCode);
        }

        public async Task<ShortLink?> UpdateAsync(ShortLink updatedLink)
        {
            var link = await _context.ShortLinks.FirstOrDefaultAsync(l => l.Id == updatedLink.Id);
            if (link == null)
            {
                return null;
            }

            // Update only the fields that are allowed to be updated
            if (!string.IsNullOrEmpty(updatedLink.DestinationUrl))
                link.DestinationUrl = updatedLink.DestinationUrl;

            if (!string.IsNullOrEmpty(updatedLink.Title))
                link.Title = updatedLink.Title;

            if (updatedLink.IsActive.HasValue)
                link.IsActive = updatedLink.IsActive;

            if (updatedLink.ClickLimit.HasValue)
                link.ClickLimit = updatedLink.ClickLimit;

            if (updatedLink.ExpireAt.HasValue)
                link.ExpireAt = updatedLink.ExpireAt;

            if (!string.IsNullOrEmpty(updatedLink.PasswordHash))
                link.PasswordHash = updatedLink.PasswordHash;

            await _context.SaveChangesAsync();
            return link;
        }

        public async Task<Guid?> GetDefualtDomain(string Domain)
        {
            var DOMAIN = await _context.Domains.FirstOrDefaultAsync(d => d.DomainName == Domain);
            return DOMAIN?.Id;
        }
    }
}
