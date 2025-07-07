using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestBucket.Domain.Identity.OAuth;

/// <summary>
/// OAuth token response model
/// </summary>
public class OAuthTokenResponse
{
    public string? access_token { get; set; }
    public string? refresh_token { get; set; }
    public string? token_type { get; set; }
    public int expires_in { get; set; }
    public string? scope { get; set; }
}

/// <summary>
/// This is registered as a singleton allowing connection from API callbacks to sessions through states
/// </summary>
public class OAuthAuthManager
{
    private readonly ConcurrentDictionary<string, OAuthAuthState> _states = [];
    private readonly IHttpClientFactory _httpClientFactory;

    public OAuthAuthManager(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public void RegisterState(OAuthAuthState state)
    {
        _states[state.Id] = state;
    }

    public async Task<string?> OnCodeReceivedAsync(string stateKey, string code)
    {
        if(_states.TryGetValue(stateKey, out var state))
        {
            await ExchangeTokenAsync(state, code);

            return state.SuccessRedirectUri;
        }
        return null;
    }

    /// <summary>
    /// Exchanges the code to a token
    /// </summary>
    /// <param name="state"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task ExchangeTokenAsync(OAuthAuthState state, string code)
    {
        await ExchangeTokenAsync(state, code, null, "authorization_code");
    }

    /// <summary>
    /// Refreshes a token
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task RefreshTokenAsync(OAuthAuthState state)
    {
        if(string.IsNullOrEmpty(state.RefreshToken))
        {
            throw new ArgumentException("Refresh token is missing");
        }
        await ExchangeTokenAsync(state, null, state.RefreshToken, "refresh_token");
    }

    private async Task ExchangeTokenAsync(OAuthAuthState state, string? code, string? refreshToken, string grantType)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        
        var tokenRequestParams = new Dictionary<string, string>
        {
            ["grant_type"] = grantType,
            ["client_id"] = state.ClientId,
            ["client_secret"] = state.ClientSecret,
            ["state"] = state.Id
        };
        if (refreshToken is not null)
        {
            tokenRequestParams["refresh_token"] = refreshToken;
        }
        if (code is not null)
        {
            tokenRequestParams["code"] = code;
        }

        if (state.RedirectUri is not null)
        {
            tokenRequestParams["redirect_uri"] = state.RedirectUri;
        }

        var tokenRequestContent = new FormUrlEncodedContent(tokenRequestParams);

        try
        {
            using var response = await httpClient.PostAsync(state.TokenEndpoint, tokenRequestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<OAuthTokenResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (tokenResponse?.access_token != null)
            {
                state.AccessToken = tokenResponse.access_token;
                state.RefreshToken = tokenResponse.refresh_token;
                state.Scope = tokenResponse.scope;
                state.ExpiresIn = tokenResponse.expires_in;

                // Store the tokens in the state or handle them as needed
                // For now, just invoke the callback to indicate success
                state.AuthenticatedCallback?.Invoke(state);
                
                // Remove the state from the dictionary as it's no longer needed
                _states.TryRemove(state.Id, out _);
            }
        }
        catch (HttpRequestException ex)
        {
            // Log the error (logging not shown for brevity)
            // In production, you might want to handle different error scenarios
            throw new InvalidOperationException($"Failed to exchange authorization code for access token: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse token response: {ex.Message}", ex);
        }
    }
}
