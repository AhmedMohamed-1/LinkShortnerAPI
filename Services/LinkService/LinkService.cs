using AutoMapper;
using LinkShorterAPI.DTOs.LinkDTO;
using LinkShorterAPI.Models;
using LinkShorterAPI.Repositories.LinkRepository;
using System.Security.Cryptography;
using System.Text;

namespace LinkShorterAPI.Services.LinkService
{
    public class LinkService : ILinkService
    {
        private readonly ILinkRepository _linkRepository;
        private readonly IMapper _mapper;
        private const int DEFAULT_SLUG_LENGTH = 8;
        private const int MAX_RETRY_ATTEMPTS = 10;

        public LinkService(ILinkRepository linkRepository, IMapper mapper)
        {
            _linkRepository = linkRepository;
            _mapper = mapper;
        }

        public async Task<ShortLinkDTO> CreateAsync(CreateShortLinkDTO shortLink, Guid userId)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(shortLink.DestinationUrl))
            {
                throw new ArgumentException("Destination URL is required.");
            }

            if (!Uri.TryCreate(shortLink.DestinationUrl, UriKind.Absolute, out _))
            {
                throw new ArgumentException("Invalid destination URL format.");
            }

            // Determine the slug to use
            string slug;
            if (!string.IsNullOrWhiteSpace(shortLink.CustomAlias))
            {
                // Use custom alias if provided
                slug = shortLink.CustomAlias.Trim().ToLower();
                
                // Validate custom alias format
                if (!IsValidSlug(slug))
                {
                    throw new ArgumentException("Custom alias can only contain letters, numbers, and hyphens.");
                }

                // Check if the custom alias already exists
                if (await _linkRepository.ShortCodeExistsAsync(slug))
                {
                    throw new InvalidOperationException("Custom alias already exists.");
                }
            }
            else
            {
                // Generate a random slug
                slug = await GenerateUniqueSlugAsync();
            }

            // Map the DTO to the entity
            var linkEntity = _mapper.Map<ShortLink>(shortLink);
            
            // Set additional properties
            linkEntity.Slug = slug;
            linkEntity.CreatedAt = DateTime.UtcNow;
            linkEntity.CreatedBy = userId;
            linkEntity.IsActive = true;

            var defaultDomain = await _linkRepository.GetDefualtDomain("//PUT-YOUR-NGROK-SERVER-SAVED-IN-DB-OR-YOUR-DOMAIN");

            if (defaultDomain == null)
                throw new Exception("Default domain not found.");

            linkEntity.DomainId = defaultDomain.Value;

            var createdLink = await _linkRepository.CreateAsync(linkEntity,userId);
            return _mapper.Map<ShortLinkDTO>(createdLink);
        }

        public async Task<ShortLinkDTO?> GetByIdAsync(Guid id, Guid userId)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid link ID.");
            }

            var link = await _linkRepository.GetByIdAsync(id);

            if (link == null || link.CreatedBy != userId)
            {
                throw new UnauthorizedAccessException("You can only access your own links.");
            }

            return _mapper.Map<ShortLinkDTO>(link);
        }

        public async Task<ShortLinkDTO?> GetByShortCodeAsync(string shortCode, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(shortCode))
            {
                throw new ArgumentException("Short code is required.");
            }

            var link = await _linkRepository.GetByShortCodeAsync(shortCode.Trim().ToLower());

            if (link == null || link.CreatedBy != userId)
            {
                throw new UnauthorizedAccessException("You can only access your own links.");
            }

            return _mapper.Map<ShortLinkDTO>(link);
        }

        public async Task<IEnumerable<ShortLinkDTO>> GetByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("Invalid user ID.");
            }

            var links = await _linkRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<ShortLinkDTO>>(links);
        }

        public async Task<ShortLinkDTO?> UpdateAsync(UpdateShortLinkDTO updatedLink, Guid userId, Guid LinkID)
        {
            if (LinkID == Guid.Empty)
            {
                throw new ArgumentException("Invalid link ID.");
            }

            // Get the existing link to ensure it belongs to the user
            var existingLink = await _linkRepository.GetByIdAsync(LinkID);

            if (existingLink == null)
            {
                throw new InvalidOperationException("Link not found.");
            }

            if (existingLink.CreatedBy != userId)
            {
                throw new UnauthorizedAccessException("You can only update your own links.");
            }

            var linkEntity = _mapper.Map(updatedLink, existingLink);//_mapper.Map<ShortLink>(updatedLink);

            var updated = await _linkRepository.UpdateAsync(linkEntity);
            return updated == null ? null : _mapper.Map<ShortLinkDTO>(updated);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid link ID.");
            }

            // Get the existing link to ensure it belongs to the user
            var existingLink = await _linkRepository.GetByIdAsync(id);
            if (existingLink == null)
            {
                return false; // Link doesn't exist
            }

            if (existingLink.CreatedBy != userId)
            {
                throw new UnauthorizedAccessException("You can only delete your own links.");
            }

            return await _linkRepository.DeleteAsync(id);
        }

        public async Task<bool> ShortCodeExistsAsync(string shortCode)
        {
            if (string.IsNullOrWhiteSpace(shortCode))
            {
                return false;
            }

            return await _linkRepository.ShortCodeExistsAsync(shortCode.Trim().ToLower());
        }



        public async Task<RedirectLinkDTO?> Redirects(string shortCode)
        {
            var link = await _linkRepository.GetByShortCodeAsync(shortCode.Trim().ToLower());

            if (link == null)
                return null;

            return new RedirectLinkDTO
            {
                Id = link.Id,
                DestinationURL = link.DestinationUrl,
                IsActive = link.IsActive,
            };
        }




        /// <summary>
        /// Generates a unique random slug
        /// </summary>
        private async Task<string> GenerateUniqueSlugAsync()
        {
            for (int attempt = 0; attempt < MAX_RETRY_ATTEMPTS; attempt++)
            {
                var slug = GenerateRandomSlug(DEFAULT_SLUG_LENGTH);
                
                if (!await _linkRepository.ShortCodeExistsAsync(slug))
                {
                    return slug;
                }
            }

            throw new InvalidOperationException("Unable to generate a unique slug after maximum attempts.");
        }

        /// <summary>
        /// Generates a random slug with the specified length
        /// </summary>
        private string GenerateRandomSlug(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new byte[length];
            
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
            }

            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random[i] % chars.Length]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Validates if a slug contains only valid characters
        /// </summary>
        private bool IsValidSlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return false;

            // Check if slug contains only letters, numbers, and hyphens
            return slug.All(c => char.IsLetterOrDigit(c) || c == '-') && 
                   !slug.StartsWith("-") && 
                   !slug.EndsWith("-") &&
                   !slug.Contains("--");
        }
    }
}
