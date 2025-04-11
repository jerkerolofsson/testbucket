using System.Security.Claims;

using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.Identity;
public interface IProfilePictureManager
{
    void ClearCachedProfileImage(string username);
    Task<ProfileImage?> GetProfileImageUriAsync(ClaimsPrincipal principal, string username);
}