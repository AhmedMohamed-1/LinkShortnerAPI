using LinkShorterAPI.Models;

namespace LinkShorterAPI.Repositories.PremiumFeaturesRepository
{
    public class ClickRepository : IClickRepository
    {
        private readonly LinkShortnerContext _context;
        public ClickRepository(LinkShortnerContext context)
        {
            _context = context;
        }
        public async Task<Click> AddAsync(Click click)
        {
            var result = await _context.Clicks.AddAsync(click);
            await _context.SaveChangesAsync(); // Save to DB
            return result.Entity; // Return the saved Click entity
        }
    }
}
