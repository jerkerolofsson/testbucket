using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using NSubstitute;

using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Integrations;
using TestBucket.Domain.ExtensionManagement;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.Projects.Models;
using TestBucket.Traits.Core;

using Xunit;

namespace TestBucket.Domain.UnitTests.ExtensionsManagement;

/// <summary>
/// Tests for <see cref="ExtensionFieldCompletionsAggregator"/>
/// </summary>
[EnrichedTest]
[UnitTest]
[Component("Extension Management")]
[FunctionalTest]
public class ExtensionFieldCompletionsAggregatorTests
{
    /// <summary>
    /// Verifies that <see cref="ExtensionFieldCompletionsAggregator.GetFieldOptionsAsync"/> returns options from the first matching data source.
    /// </summary>
    [Fact]
    public async Task GetFieldOptionsAsync_ReturnsOptionsFromFirstMatchingDataSource()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        var projectId = 1L;
        var traitType = TraitType.Release;
        var cancellationToken = CancellationToken.None;

        var integration = new ExternalSystem
        {
            Name = "TestSystem",
            Provider = "TestSystem",
            Enabled = true,
            EnabledCapabilities = ExternalSystemCapability.GetReleases,
            SupportedCapabilities = ExternalSystemCapability.GetReleases
        };

        var integrationDto = integration.ToDto();

        var projectManager = Substitute.For<IProjectManager>();
        projectManager.GetProjectIntegrationsAsync(principal, projectId)
            .Returns(Task.FromResult<IReadOnlyList<ExternalSystem>>(new[] { integration }));

        var dataSource = Substitute.For<IExternalProjectDataSource>();
        dataSource.SupportedTraits.Returns(new[] { traitType });
        dataSource.SystemName.Returns("TestSystem");
        dataSource.GetFieldOptionsAsync(integrationDto, traitType, cancellationToken)
            .Returns(Task.FromResult(new[] { new GenericVisualEntity { Title = "Option1" } }));

        var aggregator = new ExtensionFieldCompletionsAggregator(projectManager, new[] { dataSource });

        // Act
        var result = await aggregator.GetFieldOptionsAsync(principal, projectId, traitType, cancellationToken);

