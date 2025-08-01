using NSubstitute;

using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Labels;
using TestBucket.Domain.Labels.Models;

namespace TestBucket.Domain.UnitTests.Labels;

/// <summary>
/// Contains unit tests for the <see cref="LabelManager"/> class, verifying the behavior of label management functionality.
/// </summary>
[EnrichedTest]
[UnitTest]
[Component("Labels")]
public class LabelManagerTests
{
    private readonly FakeLabelRepository _fakeRepository;
    private readonly TimeProvider _mockTimeProvider;
    private readonly LabelManager _labelManager;

    private const string TenantId = "tenant-1";
    private const string UserName = "user@user.com";

    /// <summary>
    /// 
    /// </summary>
    public LabelManagerTests()
    {
        _fakeRepository = new FakeLabelRepository();
        _mockTimeProvider = Substitute.For<TimeProvider>();
        _labelManager = new LabelManager(_fakeRepository, _mockTimeProvider);
    }

    /// <summary>
    /// Verifies that <see cref="LabelManager.AddLabelAsync"/> adds a label successfully.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task AddLabelAsync_WithValidData_AddsLabelSuccessfully()
    {
        // Arrange
        var principal = Impersonation.Impersonate(builder =>
        {
            builder.TenantId = TenantId;
            builder.UserName = builder.Email = UserName;
            builder.AddAllPermissions();
        });
        var label = new Label { Title = "Test Label" };
        var time = DateTime.UtcNow;
        _mockTimeProvider.GetUtcNow().Returns(time);

        // Act
        await _labelManager.AddLabelAsync(principal, label);

        // Assert
        await _fakeRepository.Received(1).AddLabelAsync(label);
        Assert.Equal(label.Created, time);
        Assert.Equal(label.Created, label.Modified);
        Assert.Equal(TenantId, label.TenantId);
    }

    /// <summary>
    /// Verifies that <see cref="LabelManager.GetLabelsAsync"/> retrieves labels successfully.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task GetLabelsAsync_WithValidPrincipal_ReturnsLabels()
    {
        // Arrange
        var principal = Impersonation.Impersonate(builder =>
        {
            builder.TenantId = TenantId;
            builder.UserName = builder.Email = UserName;
            builder.AddAllPermissions();
        });
        var projectId = 1L;
        var labels = new List<Label> { new Label { Title = "Label1", TenantId = TenantId }, new Label { Title = "Label2", TenantId = TenantId } };
        foreach (var label in labels)
        {
            await _fakeRepository.AddLabelAsync(label);
        }

        // Act
        var result = await _labelManager.GetLabelsAsync(principal, projectId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    /// <summary>
    /// Verifies that <see cref="LabelManager.GetLabelByNameAsync"/> retrieves a label by name successfully.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task GetLabelByNameAsync_WithValidName_ReturnsLabel()
    {
        // Arrange
        var principal = Impersonation.Impersonate(builder =>
        {
            builder.TenantId = TenantId;
            builder.UserName = builder.Email = UserName;
            builder.AddAllPermissions();
        });
        var projectId = 1L;
        var labelName = "Test Label";
        var label = new Label { Title = labelName, TenantId = TenantId };
        await _fakeRepository.AddLabelAsync(label);

        // Act
        var result = await _labelManager.GetLabelByNameAsync(principal, projectId, labelName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(labelName, result?.Title);
    }


    /// <summary>
    /// Verifies that GetLabelByNameAsync throws an excpetion if the user does not have read access
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task GetLabelByNameAsync_WithoutReadPermission_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var principal = Impersonation.Impersonate(builder =>
        {
            builder.TenantId = TenantId;
            builder.UserName = builder.Email = UserName;
        });
        var projectId = 1L;
        var labelName = "Test Label";

        // Act
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _labelManager.GetLabelByNameAsync(principal, projectId, labelName));
    }

    /// <summary>
    /// Verifies that DeleteAsync deletes a label successfully.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task DeleteAsync_WithValidLabel_DeletesLabelSuccessfully()
    {
        // Arrange
        var principal = Impersonation.Impersonate(builder =>
        {
            builder.TenantId = TenantId;
            builder.UserName = builder.Email = UserName;
            builder.AddAllPermissions();
        });
        var label = new Label { Id = 1L, TenantId = TenantId };

        // Act
        await _labelManager.DeleteAsync(principal, label);

        // Assert
        await _fakeRepository.Received(1).DeleteLabelByIdAsync(label.Id);
    }


    /// <summary>
    /// Verifies that AddAsync throws an UnauthorizedAccessException if trying to add a label
    /// with a user without Issue write access
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task AddLabelAsync_WithoutWritePermission_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var principal = Impersonation.Impersonate(builder =>
        {
            builder.TenantId = TenantId;
            builder.UserName = builder.Email = UserName;

            // Only read access
            builder.Add(PermissionEntityType.Issue, PermissionLevel.Read);
        });
        var label = new Label { Id = 1L, TenantId = "other-tenant" };

        // Act
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _labelManager.AddLabelAsync(principal, label));

