using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using TestBucket.Controllers.Api;

namespace TestBucket.Domain.Identity.OAuth;
public class OAuthApiController : ProjectApiControllerBase
{
    [HttpGet("/api/oauth/{integrationName}")]
    public async Task OauthCallbackAsync(
        [FromRoute] string integrationName,
        [FromQuery] string code, [FromQuery] string state)
    {
        // https://auth.atlassian.com/authorize?audience=api.atlassian.com&client_id=5sTjbSDCYaPa3vLCNS4cqslQV6Vmn66Z&scope=read%3Ajira-work%20write%3Ajira-work&redirect_uri=https%3A%2F%2Flocalhost%3A7067%2Fapi%2Foauth%2Fjira&state=${YOUR_USER_BOUND_VALUE}&response_type=code&prompt=consent

    }
}
