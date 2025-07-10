using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.ApiKeys;

namespace TestBucket.Domain.AI.Mcp.Tools;
public class AuthenticatedTool
{
    private readonly IApiKeyAuthenticator _apiKeyAuthenticator;
    protected ClaimsPrincipal? _principal;

    public static AsyncLocal<string?> AuthorizationHeader { get; } = new();

    public AuthenticatedTool(IApiKeyAuthenticator apiKeyAuthenticator)
    {
        _apiKeyAuthenticator = apiKeyAuthenticator ?? throw new ArgumentNullException(nameof(apiKeyAuthenticator));
    }

    /// <summary>
    /// This is used to enable the tool to use the current ClaimsPrincipal and invoke the tool directly from an
    /// IChatClient or similar, not requiring authentication from the AuthorizationHeader
    /// </summary>
    /// <param name="principal"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void SetClaimsPrincipal(ClaimsPrincipal principal)
    {
        _principal = principal ?? throw new ArgumentNullException(nameof(principal));
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        if (_principal is null)
        {
            var authorization = AuthorizationHeader.Value;
            if (authorization is null)
            {
                return false;
            }
            var header = AuthenticationHeaderValue.Parse(authorization);
            if (header.Scheme.ToLower() != "bearer" || header.Parameter is null)
            {
                return false;
            }
            _principal = await _apiKeyAuthenticator.AuthenticateAsync(header.Parameter);
        }

        return _principal is not null && _principal.Identity?.IsAuthenticated == true;
    }
}
