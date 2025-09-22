using System.Security.Claims;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.UnitTests.Fakes;

namespace TestBucket.Domain.UnitTests.Fields;

/// <summary>
/// Unit tests for the <see cref="FieldDefinitionManager"/> class.
/// </summary>
[Feature("Fields")]
[UnitTest]
[Component("Fields")]
[EnrichedTest]
[FunctionalTest]
public class FieldDefinitionManagerTests
{
    private const string TenantId = "tenant-1";

    /// <summary>
    /// Creates the System Under Test (SUT) for <see cref="FieldDefinitionManager"/>.
    /// </summary>
    /// <param name="fields">Optional field definitions to prepopulate the repository.</param>
    /// <returns>An instance of <see cref="FieldDefinitionManager"/> with dependencies injected.</returns>
    private async Task<FieldDefinitionManager> CreateSutAsync(params FieldDefinition[] fields)
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddLogging();
        services.AddSingleton<IFieldRepository, FakeFieldRepository>();
        services.AddSingleton<FieldDefinitionManager>();
        var serviceProvider = services.BuildServiceProvider();
        var repo = serviceProvider.GetRequiredService<IFieldRepository>();
        foreach (var fieldDefinition in fields)
        {
            fieldDefinition.TenantId = TenantId;
            await repo.AddAsync(fieldDefinition);
        }