        // Assert
        Assert.Single(result);
        Assert.Equal("Option1", result[0].Title);
    }

    /// <summary>
    /// Verifies that <see cref="ExtensionFieldCompletionsAggregator.GetFieldOptionsAsync"/> skips data sources if capability is not enabled.
    /// </summary>
    [Fact]
    public async Task GetFieldOptionsAsync_SkipsDataSourceIfCapabilityNotEnabled()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        var projectId = 1L;
        var traitType = TraitType.Release;
        var cancellationToken = CancellationToken.None;

        var integration = new ExternalSystem
        {
            Name = "TestSystem",
            Provider = "TestSystem",
            Enabled = true,
            EnabledCapabilities = ExternalSystemCapability.GetMilestones, // Not GetReleases
            SupportedCapabilities = ExternalSystemCapability.GetReleases
        };

        var projectManager = Substitute.For<IProjectManager>();
        projectManager.GetProjectIntegrationsAsync(principal, projectId)
            .Returns(Task.FromResult<IReadOnlyList<ExternalSystem>>(new[] { integration }));

        var dataSource = Substitute.For<IExternalProjectDataSource>();
        dataSource.SupportedTraits.Returns(new[] { traitType });
        dataSource.SystemName.Returns("TestSystem");

        var aggregator = new ExtensionFieldCompletionsAggregator(projectManager, new[] { dataSource });

        // Act
        var result = await aggregator.GetFieldOptionsAsync(principal, projectId, traitType, cancellationToken);

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that <see cref="ExtensionFieldCompletionsAggregator.GetFieldOptionsAsync"/> returns empty if no data source matches.
    /// </summary>
    [Fact]
    public async Task GetFieldOptionsAsync_ReturnsEmptyIfNoDataSourceMatches()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        var projectId = 1L;
        var traitType = TraitType.Release;
        var cancellationToken = CancellationToken.None;

        var integration = new ExternalSystem
        {
            Name = "TestSystem",
            Provider = "TestSystem",
            Enabled = true,
            EnabledCapabilities = ExternalSystemCapability.GetReleases,
            SupportedCapabilities = ExternalSystemCapability.GetReleases
        };

        var projectManager = Substitute.For<IProjectManager>();
        projectManager.GetProjectIntegrationsAsync(principal, projectId)
            .Returns(Task.FromResult<IReadOnlyList<ExternalSystem>>(new[] { integration }));

        var dataSource = Substitute.For<IExternalProjectDataSource>();
        dataSource.SupportedTraits.Returns(Array.Empty<TraitType>());
        dataSource.SystemName.Returns("TestSystem");

        var aggregator = new ExtensionFieldCompletionsAggregator(projectManager, new[] { dataSource });

        // Act
        var result = await aggregator.GetFieldOptionsAsync(principal, projectId, traitType, cancellationToken);

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that <see cref="ExtensionFieldCompletionsAggregator.GetFieldOptionsAsync"/> skips data sources if integration is not enabled.
    /// </summary>
    [Fact]
    public async Task GetFieldOptionsAsync_SkipsDataSourceIfIntegrationNotEnabled()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        var projectId = 1L;
        var traitType = TraitType.Release;
        var cancellationToken = CancellationToken.None;

        var integration = new ExternalSystem
        {
            Name = "TestSystem",
            Provider = "TestSystem",
            Enabled = false, // Not enabled
            EnabledCapabilities = ExternalSystemCapability.GetReleases,
            SupportedCapabilities = ExternalSystemCapability.GetReleases
        };

        var projectManager = Substitute.For<IProjectManager>();
        projectManager.GetProjectIntegrationsAsync(principal, projectId)
            .Returns(Task.FromResult<IReadOnlyList<ExternalSystem>>(new[] { integration }));

        var dataSource = Substitute.For<IExternalProjectDataSource>();
        dataSource.SupportedTraits.Returns(new[] { traitType });
        dataSource.SystemName.Returns("TestSystem");

        var aggregator = new ExtensionFieldCompletionsAggregator(projectManager, new[] { dataSource });

        // Act
        var result = await aggregator.GetFieldOptionsAsync(principal, projectId, traitType, cancellationToken);

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that <see cref="ExtensionFieldCompletionsAggregator.GetFieldOptionsAsync"/> continues to next data source if exception is thrown.
    /// </summary>
    [Fact]
    public async Task GetFieldOptionsAsync_ContinuesOnException()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        var projectId = 1L;
        var traitType = TraitType.Release;
        var cancellationToken = CancellationToken.None;

        var integration = new ExternalSystem
        {
            Name = "TestSystem",
            Provider = "TestSystem",
            Enabled = true,
            EnabledCapabilities = ExternalSystemCapability.GetReleases,
            SupportedCapabilities = ExternalSystemCapability.GetReleases
        };

        var integrationDto = integration.ToDto();

        var projectManager = Substitute.For<IProjectManager>();
        projectManager.GetProjectIntegrationsAsync(principal, projectId)
            .Returns(Task.FromResult<IReadOnlyList<ExternalSystem>>(new[] { integration }));

        var dataSource1 = Substitute.For<IExternalProjectDataSource>();
        dataSource1.SupportedTraits.Returns(new[] { traitType });
        dataSource1.SystemName.Returns("TestSystem");
        dataSource1.GetFieldOptionsAsync(integrationDto, traitType, cancellationToken)
            .Returns<Task<GenericVisualEntity[]>>(x => throw new Exception("fail"));

        var dataSource2 = Substitute.For<IExternalProjectDataSource>();
        dataSource2.SupportedTraits.Returns(new[] { traitType });
        dataSource2.SystemName.Returns("TestSystem");
        dataSource2.GetFieldOptionsAsync(integrationDto, traitType, cancellationToken)
            .Returns(Task.FromResult(new[] { new GenericVisualEntity { Title = "Option2" } }));

        var aggregator = new ExtensionFieldCompletionsAggregator(projectManager, new[] { dataSource1, dataSource2 });

        // Act
        var result = await aggregator.GetFieldOptionsAsync(principal, projectId, traitType, cancellationToken);

        // Assert
        Assert.Single(result);
        Assert.Equal("Option2", result[0].Title);
    }
}