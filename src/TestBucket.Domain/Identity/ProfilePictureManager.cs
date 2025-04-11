using System.Security.Claims;

using Microsoft.Extensions.Caching.Memory;

using TestBucket.Domain.Identity.Models;
using TestBucket.Formats.Shared;

namespace TestBucket.Domain.Identity
{
    internal class ProfilePictureManager : IProfilePictureManager
    {
        private readonly IMemoryCache _cache;
        private readonly IUserManager _userManager;

        public ProfilePictureManager(IMemoryCache cache, IUserManager userManager)
        {
            _cache = cache;
            _userManager = userManager;
        }

        public void ClearCachedProfileImage(string username)
        {
            string key = "profileimage:" + username;
            _cache.Remove(key);
        }

        /// <summary>
        /// Returns a data uri
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<ProfileImage?> GetProfileImageUriAsync(ClaimsPrincipal principal, string username)
        {
            string key = "profileimage:" + username;
            return await _cache.GetOrCreateAsync<ProfileImage?>(key, async (entry) =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                var user = await _userManager.GetUserByNormalizedUserNameAsync(principal, username.ToUpper());
                if (user?.ProfileImageUri is not null)
                {
                    var dto = DataUriParser.ParseDataUri(user.ProfileImageUri);
                    if (dto.ContentType is not null && dto.Data is not null)
                    {
                        return new ProfileImage(dto.ContentType, dto.Data);
                    }
                }
                return null;
            });
        }
    }
}
