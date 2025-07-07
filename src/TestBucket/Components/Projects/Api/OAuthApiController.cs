using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using TestBucket.Controllers.Api;
using TestBucket.Domain.Identity.OAuth;

namespace TestBucket.Components.Projects.Api;
public class OAuthApiController : ProjectApiControllerBase
{
    private readonly OAuthAuthManager _oauthAuthManager;

    public OAuthApiController(OAuthAuthManager oauthAuthManager)
    {
        _oauthAuthManager = oauthAuthManager;
    }

    [HttpGet("/api/oauth/{integrationName}")]
    public async Task<IActionResult> OauthCallbackAsync(
        [FromRoute] string integrationName,
        [FromQuery] string code, [FromQuery] string state)
    {
        // https://auth.atlassian.com/authorize?audience=api.atlassian.com&client_id=5sTjbSDCYaPa3vLCNS4cqslQV6Vmn66Z&scope=read%3Ajira-work%20write%3Ajira-work&redirect_uri=https%3A%2F%2Flocalhost%3A7067%2Fapi%2Foauth%2Fjira&state=${YOUR_USER_BOUND_VALUE}&response_type=code&prompt=consent
        
        var url = await _oauthAuthManager.OnCodeReceivedAsync(state, code);
        if(url is not null)
        {
            return Redirect(url);
        }
        return Ok();
    }
}