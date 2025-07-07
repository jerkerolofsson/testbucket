using System.Web;

using TestBucket.Integrations;

namespace TestBucket.Domain.Identity.OAuth;
internal class OAuthAuthenticator : IOAuth2Authenticator
{
    private readonly OAuthAuthManager _manager;

    public OAuthAuthenticator(OAuthAuthManager manager)
    {
        _manager = manager;
    }

    public Task<string> AuthenticateAsync(ClaimsPrincipal user, string scope, string clientId, string clientSecret, string authEndpoint, string tokenEndpoint, string integrationName, string baseUrl, Func<OAuthAuthState, Task> authenticatedCallback)
    {
        // Trim end so we create a valid string
        baseUrl = baseUrl.TrimEnd('/');
        var redirectUri = $"{baseUrl}/api/oauth/{integrationName.ToLowerInvariant()}"; // This should match the callback endpoint

        var state = new OAuthAuthState(user, clientId, clientSecret, authEndpoint, tokenEndpoint, integrationName, authenticatedCallback);
        state.RedirectUri = redirectUri;
        state.Scope = scope;
        _manager.RegisterState(state);

        // Build the authorization URL with required OAuth2 parameters
        var authUrlBuilder = new UriBuilder(authEndpoint);
        var query = HttpUtility.ParseQueryString(authUrlBuilder.Query);
        
        query["client_id"] = clientId;
        if (string.IsNullOrEmpty(query["response_type"]))
        {
            query["response_type"] = "code";
        }
        query["state"] = state.Id;

        query["scope"] = state.Scope;

        query["redirect_uri"] = redirectUri;
        
        authUrlBuilder.Query = query.ToString();
        
        return Task.FromResult(authUrlBuilder.ToString());
    }
}
