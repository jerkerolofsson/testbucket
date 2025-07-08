using System.Security.Claims;

using TestBucket.Domain.Identity.OAuth;

namespace TestBucket.Integrations;

/// <summary>
/// This can be used by oauth to authenticate
/// </summary>
public interface IOAuth2Authenticator
{
    /// <summary>
    /// Requests oauth authentication using the specified endpoints.
    /// Returns a re-direct URL to the authentication endpoint.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <param name="authEndpoint"></param>
    /// <param name="tokenEndpoint"></param>
    /// <returns></returns>
    Task<string> AuthenticateAsync(ClaimsPrincipal user, string scope, string clientId, string clientSecret, string authEndpoint, string tokenEndpoint, string integrationName, string baseUrl, Func<OAuthAuthState,Task> authenticatedCallback);
}
