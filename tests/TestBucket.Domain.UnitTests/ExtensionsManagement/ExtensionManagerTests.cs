using NSubstitute;

using TestBucket.Contracts.Integrations;
using TestBucket.Domain.ExtensionManagement;

namespace TestBucket.Domain.UnitTests.ExtensionsManagement;

/// <summary>
/// Unit tests for the <see cref="ExtensionManager"/> class.
/// </summary>
[EnrichedTest]
[UnitTest]
[Component("Extensions Management")]
[FunctionalTest]
public class ExtensionManagerTests
{
    /// <summary>
    /// Verifies that the constructor initializes the manager with the provided extensions.
    /// </summary>
    [Fact]
    public void Constructor_ShouldInitializeWithExtensions()
    {
        // Arrange
        var extension1 = Substitute.For<IExtension>();
        extension1.SystemName.Returns("Extension1");

        var extension2 = Substitute.For<IExtension>();
        extension2.SystemName.Returns("Extension2");

        var extensions = new List<IExtension> { extension1, extension2 };

        // Act
        var manager = new ExtensionManager(extensions);

        // Assert
        Assert.Equal(2, manager.GetExtensions().Count);
    }

    /// <summary>
    /// Verifies that the constructor handles an empty list of extensions.
    /// </summary>
    [Fact]
    public void Constructor_ShouldHandleEmptyExtensions()
    {
        // Arrange
        var extensions = new List<IExtension>();

        // Act
        var manager = new ExtensionManager(extensions);

        // Assert
        Assert.Empty(manager.GetExtensions());
    }

    /// <summary>
    /// Verifies that <see cref="ExtensionManager.FindExtension"/> returns the correct extension when the system name matches.
    /// </summary>
    [Fact]
    public void FindExtension_ShouldReturnExtension_WhenSystemNameMatches()
    {
        // Arrange
        var extension = Substitute.For<IExtension>();
        extension.SystemName.Returns("Extension1");

        var extensions = new List<IExtension> { extension };
        var manager = new ExtensionManager(extensions);

        // Act
        var result = manager.FindExtension("Extension1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Extension1", result.SystemName);
    }

    /// <summary>
    /// Verifies that <see cref="ExtensionManager.FindExtension"/> returns null when no extension matches the system name.
    /// </summary>
    [Fact]
    public void FindExtension_ShouldReturnNull_WhenSystemNameDoesNotMatch()
    {
        // Arrange
        var extension = Substitute.For<IExtension>();
        extension.SystemName.Returns("Extension1");

        var extensions = new List<IExtension> { extension };
        var manager = new ExtensionManager(extensions);

        // Act
        var result = manager.FindExtension("NonExistent");

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that <see cref="ExtensionManager.GetIcon"/> returns the correct icon when the system name matches.
    /// </summary>
    [Fact]
    public void GetIcon_ShouldReturnIcon_WhenSystemNameMatches()
    {
        // Arrange
        var extension = Substitute.For<IExtension>();
        extension.SystemName.Returns("Extension1");
        extension.Icon.Returns("Icon1");

        var extensions = new List<IExtension> { extension };
        var manager = new ExtensionManager(extensions);

        // Act
        var result = manager.GetIcon("Extension1");

        // Assert
        Assert.Equal("Icon1", result);
    }

    /// <summary>
    /// Verifies that <see cref="ExtensionManager.GetIcon"/> returns the TestBucket icon when the system name is "Test Bucket".
    /// </summary>
    [Fact]
    public void GetIcon_ShouldReturnTestBucketIcon_WhenSystemNameIsTestBucket()
    {
        // Arrange
        var manager = new ExtensionManager(new List<IExtension>());

        // Act
        var result = manager.GetIcon("Test Bucket");

        // Assert
        Assert.Equal(TbIcons.Brands.TestBucket, result);
    }

    /// <summary>
    /// Verifies that <see cref="ExtensionManager.GetIcon"/> returns null when no extension matches the system name.
    /// </summary>
    [Fact]
    public void GetIcon_ShouldReturnNull_WhenSystemNameDoesNotExist()
    {
        // Arrange
        var manager = new ExtensionManager(new List<IExtension>());

        // Act
        var result = manager.GetIcon("NonExistent");

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that <see cref="ExtensionManager.GetExtensions"/> returns all extensions.
    /// </summary>
    [Fact]
    public void GetExtensions_ShouldReturnAllExtensions()
    {
        // Arrange
        var extension1 = Substitute.For<IExtension>();
        extension1.SystemName.Returns("Extension1");

        var extension2 = Substitute.For<IExtension>();
        extension2.SystemName.Returns("Extension2");

        var extensions = new List<IExtension> { extension1, extension2 };
        var manager = new ExtensionManager(extensions);

        // Act
        var result = manager.GetExtensions();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, e => e.SystemName == "Extension1");
        Assert.Contains(result, e => e.SystemName == "Extension2");
    }
}