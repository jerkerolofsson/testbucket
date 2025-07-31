using NSubstitute;
using System.Security.Claims;
using System.Web;
using TestBucket.Domain.Identity.OAuth;

namespace TestBucket.Domain.UnitTests.Identity.OAuth
{
    /// <summary>
    /// Tests for OAuthAuthenticator functionality
    /// </summary>
    [UnitTest]
    [Feature("OAuth")]
    [Component("Identity")]
    [FunctionalTest]
    [EnrichedTest]
    public class OAuthAuthenticatorTests
    {
        private readonly OAuthAuthManager _manager;
        private readonly OAuthAuthenticator _authenticator;
        private readonly ClaimsPrincipal _user;
        private Task OnAuthenticated(OAuthAuthState state) => Task.CompletedTask;
        private string BaseUrl => "https://localhost";


        /// <summary>
        /// 
        /// </summary>
        public OAuthAuthenticatorTests()
        {
            _manager = Substitute.For<OAuthAuthManager>(Substitute.For<IHttpClientFactory>());
            _authenticator = new OAuthAuthenticator(_manager);
            _user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim("tenant", "tenant1")
            }, "test"));
        }

        /// <summary>
        /// Verifies that AuthenticateAsync generates correct OAuth URL with all required parameters.
        /// </summary>
        [Fact]
        public async Task AuthenticateAsync_GeneratesCorrectAuthUrl_WithRequiredParameters()
        {
            // Arrange
            var clientId = "test-client-id";
            var clientSecret = "test-client-secret";
            var authEndpoint = "https://example.com/oauth/authorize";
            var tokenEndpoint = "https://example.com/oauth/token";
            var integrationName = "github";
            var callback = OnAuthenticated;

            // Act
            var result = await _authenticator.AuthenticateAsync(_user, "", clientId, clientSecret, authEndpoint, tokenEndpoint, integrationName, BaseUrl, callback);

            // Assert
            Assert.NotNull(result);
            var uri = new Uri(result);
            var query = HttpUtility.ParseQueryString(uri.Query);

            Assert.Equal("https", uri.Scheme);
            Assert.Equal("example.com", uri.Host);
            Assert.Equal("/oauth/authorize", uri.AbsolutePath);
            Assert.Equal(clientId, query["client_id"]);
            Assert.Equal("code", query["response_type"]);
            Assert.NotNull(query["state"]);
            Assert.Equal($"{BaseUrl}/api/oauth/{integrationName}", query["redirect_uri"]);
        }


        /// <summary>
        /// Verifies that AuthenticateAsync generates correct OAuth URL with all required parameters.
        /// </summary>
        [Fact]
        public async Task AuthenticateAsync_GeneratesCorrectAuthUrl_WhenBaseUrlHasTrailingSlash()
        {
            // Arrange
            var clientId = "test-client-id";
            var clientSecret = "test-client-secret";
            var authEndpoint = "https://example.com/oauth/authorize";
            var tokenEndpoint = "https://example.com/oauth/token";
            var integrationName = "github";
            var callback = OnAuthenticated;

            // Act
            var result = await _authenticator.AuthenticateAsync(_user, "", clientId, clientSecret, authEndpoint, tokenEndpoint, integrationName, BaseUrl + "/", callback);

            // Assert
            Assert.NotNull(result);
            var uri = new Uri(result);
            var query = HttpUtility.ParseQueryString(uri.Query);

            Assert.Equal("https", uri.Scheme);
            Assert.Equal("example.com", uri.Host);
            Assert.Equal("/oauth/authorize", uri.AbsolutePath);
            Assert.Equal(clientId, query["client_id"]);
            Assert.Equal("code", query["response_type"]);
            Assert.NotNull(query["state"]);
            Assert.Equal($"{BaseUrl}/api/oauth/{integrationName}", query["redirect_uri"]);
        }

        /// <summary>
        /// Verifies that AuthenticateAsync preserves existing response_type parameter if already present.
        /// </summary>
        [Fact]
        public async Task AuthenticateAsync_PreservesExistingResponseType_WhenAlreadyPresent()
        {
            // Arrange
            var clientId = "test-client-id";
            var clientSecret = "test-client-secret";
            var authEndpoint = "https://example.com/oauth/authorize?response_type=token";
            var tokenEndpoint = "https://example.com/oauth/token";
            var integrationName = "github";
            var callback = OnAuthenticated;

            // Act
            var result = await _authenticator.AuthenticateAsync(_user, "", clientId, clientSecret, authEndpoint, tokenEndpoint, integrationName, BaseUrl, callback);

            // Assert
            var uri = new Uri(result);
            var query = HttpUtility.ParseQueryString(uri.Query);
            Assert.Equal("token", query["response_type"]);
        }

        /// <summary>
        /// Verifies that AuthenticateAsync generates unique state values for different authentication requests.
        /// </summary>
        [Fact]
        public async Task AuthenticateAsync_GeneratesUniqueStateValues_ForDifferentRequests()
        {
            // Arrange
            var clientId = "test-client-id";
            var clientSecret = "test-client-secret";
            var authEndpoint = "https://example.com/oauth/authorize";
            var tokenEndpoint = "https://example.com/oauth/token";
            var integrationName = "github";
            var callback = OnAuthenticated;

            // Act
            var result1 = await _authenticator.AuthenticateAsync(_user, "", clientId, clientSecret, authEndpoint, tokenEndpoint, integrationName, BaseUrl, callback);
            var result2 = await _authenticator.AuthenticateAsync(_user, "", clientId, clientSecret, authEndpoint, tokenEndpoint, integrationName, BaseUrl, callback);

            // Assert
            var uri1 = new Uri(result1);
            var uri2 = new Uri(result2);
            var query1 = HttpUtility.ParseQueryString(uri1.Query);
            var query2 = HttpUtility.ParseQueryString(uri2.Query);

            Assert.NotEqual(query1["state"], query2["state"]);
        }

        /// <summary>
        /// Verifies that AuthenticateAsync handles auth endpoints with existing query parameters correctly.
        /// </summary>
        [Fact]
        public async Task AuthenticateAsync_HandlesExistingQueryParameters_Correctly()
        {
            // Arrange
            var clientId = "test-client-id";
            var clientSecret = "test-client-secret";
            var authEndpoint = "https://example.com/oauth/authorize?scope=read%20write&prompt=consent";
            var tokenEndpoint = "https://example.com/oauth/token";
            var integrationName = "github";
            var callback = OnAuthenticated;

            // Act
            var result = await _authenticator.AuthenticateAsync(_user, "read write", clientId, clientSecret, authEndpoint, tokenEndpoint, integrationName, BaseUrl, callback);

            // Assert
            var uri = new Uri(result);
            var query = HttpUtility.ParseQueryString(uri.Query);

            Assert.Equal("read write", query["scope"]);
            Assert.Equal("consent", query["prompt"]);
            Assert.Equal(clientId, query["client_id"]);
            Assert.Equal("code", query["response_type"]);
            Assert.NotNull(query["state"]);
            Assert.Equal($"{BaseUrl}/api/oauth/{integrationName}", query["redirect_uri"]);
        }

        /// <summary>
        /// Verifies that AuthenticateAsync works with different integration names.
        /// </summary>
        [Theory]
        [InlineData("github")]
        [InlineData("gitlab")]
        [InlineData("azure")]
        [InlineData("google")]
        public async Task AuthenticateAsync_WorksWithDifferentIntegrationNames(string integrationName)
        {
            // Arrange
            var clientId = "test-client-id";
            var clientSecret = "test-client-secret";
            var authEndpoint = "https://example.com/oauth/authorize";
            var tokenEndpoint = "https://example.com/oauth/token";
            var callback = OnAuthenticated;

            // Act
            var result = await _authenticator.AuthenticateAsync(_user, "", clientId, clientSecret, authEndpoint, tokenEndpoint, integrationName, BaseUrl, callback);

            // Assert
            var uri = new Uri(result);
            var query = HttpUtility.ParseQueryString(uri.Query);
            Assert.Equal($"{BaseUrl}/api/oauth/{integrationName}", query["redirect_uri"]);
        }

        /// <summary>
        /// Verifies that AuthenticateAsync returns a task that completes successfully.
        /// </summary>
        [Fact]
        public async Task AuthenticateAsync_ReturnsCompletedTask()
        {
            // Arrange
            var clientId = "test-client-id";
            var clientSecret = "test-client-secret";
            var authEndpoint = "https://example.com/oauth/authorize";
            var tokenEndpoint = "https://example.com/oauth/token";
            var integrationName = "github";
            var callback = OnAuthenticated;

            // Act
            var task = _authenticator.AuthenticateAsync(_user, "", clientId, clientSecret, authEndpoint, tokenEndpoint, integrationName, BaseUrl, callback);

            // Assert
            Assert.True(task.IsCompleted);
            var result = await task;
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}
