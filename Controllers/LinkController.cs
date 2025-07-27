using LinkShorterAPI.DTOs.LinkDTO;
using LinkShorterAPI.Services.LinkService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LinkShorterAPI.HelperMethods;
using LinkShorterAPI.Services.PremiumFeaturesServices;

namespace LinkShorterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize]
    public class LinkController : ControllerBase
    {
        private readonly ILinkService _linkService;
        private readonly IClickTrackingService _clickTrackingService;
        private readonly ILogger<LinkController> _logger;

        public LinkController(ILinkService linkService, IClickTrackingService clickTrackingService, ILogger<LinkController> logger)
        {
            _clickTrackingService = clickTrackingService;
            _linkService = linkService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new short link
        /// </summary>
        /// <param name="createLinkDto">Link creation data</param>
        /// <returns>Created short link</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateLink([FromBody] CreateShortLinkDTO createLinkDto)
        {
            _logger.LogInformation("Create link request received for URL: {Url}", createLinkDto.DestinationUrl);

            try
            {
                // Get user ID from JWT token
                var userId = GetUserIdFromJWT.GetUserIdFromClaims(User);
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new { error = "Invalid user token." });
                }

                var createdLink = await _linkService.CreateAsync(createLinkDto, userId);

                _logger.LogInformation("Link created successfully with ID: {LinkId}, Slug: {Slug}", 
                    createdLink.Id, createdLink.ShortCode);

                return CreatedAtAction(nameof(GetLink), new { id = createdLink.Id }, createdLink);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Link creation failed: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Link creation failed: {Message}", ex.Message);
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during link creation");
                return StatusCode(500, new { error = "An unexpected error occurred while creating the link." });
            }
        }

        /// <summary>
        /// Get a link by ID
        /// </summary>
        /// <param name="id">Link ID</param>
        /// <returns>Link information</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLink(Guid id)
        {
            _logger.LogInformation("Get link request received for ID: {LinkId}", id);

            try
            {
                var UserId = GetUserIdFromJWT.GetUserIdFromClaims(User); 

                var link = await _linkService.GetByIdAsync(id , UserId);
                
                if (link == null)
                {
                    _logger.LogWarning("Link not found for ID: {LinkId}", id);
                    return NotFound(new { error = "Link not found." });
                }

                _logger.LogInformation("Link retrieved successfully for ID: {LinkId}", id);
                return Ok(link);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Get link failed: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving link with ID: {LinkId}", id);
                return StatusCode(500, new { error = "An unexpected error occurred while retrieving the link." });
            }
        }

        /// <summary>
        /// Get a link by short code (authentication required)
        /// </summary>
        /// <param name="shortCode">Short code</param>
        /// <returns>Link information</returns>
        [HttpGet("redirect/{shortCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetLinkByShortCode(string shortCode)
        {
            _logger.LogInformation("Get link by short code request received: {ShortCode}", shortCode);

            try
            {
                var userId = GetUserIdFromJWT.GetUserIdFromClaims(User);

                var link = await _linkService.GetByShortCodeAsync(shortCode , userId);
                
                if (link == null)
                {
                    _logger.LogWarning("Link not found for short code: {ShortCode}", shortCode);
                    return NotFound(new { error = "Link not found." });
                }

                _logger.LogInformation("Link retrieved successfully for short code: {ShortCode}", shortCode);
                return Ok(link);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Link update failed - unauthorized: {Message}", ex.Message);
                return Unauthorized(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Get link by short code failed: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving link with short code: {ShortCode}", shortCode);
                return StatusCode(500, new { error = "An unexpected error occurred while retrieving the link." });
            }
        }

        /// <summary>
        /// Get all links for the current user
        /// </summary>
        /// <returns>List of user's links</returns>
        [HttpGet("my-links")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyLinks()
        {
            _logger.LogInformation("Get my links request received");

            try
            {
                var userId = GetUserIdFromJWT.GetUserIdFromClaims(User);
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new { error = "Invalid user token." });
                }

                var links = await _linkService.GetByUserIdAsync(userId);

                _logger.LogInformation("Retrieved {Count} links for user ID: {UserId}", links.Count(), userId);
                return Ok(links);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Link update failed - unauthorized: {Message}", ex.Message);
                return Unauthorized(new { error = ex.Message });
            }

            catch (ArgumentException ex)
            {
                _logger.LogWarning("Get my links failed: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving links for current user");
                return StatusCode(500, new { error = "An unexpected error occurred while retrieving your links." });
            }
        }

        /// <summary>
        /// Update a link
        /// </summary>
        /// <param name="id">Link ID</param>
        /// <param name="updateLinkDto">Updated link data</param>
        /// <returns>Updated link</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateLink(Guid id, [FromBody] UpdateShortLinkDTO updateLinkDto)
        {
            _logger.LogInformation("Update link request received for ID: {LinkId}", id);

            try
            {
                var userId = GetUserIdFromJWT.GetUserIdFromClaims(User);
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new { error = "Invalid user token." });
                }

                var updatedLink = await _linkService.UpdateAsync(updateLinkDto, userId, id);
                
                if (updatedLink == null)
                {
                    _logger.LogWarning("Link update failed - link not found for ID: {LinkId}", id);
                    return NotFound(new { error = "Link not found." });
                }

                _logger.LogInformation("Link updated successfully for ID: {LinkId}", id);
                return Ok(updatedLink);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Link update failed: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Link update failed: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Link update failed - unauthorized: {Message}", ex.Message);
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during link update for ID: {LinkId}", id);
                return StatusCode(500, new { error = "An unexpected error occurred while updating the link." });
            }
        }

        /// <summary>
        /// Delete a link
        /// </summary>
        /// <param name="id">Link ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteLink(Guid id)
        {
            _logger.LogInformation("Delete link request received for ID: {LinkId}", id);

            try
            {
                var userId = GetUserIdFromJWT.GetUserIdFromClaims(User);
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new { error = "Invalid user token." });
                }

                var deleted = await _linkService.DeleteAsync(id, userId);
                
                if (!deleted)
                {
                    _logger.LogWarning("Link deletion failed - link not found for ID: {LinkId}", id);
                    return NotFound(new { error = "Link not found." });
                }

                _logger.LogInformation("Link deleted successfully for ID: {LinkId}", id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Link deletion failed: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Link deletion failed - unauthorized: {Message}", ex.Message);
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during link deletion for ID: {LinkId}", id);
                return StatusCode(500, new { error = "An unexpected error occurred while deleting the link." });
            }
        }

        /// <summary>
        /// Check if a short code exists
        /// </summary>
        /// <param name="shortCode">Short code to check</param>
        /// <returns>Whether the short code exists</returns>
        [HttpGet("check/{shortCode}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckShortCodeExists(string shortCode)
        {
            _logger.LogInformation("Check short code request received: {ShortCode}", shortCode);

            try
            {
                var exists = await _linkService.ShortCodeExistsAsync(shortCode);

                _logger.LogInformation("Short code {ShortCode} exists: {Exists}", shortCode, exists);
                return Ok(new { exists = exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while checking short code: {ShortCode}", shortCode);
                return StatusCode(500, new { error = "An unexpected error occurred while checking the short code." });
            }
        }




        [HttpGet("/{shortCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> RedirectToDestination(string shortCode)
        {
            var link = await _linkService.Redirects(shortCode);

            if (link == null)
            {
                return NotFound("Link not found");
            }
            if (link.IsActive == false)
            {
                return NotFound("Link isn't active.");
            }

            await _clickTrackingService.TrackClickAsync(link.Id, Request);
            return Redirect(link.DestinationURL!);
        }
    }
} 