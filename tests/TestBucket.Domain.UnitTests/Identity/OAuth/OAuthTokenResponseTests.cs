using System.Text.Json;
using TestBucket.Domain.Identity.OAuth;
using TestBucket.Traits.Xunit;
using Xunit;

namespace TestBucket.Domain.UnitTests.Identity.OAuth
{
    /// <summary>
    /// Parsing tests for OAuthTokenResponse
    /// </summary>
    [UnitTest]
    [Feature("OAuth")]
    [Component("Identity")]
    [FunctionalTest]
    [EnrichedTest]
    public class OAuthTokenResponseTests
    {
        /// <summary>
        /// Verifies that OAuthTokenResponse can be deserialized from JSON correctly.
        /// </summary>
        [Fact]
        public void Deserialization_FromJson_WorksCorrectly()
        {
            // Arrange
            var json = """
                {
                    "access_token": "test-access-token",
                    "refresh_token": "test-refresh-token",
                    "token_type": "Bearer",
                    "expires_in": 3600,
                    "scope": "read write delete"
                }
                """;

            // Act
            var response = JsonSerializer.Deserialize<OAuthTokenResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            Assert.NotNull(response);
            Assert.Equal("test-access-token", response.access_token);
            Assert.Equal("test-refresh-token", response.refresh_token);
            Assert.Equal("Bearer", response.token_type);
            Assert.Equal(3600, response.expires_in);
            Assert.Equal("read write delete", response.scope);
        }

        /// <summary>
        /// Verifies that OAuthTokenResponse can be serialized to JSON correctly.
        /// </summary>
        [Fact]
        public void Serialization_ToJson_WorksCorrectly()
        {
            // Arrange
            var response = new OAuthTokenResponse
            {
                access_token = "test-access-token",
                refresh_token = "test-refresh-token",
                token_type = "Bearer",
                expires_in = 3600,
                scope = "read write delete"
            };

            // Act
            var json = JsonSerializer.Serialize(response);

            // Assert
            Assert.Contains("\"access_token\":\"test-access-token\"", json);
            Assert.Contains("\"refresh_token\":\"test-refresh-token\"", json);
            Assert.Contains("\"token_type\":\"Bearer\"", json);
            Assert.Contains("\"expires_in\":3600", json);
            Assert.Contains("\"scope\":\"read write delete\"", json);
        }

        /// <summary>
        /// Verifies that OAuthTokenResponse handles null values correctly.
        /// </summary>
        [Fact]
        public void Deserialization_WithNullValues_WorksCorrectly()
        {
            // Arrange
            var json = """
                {
                    "access_token": null,
                    "refresh_token": null,
                    "token_type": null,
                    "expires_in": 0,
                    "scope": null
                }
                """;

            // Act
            var response = JsonSerializer.Deserialize<OAuthTokenResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            Assert.NotNull(response);
            Assert.Null(response.access_token);
            Assert.Null(response.refresh_token);
            Assert.Null(response.token_type);
            Assert.Equal(0, response.expires_in);
            Assert.Null(response.scope);
        }

        /// <summary>
        /// Verifies that OAuthTokenResponse handles missing properties correctly.
        /// </summary>
        [Fact]
        public void Deserialization_WithMissingProperties_WorksCorrectly()
        {
            // Arrange
            var json = """
                {
                    "access_token": "test-token"
                }
                """;

            // Act
            var response = JsonSerializer.Deserialize<OAuthTokenResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            Assert.NotNull(response);
            Assert.Equal("test-token", response.access_token);
            Assert.Null(response.refresh_token);
            Assert.Null(response.token_type);
            Assert.Equal(0, response.expires_in);
            Assert.Null(response.scope);
        }

        /// <summary>
        /// Verifies that OAuthTokenResponse works with case-insensitive property matching.
        /// </summary>
        [Fact]
        public void Deserialization_CaseInsensitive_WorksCorrectly()
        {
            // Arrange
            var json = """
                {
                    "ACCESS_TOKEN": "test-access-token",
                    "Refresh_Token": "test-refresh-token",
                    "Token_Type": "Bearer",
                    "EXPIRES_IN": 7200,
                    "SCOPE": "admin"
                }
                """;

            // Act
            var response = JsonSerializer.Deserialize<OAuthTokenResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            Assert.NotNull(response);
            Assert.Equal("test-access-token", response.access_token);
            Assert.Equal("test-refresh-token", response.refresh_token);
            Assert.Equal("Bearer", response.token_type);
            Assert.Equal(7200, response.expires_in);
            Assert.Equal("admin", response.scope);
        }

        /// <summary>
        /// Verifies that OAuthTokenResponse can be instantiated with default values.
        /// </summary>
        [Fact]
        public void DefaultConstructor_SetsDefaultValues()
        {
            // Act
            var response = new OAuthTokenResponse();

            // Assert
            Assert.Null(response.access_token);
            Assert.Null(response.refresh_token);
            Assert.Null(response.token_type);
            Assert.Equal(0, response.expires_in);
            Assert.Null(response.scope);
        }

        /// <summary>
        /// Verifies that OAuthTokenResponse properties can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void Properties_CanBeSetAndRetrieved()
        {
            // Arrange
            var response = new OAuthTokenResponse();

            // Act
            response.access_token = "new-access-token";
            response.refresh_token = "new-refresh-token";
            response.token_type = "Bearer";
            response.expires_in = 1800;
            response.scope = "read write";

            // Assert
            Assert.Equal("new-access-token", response.access_token);
            Assert.Equal("new-refresh-token", response.refresh_token);
            Assert.Equal("Bearer", response.token_type);
            Assert.Equal(1800, response.expires_in);
            Assert.Equal("read write", response.scope);
        }
    }
}