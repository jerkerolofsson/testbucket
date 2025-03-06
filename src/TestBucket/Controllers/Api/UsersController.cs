using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace TestBucket.Controllers.Api;

[ApiController]
public class UsersController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public UsersController(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [DisableCors]
    [HttpPost("/api/{tenantId}/users/login")]
    public async Task<IActionResult> LoginAsyncAsync([FromRoute] string tenantId, [FromQuery] string u, [FromQuery] string p)
    {
        var result = await _signInManager.PasswordSignInAsync(u,p,true, lockoutOnFailure: false);
        if(result.Succeeded)
        {
            return Ok();
        }
        return Unauthorized();
    }
}
