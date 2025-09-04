using System.Security.Claims;

using NSubstitute;

using TestBucket.Domain.AI.Runner;
using TestBucket.Domain.Files;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Shared;

namespace TestBucket.Domain.UnitTests.AI.Runner;

/// <summary>
/// Tests related to uploading attachments to tests executed by the AI runner
/// </summary>
[Feature("AI Runner")]
[Component("AI")]
[UnitTest]
[FunctionalTest]
[EnrichedTest]
public class IFileResourceManagerArtifactExtensionsTests
{
    private static ClaimsPrincipal GetTestUser(string tenantId) => Impersonation.Impersonate(tenantId);

    /// <summary>
    /// Verifies that magic detection of PNG files are accurately detected when the file extension cannot be resolved to a mime type
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SaveArtifactsAsync_WithPngWithMagicMatchButUnknownExtensions_WithCorrectProperties()
    {
        // Arrange
        var fileManager = Substitute.For<IFileResourceManager>();
        var principal = GetTestUser("tenant1");
        var run = new TestCaseRun { Id = 123, Name = "tc" };

        var pngBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47 }; // PNG signature
        var artifacts = new Dictionary<string, byte[]>
        {
            ["image.unknown"] = pngBytes
        };

        // Act
        await fileManager.SaveArtifactsAsync(principal, run, artifacts);

        // Assert - each artifact should be added with correct properties
        foreach (var kv in artifacts)
        {
            var expectedContentType = kv.Key switch
            {
                "image.unknown" => "image/png",
                _ => "error"
            };
            await fileManager.Received(1).AddResourceAsync(principal, Arg.Is<FileResource>(r =>
                r.TestCaseRunId == run.Id &&
                r.TenantId == principal.GetTenantIdOrThrow() &&
                r.Data != null &&
                r.Data.SequenceEqual(kv.Value) &&
                r.ContentType == expectedContentType
            ));
        }

        // Ensure exactly two calls were made
        await fileManager.Received(1).AddResourceAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<FileResource>());
    }

    /// <summary>
    /// Verifies that saving multiple artifacts with known file extensions works
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SaveArtifactsAsync_SavesAllArtifacts_WithCorrectProperties()
    {
        // Arrange
        var fileManager = Substitute.For<IFileResourceManager>();
        var principal = GetTestUser("tenant1");
        var run = new TestCaseRun { Id = 123, Name = "tc" };

        var textBytes = Encoding.UTF8.GetBytes("hello world");
        var pngBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47 }; // PNG signature
        var artifacts = new Dictionary<string, byte[]>
        {
            ["file.txt"] = textBytes,
            ["image.png"] = pngBytes
        };

        // Act
        await fileManager.SaveArtifactsAsync(principal, run, artifacts);

        // Assert - each artifact should be added with correct properties
        foreach (var kv in artifacts)
        {
            var expectedContentType = kv.Key switch
            {
                "file.txt" => "text/plain",
                "image.png" => "image/png",
                _ => "error"
            };
            await fileManager.Received(1).AddResourceAsync(principal, Arg.Is<FileResource>(r =>
                r.TestCaseRunId == run.Id &&
                r.TenantId == principal.GetTenantIdOrThrow() &&
                r.Data != null &&
                r.Data.SequenceEqual(kv.Value) &&
                r.ContentType == expectedContentType
            ));
        }

        // Ensure exactly two calls were made
        await fileManager.Received(2).AddResourceAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<FileResource>());
    }

    /// <summary>
    /// Verifies that providing an empty list of artifacts doesn't save any artifacts
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SaveArtifactsAsync_WithNoArtifacts_DoesNotCallAddResourceAsync()
    {
        // Arrange
        var fileManager = Substitute.For<IFileResourceManager>();
        var principal = GetTestUser("tenant1");
        var run = new TestCaseRun { Id = 1, Name = "tc" };
        var artifacts = new Dictionary<string, byte[]>();

        // Act
        await fileManager.SaveArtifactsAsync(principal, run, artifacts);

        // Assert
        await fileManager.DidNotReceive().AddResourceAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<FileResource>());
    }
}