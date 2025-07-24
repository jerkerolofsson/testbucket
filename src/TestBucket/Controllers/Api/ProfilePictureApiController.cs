using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Domain.Identity;

namespace TestBucket.Controllers.Api;

[ApiController]
public class ProfilePictureApiController : ControllerBase
{
    private readonly IProfilePictureManager _profilePictureManager;

    public ProfilePictureApiController(IProfilePictureManager profilePictureManager)
    {
        _profilePictureManager = profilePictureManager;
    }

    [Authorize]
    [HttpGet("/api/users/profile/{userName}/image")]
    public async Task<IActionResult> GetUserProfilePicture([FromRoute] string userName)
    {
        var image = await _profilePictureManager.GetProfileImageUriAsync(User, userName);
        if(image is not null)
        {
            return File(image.Bytes, image.MediaType);
        }

        if(userName is "classification-bot" or "ai-runner")
        {
            return Redirect("/img/ai-user.png");
        }

        var imageBytes = Array.Empty<byte>();
        return File(imageBytes, "image/jpg");
    }
}
