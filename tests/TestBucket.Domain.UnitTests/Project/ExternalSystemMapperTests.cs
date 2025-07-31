using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.Projects.Models;

namespace TestBucket.Domain.UnitTests.Project;

/// <summary>
/// Unit tests for the ExternalSystemMapper class.
/// </summary>
[FunctionalTest]
[EnrichedTest]
[UnitTest]
[Feature("Extensions")]
[Component("Projects")]
public class ExternalSystemMapperTests
{
    /// <summary>
    /// Tests the <see cref="ExternalSystemMapper.ToDbo(ExternalSystemDto)"/> method.
    /// </summary>
    [Fact]
    public void ExternalSystem_ToDbo_MapsDtoToDboCorrectly()
    {
        // Arrange
        var dto = new ExternalSystemDto
        {
            Id = 123,
            Enabled = true,
            Name = "Test System",
            Provider = "Test Provider",
            AccessToken = "token",
            ApiKey = "key",
            ClientId = "clientId",
            ClientSecret = "clientSecret",
            BaseUrl = "http://example.com",
            ExternalProjectId = "projectId",
            ReadOnly = false
        };

        // Act
        var dbo = dto.ToDbo();

        // Assert
        Assert.Equal(dto.Id, dbo.Id);
        Assert.Equal(dto.Enabled, dbo.Enabled);
        Assert.Equal(dto.Name, dbo.Name);
        Assert.Equal(dto.Provider, dbo.Provider);
        Assert.Equal(dto.AccessToken, dbo.AccessToken);
        Assert.Equal(dto.ApiKey, dbo.ApiKey);
        Assert.Equal(dto.ClientId, dbo.ClientId);
        Assert.Equal(dto.ClientSecret, dbo.ClientSecret);
        Assert.Equal(dto.BaseUrl, dbo.BaseUrl);
        Assert.Equal(dto.ExternalProjectId, dbo.ExternalProjectId);
        Assert.Equal(dto.ReadOnly, dbo.ReadOnly);
    }

    /// <summary>
    /// Tests the <see cref="ExternalSystemMapper.ToDto(ExternalSystem)"/> method.
    /// </summary>
    [Fact]
    public void ExternalSystem_ToDto_MapsDboToDtoCorrectly()
    {
        // Arrange
        var dbo = new ExternalSystem
        {
            Id = 123,
            Enabled = true,
            Name = "Test System",
            Provider = "Test Provider",
            AccessToken = "token",
            ApiKey = "key",
            ClientId = "clientId",
            ClientSecret = "clientSecret",
            BaseUrl = "http://example.com",
            ExternalProjectId = "projectId",
            ReadOnly = false
        };

        // Act
        var dto = dbo.ToDto();

        // Assert
        Assert.Equal(dbo.Id, dto.Id);
        Assert.Equal(dbo.Enabled, dto.Enabled);
        Assert.Equal(dbo.Name, dto.Name);
        Assert.Equal(dbo.Provider, dto.Provider);
        Assert.Equal(dbo.AccessToken, dto.AccessToken);
        Assert.Equal(dbo.ApiKey, dto.ApiKey);
        Assert.Equal(dbo.ClientId, dto.ClientId);
        Assert.Equal(dbo.ClientSecret, dto.ClientSecret);
        Assert.Equal(dbo.BaseUrl, dto.BaseUrl);
        Assert.Equal(dbo.ExternalProjectId, dto.ExternalProjectId);
        Assert.Equal(dbo.ReadOnly, dto.ReadOnly);
    }
}
