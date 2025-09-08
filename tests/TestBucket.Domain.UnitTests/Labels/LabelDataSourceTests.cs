using System.Security.Claims;

using NSubstitute;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Labels;
using TestBucket.Domain.Labels.DataSources;
using TestBucket.Domain.Labels.Models;
namespace TestBucket.Domain.UnitTests.Labels;

/// <summary>
/// Tests for LabelDataSource.
/// </summary>
[EnrichedTest]
[UnitTest]
[Component("Labels")]
[FunctionalTest]
public class LabelDataSourceTests
{
    private readonly ILabelManager _mockLabelManager;
    private readonly LabelDataSource _dataSource;
    private readonly ClaimsPrincipal _principal;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelDataSourceTests"/> class.
    /// Sets up mock dependencies and test data.
    /// </summary>
    public LabelDataSourceTests()
    {
        _mockLabelManager = Substitute.For<ILabelManager>();
        _dataSource = new LabelDataSource(_mockLabelManager);
        _principal = new ClaimsPrincipal();
    }

    /// <summary>
    /// Tests that <see cref="LabelDataSource.GetOptionsAsync"/> returns labels when the type is <see cref="FieldDataSourceType.Labels"/>.
    /// </summary>
    [Fact]
    public async Task GetOptionsAsync_ShouldReturnLabels_WhenTypeIsLabels()
    {
        // Arrange
        var labels = new List<Label>
        {
            new Label { Title = "Bug", Description = "Bug label", Color = "Red" },
            new Label { Title = "Feature", Description = "Feature label", Color = "Blue" }
        };
        _mockLabelManager.GetLabelsAsync(_principal, 1).Returns(labels);

        // Act
        var result = await _dataSource.GetOptionsAsync(_principal, FieldDataSourceType.Labels, 1, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Title == "Bug" && x.Color == "Red");
        Assert.Contains(result, x => x.Title == "Feature" && x.Color == "Blue");
    }

    /// <summary>
    /// Tests that <see cref="LabelDataSource.GetOptionsAsync"/> returns an empty list when the type is not <see cref="FieldDataSourceType.Labels"/>.
    /// </summary>
    [Fact]
    public async Task GetOptionsAsync_ShouldReturnEmpty_WhenTypeIsNotLabels()
    {
        // Act
        var result = await _dataSource.GetOptionsAsync(_principal, FieldDataSourceType.Commit, 1, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Tests that <see cref="LabelDataSource.SearchOptionsAsync"/> returns filtered labels when the type is <see cref="FieldDataSourceType.Labels"/>.
    /// </summary>
    [Fact]
    public async Task SearchOptionsAsync_ShouldReturnFilteredLabels_WhenTypeIsLabels()
    {
        // Arrange
        var labels = new List<Label>
        {
            new Label { Title = "Bug", Description = "Bug label", Color = "Red" },
            new Label { Title = "Feature", Description = "Feature label", Color = "Blue" }
        };
        _mockLabelManager.SearchLabelsAsync(_principal, 1, "Bug", 0, 10).Returns(labels);

        // Act
        var result = await _dataSource.SearchOptionsAsync(_principal, FieldDataSourceType.Labels, 1, "Bug", 10, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Contains(result, x => x.Title == "Bug" && x.Color == "Red");
    }

    /// <summary>
    /// Tests that <see cref="LabelDataSource.SearchOptionsAsync"/> returns an empty list when the type is not <see cref="FieldDataSourceType.Labels"/>.
    /// </summary>
    [Fact]
    public async Task SearchOptionsAsync_ShouldReturnEmpty_WhenTypeIsNotLabels()
    {
        // Act
        var result = await _dataSource.SearchOptionsAsync(_principal, FieldDataSourceType.Commit, 1, "Bug", 10, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}