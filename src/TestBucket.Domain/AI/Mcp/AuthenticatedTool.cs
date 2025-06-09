using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.ApiKeys;

namespace TestBucket.Domain.AI.Mcp;
public class AuthenticatedTool
{
    private readonly IApiKeyAuthenticator _apiKeyAuthenticator;
    protected ClaimsPrincipal? _principal;

    public static AsyncLocal<string?> AuthorizationHeader { get; } = new();

    public AuthenticatedTool(IApiKeyAuthenticator apiKeyAuthenticator)
    {
        _apiKeyAuthenticator = apiKeyAuthenticator ?? throw new ArgumentNullException(nameof(apiKeyAuthenticator));
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var authorization = AuthenticatedTool.AuthorizationHeader.Value;
        if(authorization is null)
        {
            return false;
        }
        var header = AuthenticationHeaderValue.Parse(authorization);
        if(header.Scheme.ToLower() != "bearer" || header.Parameter is null)
        {
            return false;
        }

        _principal = await _apiKeyAuthenticator.AuthenticateAsync(header.Parameter);
        return _principal is not null && _principal.Identity?.IsAuthenticated == true;
    }
}