        return serviceProvider.GetRequiredService<FieldDefinitionManager>();
    }

    /// <summary>
    /// Tests that <see cref="FieldDefinitionManager.AddAsync"/> adds a field definition successfully.
    /// </summary>
    [Fact]
    public async Task AddAsync_ShouldAddFieldDefinition()
    {
        // Arrange
        var sut = await CreateSutAsync();
        var principal = GetUserWithPermissions(PermissionLevel.All);
        var fieldDefinition = new FieldDefinition
        {
            Name = "Test Field",
            TenantId = TenantId,
            Target = FieldTarget.Issue
        };

        // Act
        await sut.AddAsync(principal, fieldDefinition);

        // Assert
        var definitions = await sut.GetDefinitionsAsync(principal, null, FieldTarget.Issue);
        Assert.Single(definitions);
    }

    /// <summary>
    /// Tests that <see cref="FieldDefinitionManager.UpdateAsync"/> updates a field definition successfully.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldUpdateFieldDefinition()
    {
        // Arrange
        var fieldDefinition = new FieldDefinition
        {
            Name = "New Field",
            Target = FieldTarget.Requirement
        };
        var sut = await CreateSutAsync(fieldDefinition);
        var principal = GetUserWithPermissions(PermissionLevel.All);

        var fieldDefinitionUpdate = new FieldDefinition
        {
            Id = fieldDefinition.Id,
            Name = "Updated Field",
            Target = FieldTarget.Requirement
        };

        // Act
        await sut.UpdateAsync(principal, fieldDefinitionUpdate);

        // Assert
        // Verify that the field was updated
        var definitions = await sut.GetDefinitionsAsync(principal, null, FieldTarget.Requirement);
        Assert.Single(definitions);
        Assert.Equal(fieldDefinitionUpdate.Name, definitions[0].Name);
    }

    /// <summary>
    /// Tests that <see cref="FieldDefinitionManager.DeleteAsync"/> removes a field definition successfully.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldRemoveFieldDefinition()
    {
        // Arrange
        var fieldDefinition = new FieldDefinition
        {
            Id = 1,
            Name = "New Field",
            Target = FieldTarget.Requirement
        };
        var sut = await CreateSutAsync(fieldDefinition);

        var principal = GetUserWithPermissions(PermissionLevel.All);

        // Act
        await sut.DeleteAsync(principal, fieldDefinition);

        // Assert
        // Verify that the field was removed
        var definitions = await sut.GetDefinitionsAsync(principal, null, FieldTarget.Requirement);
        Assert.Empty(definitions);
    }

    /// <summary>
    /// Tests that cache is invalidated when a field is added.
    /// </summary>
    [Fact]
    public async Task AddAsync_ShouldInvalidateCache()
    {
        // Arrange
        var memoryCache = Substitute.For<IMemoryCache>();
        var sut = await CreateSutWithMockedCacheAsync(memoryCache);
        var principal = GetUserWithPermissions(PermissionLevel.All);
        var fieldDefinition = new FieldDefinition
        {
            Name = "Test Field",
            TenantId = TenantId,
            Target = FieldTarget.Issue
        };

        // Act
        await sut.AddAsync(principal, fieldDefinition);

        // Assert
        memoryCache.Received(1).Remove(Arg.Any<object>());
    }

    /// <summary>
    /// Tests that cache is invalidated when a field is updated.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldInvalidateCache()
    {
        // Arrange
        var memoryCache = Substitute.For<IMemoryCache>();
        var fieldDefinition = new FieldDefinition
        {
            Name = "New Field",
            Target = FieldTarget.Requirement
        };
        var sut = await CreateSutWithMockedCacheAsync(memoryCache, fieldDefinition);
        var principal = GetUserWithPermissions(PermissionLevel.All);

        var fieldDefinitionUpdate = new FieldDefinition
        {
            Id = fieldDefinition.Id,
            Name = "Updated Field",
            Target = FieldTarget.Requirement
        };

        // Act
        await sut.UpdateAsync(principal, fieldDefinitionUpdate);

        // Assert
        memoryCache.Received(1).Remove(Arg.Any<object>());
    }

    /// <summary>
    /// Tests that cache is invalidated when a field is deleted.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldInvalidateCache()
    {
        // Arrange
        var memoryCache = Substitute.For<IMemoryCache>();
        var fieldDefinition = new FieldDefinition
        {
            Id = 1,
            Name = "New Field",
            Target = FieldTarget.Requirement
        };
        var sut = await CreateSutWithMockedCacheAsync(memoryCache, fieldDefinition);
        var principal = GetUserWithPermissions(PermissionLevel.All);

        // Act
        await sut.DeleteAsync(principal, fieldDefinition);

        // Assert
        memoryCache.Received(1).Remove(Arg.Any<object>());
    }

    /// <summary>
    /// Creates the System Under Test (SUT) with a mocked memory cache.
    /// </summary>
    /// <param name="memoryCache">The mocked memory cache.</param>
    /// <param name="fields">Optional field definitions to prepopulate the repository.</param>
    /// <returns>An instance of <see cref="FieldDefinitionManager"/> with dependencies injected.</returns>
    private async Task<FieldDefinitionManager> CreateSutWithMockedCacheAsync(IMemoryCache memoryCache, params FieldDefinition[] fields)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IFieldRepository, FakeFieldRepository>();
        services.AddSingleton(memoryCache);
        services.AddSingleton<FieldDefinitionManager>();
        var serviceProvider = services.BuildServiceProvider();
        var repo = serviceProvider.GetRequiredService<IFieldRepository>();
        foreach (var fieldDefinition in fields)
        {
            fieldDefinition.TenantId = TenantId;
            await repo.AddAsync(fieldDefinition);
        }

        return serviceProvider.GetRequiredService<FieldDefinitionManager>();
    }

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> with the specified permissions.
    /// </summary>
    /// <param name="level">The permission level to assign to the user.</param>
    /// <returns>A <see cref="ClaimsPrincipal"/> with the specified permissions.</returns>
    private ClaimsPrincipal GetUserWithPermissions(PermissionLevel level)
    {
        return Impersonation.Impersonate(x =>
        {
            x.TenantId = TenantId;
            x.Add(PermissionEntityType.Project, level);
        });
    }

    /// <summary>
    /// Tests that field definitions are cached after the first retrieval.
    /// </summary>
    [Fact]
    public async Task GetDefinitionsAsync_ShouldCacheFieldDefinitions()
    {
        // Arrange
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var fieldDefinition = new FieldDefinition
        {
            Name = "Cached Field",
            Target = FieldTarget.Issue
        };
        var sut = await CreateSutWithMockedCacheAsync(memoryCache, fieldDefinition);
        var principal = GetUserWithPermissions(PermissionLevel.All);

        // Act
        var definitions1 = await sut.GetDefinitionsAsync(principal, null, FieldTarget.Issue);
        var definitions2 = await sut.GetDefinitionsAsync(principal, null, FieldTarget.Issue);

        // Assert
        Assert.Single(definitions1);
        Assert.Single(definitions2);
        Assert.True(ReferenceEquals(definitions1, definitions2) || definitions1.SequenceEqual(definitions2));
    }

    /// <summary>
    /// Tests that <see cref="FieldDefinitionManager.GetOptionsAsync"/> returns options defined on the field itself.
    /// </summary>
    [Fact]
    public async Task GetOptionsAsync_ShouldReturnFieldDefinedOptions()
    {
        // Arrange
        var fieldDefinition = new FieldDefinition
        {
            Name = "Priority",
            Target = FieldTarget.Issue,
            Options = new List<string> { "High", "Medium", "Low" }
        };
        var sut = await CreateSutAsync(fieldDefinition);
        var principal = GetUserWithPermissions(PermissionLevel.All);

        // Act
        var options = await sut.GetOptionsAsync(principal, fieldDefinition);

        // Assert
        Assert.NotNull(options);
        Assert.Equal(3, options.Count);
        Assert.Contains(options, o => o.Title == "High");
        Assert.Contains(options, o => o.Title == "Medium");
        Assert.Contains(options, o => o.Title == "Low");
    }

    /// <summary>
    /// Tests that <see cref="FieldDefinitionManager.GetOptionsAsync"/> returns an empty list when no options are defined.
    /// </summary>
    [Fact]
    public async Task GetOptionsAsync_ShouldReturnEmptyList_WhenNoOptionsDefined()
    {
        // Arrange
        var fieldDefinition = new FieldDefinition
        {
            Name = "EmptyOptions",
            Target = FieldTarget.Issue,
            Options = null
        };
        var sut = await CreateSutAsync(fieldDefinition);
        var principal = GetUserWithPermissions(PermissionLevel.All);

        // Act
        var options = await sut.GetOptionsAsync(principal, fieldDefinition);

        // Assert
        Assert.NotNull(options);
        Assert.Empty(options);
    }

    /// <summary>
    /// Verifies that clearing the cache calls Remove with the expected cache key
    /// </summary>
    [Fact]
    public void ClearTenantCache_ShouldRemoveCacheEntry_ForGivenTenantId()
    {
        // Arrange
        var memoryCache = Substitute.For<IMemoryCache>();
        var completionProviders = Array.Empty<IFieldCompletionsProvider>();
        var fieldRepository = Substitute.For<IFieldRepository>();
        var sut = new FieldDefinitionManager(memoryCache, completionProviders, fieldRepository);
        var tenantId = "tenant-123";
        var expectedCacheKey = "fielddefinitions:" + tenantId;

        // Act
        sut.ClearTenantCache(tenantId);

        // Assert
        memoryCache.Received(1).Remove(expectedCacheKey);
    }

    /// <summary>
    /// Verifies that there's no exception when there's no cache entry for the given tenant ID
    /// </summary>
    [Fact]
    public void ClearTenantCache_ShouldNotThrow_WhenCacheKeyDoesNotExist()
    {
        // Arrange
        var memoryCache = Substitute.For<IMemoryCache>();
        var completionProviders = Array.Empty<IFieldCompletionsProvider>();
        var fieldRepository = Substitute.For<IFieldRepository>();
        var sut = new FieldDefinitionManager(memoryCache, completionProviders, fieldRepository);
        var tenantId = "nonexistent-tenant";

        // Act & Assert
        var exception = Record.Exception(() => sut.ClearTenantCache(tenantId));
        Assert.Null(exception);
        memoryCache.Received(1).Remove("fielddefinitions:" + tenantId);
    }
}