        // Assert
        await _fakeRepository.Received(0).AddLabelAsync(Arg.Any<Label>());
    }

    /// <summary>
    /// Verifies that UpdateAsync throws an UnauthorizedAccessException if trying to update the label
    /// with a user without Issue write access
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task UpdateLabelAsync_WithoutWritePermission_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var principal = Impersonation.Impersonate(builder =>
        {
            builder.TenantId = TenantId;
            builder.UserName = builder.Email = UserName;

            // Only read access
            builder.Add(PermissionEntityType.Issue, PermissionLevel.Read);
        });
        var label = new Label { Id = 1L, TenantId = "other-tenant" };

        // Act
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _labelManager.UpdateLabelAsync(principal, label));

        // Assert
        await _fakeRepository.Received(0).UpdateLabelAsync(Arg.Any<Label>());
    }

    /// <summary>
    /// Verifies that UpdateAsync throws an UnauthorizedAccessException if trying to update the label
    /// with a user from another tenant than the label
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task UpdateLabelAsync_WithWrongTenant_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var principal = Impersonation.Impersonate(builder =>
        {
            builder.TenantId = TenantId;
            builder.UserName = builder.Email = UserName;
            builder.AddAllPermissions();
        });
        var label = new Label { Id = 1L, TenantId = "other-tenant" };

        // Act
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _labelManager.UpdateLabelAsync(principal, label));

        // Assert
        await _fakeRepository.Received(0).UpdateLabelAsync(Arg.Any<Label>());
    }

    /// <summary>
    /// Verifies that DeleteAsync throws an UnauthorizedAccessException if trying to delete the label
    /// with a user from another tenant than the label
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task DeleteAsync_WithWrongTenant_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var principal = Impersonation.Impersonate(builder =>
        {
            builder.TenantId = TenantId;
            builder.UserName = builder.Email = UserName;
            builder.AddAllPermissions();
        });
        var label = new Label { Id = 1L, TenantId = "other-tenant" };

        // Act
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _labelManager.DeleteAsync(principal, label));

        // Assert
        await _fakeRepository.Received(0).DeleteLabelByIdAsync(label.Id);
    }

    /// <summary>
    /// Verifies that <see cref="LabelManager.UpdateLabelAsync"/> updates a label successfully.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateLabelAsync_WithValidLabel_UpdatesLabelSuccessfully()
    {
        // Arrange
        var principal = Impersonation.Impersonate(builder =>
        {
            builder.TenantId = TenantId;
            builder.UserName = builder.Email = UserName;
            builder.AddAllPermissions();
        });
        var label = new Label { Id = 1L, Title = "Updated Label", TenantId = TenantId };
        _mockTimeProvider.GetUtcNow().Returns(DateTime.UtcNow);


        // Act
        await _labelManager.UpdateLabelAsync(principal, label);

        // Assert
        await _fakeRepository.Received(1).UpdateLabelAsync(label);
        Assert.Equal(label.Modified, _mockTimeProvider.GetUtcNow());
    }

    /// <summary>
    /// Verifies that <see cref="LabelManager.SearchLabelsAsync"/> searches labels successfully.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task SearchLabelsAsync_WithValidText_ReturnsMatchingLabels()
    {
        // Arrange
        var principal = Impersonation.Impersonate(builder =>
        {
            builder.TenantId = TenantId;
            builder.UserName = builder.Email = UserName;
            builder.AddAllPermissions();
        });
        var projectId = 1L;
        var label1 = new Label { Title = "Test Label 1", TenantId = TenantId };
        var label2 = new Label { Title = "Another Label", TenantId = TenantId };
        await _fakeRepository.AddLabelAsync(label1);
        await _fakeRepository.AddLabelAsync(label2);

        // Act
        var result = await _labelManager.SearchLabelsAsync(principal, projectId, "Test", 0, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Test Label 1", result.First().Title);
    }
}
