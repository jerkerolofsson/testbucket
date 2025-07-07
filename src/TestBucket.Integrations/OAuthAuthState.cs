using System.Security.Claims;

namespace TestBucket.Domain.Identity.OAuth;
public record class OAuthAuthState(ClaimsPrincipal User, string ClientId, string ClientSecret, string AuthEndpoint, string TokenEndpoint, string ServiceName, Func<OAuthAuthState,Task> AuthenticatedCallback)
{
    private string? _state;

    /// <summary>
    /// Authentication redirect (endpoint where we receive the code)
    /// This is passed to the remote system
    /// </summary>
    public string? RedirectUri { get; set; }

    /// <summary>
    /// Final redirect, once we have completed the authentication
    /// </summary>
    public string? SuccessRedirectUri { get; set; }

    public string Id
    {
        get
        {
            _state ??= GenerateState();
            return _state;
        }
    }

    public int ExpiresIn { get; set; }
    public string? Scope { get; set; }
    public string? RefreshToken { get; set; }
    public string? AccessToken { get; set; }

    private string GenerateState()
    {
        return Guid.NewGuid().ToString("N");
    }
}
