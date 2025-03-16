using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace TestBucket.Domain.ApiKeys;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
}
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options,
                                       ILoggerFactory logger,
                                       UrlEncoder encoder,
                                       Microsoft.AspNetCore.Authentication.ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? apiKey = null;
        if (Request.Headers.TryGetValue("ApiKey", out var apiKeyValues) && apiKeyValues != StringValues.Empty && apiKeyValues.FirstOrDefault() is not null)
        {
            apiKey = apiKeyValues.FirstOrDefault();
        }

        if (apiKey == null)
        {
            if (Request.Headers.TryGetValue("Authorization", out var authorization) && authorization != StringValues.Empty)
            {
                var authorizationHeaderValue = authorization.First();
                if (authorizationHeaderValue is not null)
                {
                    var pair = authorizationHeaderValue.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (pair.Length == 2)
                    {
                        apiKey = pair[1];
                    }
                }
            }
        }

        if(apiKey is null)
        {
            return AuthenticateResult.Fail("Missing API Key");
        }

        // TODO: DB
        await Task.Delay(200);

        var claims = new[] 
        { 
            new Claim(ClaimTypes.Name, "admin@admin.com"),
            new Claim("tenant", "jerkerolofsson"),
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
