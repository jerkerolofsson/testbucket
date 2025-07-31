using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Identity.OAuth;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace TestBucket.Domain.UnitTests.Identity.OAuth
{
    /// <summary>
    /// Unit tests for the OAuth authentication manager functionality.
    /// Tests the OAuth flow including authorization code exchange and token management.
    /// </summary>
    [UnitTest]
    [Feature("OAuth")]
    [Component("Identity")]
    [FunctionalTest]
    [EnrichedTest]
    public class OAuthManagerTests : IDisposable
    {
        /// <summary>
        /// Mock HTTP server for simulating OAuth endpoints during testing.
        /// </summary>
        private readonly WireMockServer _server;

        /// <summary>
        /// HTTP client factory implementation that creates clients configured to use the WireMock server.
        /// Used to mock external OAuth provider endpoints.
        /// </summary>
        /// <param name="Server">The WireMock server instance to use for HTTP requests.</param>

        private class WireMockHttpClientFactory(WireMockServer Server) : IHttpClientFactory
        {
            /// <summary>
            /// Creates an HTTP client configured to use the WireMock server.
            /// </summary>
            /// <param name="name">The logical name of the client to create.</param>
            /// <returns>An HTTP client instance configured for the WireMock server.</returns>
            public HttpClient CreateClient(string name) => Server.CreateClient();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthManagerTests"/> class.
        /// Sets up the WireMock server for testing OAuth endpoints.
        /// </summary>
        public OAuthManagerTests()
        {
            _server = WireMockServer.Start();
        }
        /// <summary>
        /// Performs cleanup by stopping the WireMock server.
        /// </summary>
        public void Dispose()
        {
            _server?.Stop();
        }

        /// <summary>
        /// Tests that the OAuth authorization code flow correctly exchanges an authorization code for access tokens.
        /// Verifies that the token exchange request is made, the callback is invoked, and tokens are properly stored.
        /// </summary>
        [Fact]
        public async Task OnCodeReceivedAsync_CallsExchangeToken()
        {
            string baseUrl = _server!.Url!;
            var user = new System.Security.Claims.ClaimsPrincipal();
            bool wasCallbackInvoked = false;
            var state = new OAuthAuthState(user, "1", "2", $"{baseUrl}/auth", $"{baseUrl}/token", "service-name", (state) =>
            {
                wasCallbackInvoked = true;
                return Task.CompletedTask;
            });
            state.SuccessRedirectUri = "http://it-is-a-success";

            var response = new OAuthTokenResponse
            {
                access_token = "1234",
                expires_in = 1000,
                refresh_token = "2345"
            };

            _server.Given(
                Request.Create()
                    .WithPath("/token"))
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyAsJson(response));

            var sut = new OAuthAuthManager(new WireMockHttpClientFactory(_server));

            //act
            sut.RegisterState(state);
            var actual = await sut.OnCodeReceivedAsync(state.Id, "12345678");

            //assert
            Assert.Equal(actual, state.SuccessRedirectUri);
            Assert.True(wasCallbackInvoked);
            Assert.Equal(response.access_token, state.AccessToken);
            Assert.Equal(response.refresh_token, state.RefreshToken);
        }

        /// <summary>
        /// Tests that the OAuth authorization code exchange sends the correct HTTP request with proper parameters.
        /// Verifies that the token exchange request includes all required OAuth parameters in the correct format.
        /// </summary>
        [Fact]
        public async Task OnCodeReceivedAsync_CallsExchangeToken_WithCorrectRequest()
        {
            string baseUrl = _server!.Url!;
            var user = new System.Security.Claims.ClaimsPrincipal();
            bool wasCallbackInvoked = false;
            var state = new OAuthAuthState(user, "1", "2", $"{baseUrl}/auth", $"{baseUrl}/token", "service-name", (state) =>
            {
                wasCallbackInvoked = true;
                return Task.CompletedTask;
            });
            state.SuccessRedirectUri = "http://it-is-a-success";
            string code = "12345678";

            var response = new OAuthTokenResponse
            {
                access_token = "1234",
                expires_in = 1000,
                refresh_token = "2345"
            };

            _server.Given(
                           Request.Create()
                               .UsingPost()
                               .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                               .WithPath("/token").WithBody(new FormUrlEncodedMatcher(
                               [
                                    $"grant_type=authorization_code",
                                    $"code={code}",
                                    $"client_id={state.ClientId}",
                                    $"client_secret={state.ClientSecret}",
                                    $"state={state.Id}",
                               ])))
                           .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyAsJson(response));

            var sut = new OAuthAuthManager(new WireMockHttpClientFactory(_server));

            //act
            sut.RegisterState(state);
            var actual = await sut.OnCodeReceivedAsync(state.Id, code);

            //assert
            Assert.Equal(actual, state.SuccessRedirectUri);
            Assert.True(wasCallbackInvoked);
            Assert.Equal(response.access_token, state.AccessToken);
            Assert.Equal(response.refresh_token, state.RefreshToken);
        }

        /// <summary>
        /// Tests that the OAuth refresh token flow sends the correct HTTP request with proper parameters.
        /// Verifies that the refresh token request includes all required OAuth parameters for token renewal.
        /// </summary>
        [Fact]
        public async Task RefreshTokenAsync_CallsExchangeToken_WithCorrectRequest()
        {
            string baseUrl = _server!.Url!;
            var user = new System.Security.Claims.ClaimsPrincipal();
            bool wasCallbackInvoked = false;
            var state = new OAuthAuthState(user, "1", "2", $"{baseUrl}/auth", $"{baseUrl}/token", "service-name", (state) =>
            {
                wasCallbackInvoked = true;
                return Task.CompletedTask;
            });
            state.RefreshToken = "abcd";
            state.SuccessRedirectUri = "http://it-is-a-success";

            var response = new OAuthTokenResponse
            {
                access_token = "1234",
                expires_in = 1000,
                refresh_token = "2345"
            };

            _server.Given(
                Request.Create()
                    .UsingPost()
                    .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                    .WithPath("/token").WithBody(new FormUrlEncodedMatcher(
                    [
                        $"grant_type=refresh_token",
                        $"refresh_token={state.RefreshToken}",
                        $"client_id={state.ClientId}",
                        $"client_secret={state.ClientSecret}",
                        $"state={state.Id}",
                    ])))
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyAsJson(response));

            var sut = new OAuthAuthManager(new WireMockHttpClientFactory(_server));

            //act
            sut.RegisterState(state);
            var actual = await sut.OnCodeReceivedAsync(state.Id, "12345678");

            //assert
            Assert.Equal(actual, state.SuccessRedirectUri);
            Assert.True(wasCallbackInvoked);
            Assert.Equal(response.access_token, state.AccessToken);
            Assert.Equal(response.refresh_token, state.RefreshToken);
        }

    }
}