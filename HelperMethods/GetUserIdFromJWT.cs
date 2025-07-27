using LinkShorterAPI.Models;
using System.Security.Claims;

namespace LinkShorterAPI.HelperMethods
{
    public class GetUserIdFromJWT
    {
        /// <summary>
        /// Extract user ID from JWT ClaimsPrincipal
        /// </summary>
        public static Guid GetUserIdFromClaims(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(userIdClaim, out Guid userId) ? userId : Guid.Empty;
        }
    }
}
