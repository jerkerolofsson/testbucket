using System.Security.Claims;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;

using TestBucket.CodeCoverage;
using TestBucket.Domain.Code.CodeCoverage;
using TestBucket.Domain.Code.CodeCoverage.Import;
using TestBucket.Domain.Code.CodeCoverage.Models;
using TestBucket.Domain.Files;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Settings.Fakes;
using TestBucket.Domain.Testing.TestRuns;

namespace TestBucket.Domain.UnitTests.Code.CodeCoverage;

/// <summary>
/// Contains unit tests for <see cref="CodeCoverageImporter"/>, verifying code coverage import functionality.
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Feature("Code Coverage")]
[Component("Code")]
public class CodeCoverageImporterTests
{
    /// <summary>
    /// Creates a new instance of <see cref="CodeCoverageManager"/> with fake dependencies for testing.
    /// </summary>
    /// <returns>
    /// A configured <see cref="CodeCoverageManager"/> instance.
    /// </returns>
    private async Task<CodeCoverageManager> CreateManagerAsync()
    {
        var settingsProvider = new FakeSettingsProvider();
        await settingsProvider.SaveDomainSettingsAsync("tenant-1", 1, new CodeCoverageSettings());

        var repository = new Fakes.FakeCodeCoverageRepository();
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 12, 0, 1, 2, TimeSpan.Zero));
        return new CodeCoverageManager(settingsProvider, repository, timeProvider);
    }

    /// <summary>
    /// Gets a logger instance for <see cref="CodeCoverageImporter"/>.
    /// </summary>
    private ILogger<CodeCoverageImporter> Logger => NullLogger<CodeCoverageImporter>.Instance;

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> for the specified tenant.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <returns>
    /// A <see cref="ClaimsPrincipal"/> representing the user.
    /// </returns>
    private ClaimsPrincipal CreateUser(string tenantId)
    {
        return Impersonation.Impersonate(tenantId);
    }

    /// <summary>
    /// Returns a valid Cobertura XML file as a byte array for testing.
    /// </summary>
    /// <returns>
    /// A byte array containing Cobertura XML data.
    /// </returns>
    private byte[] GetValidCobertura()
    {
        string xml = """
            <?xml version="1.0" encoding="utf-8" standalone="yes"?>
            <coverage line-rate="0.5" branch-rate="0.5" complexity="10" version="1.9" timestamp="1747013452" lines-covered="1" lines-valid="2">
              <packages>
                <package line-rate="0.21761151590545583" branch-rate="0.16276595744680852" complexity="4971" name="FooBar">
                  <classes>
                    <class line-rate="1" branch-rate="1" complexity="2" name="Bar" filename="test.c">
                      <methods>
                        <method line-rate="1" branch-rate="1" complexity="2" name="foo_bar" signature="(int)">
                          <lines>
                            <line number="1" hits="1" branch="False" />
                            <line number="2" hits="0" branch="False" />
                          </lines>
                        </method>
                      </methods>
                      <lines>
                            <line number="1" hits="1" branch="False" />
                            <line number="2" hits="0" branch="False" />
                      </lines>
                    </class>
                  </classes>
                </package>
              </packages>
            </coverage>
            """;
        return Encoding.UTF8.GetBytes(xml);
    }

    /// <summary>
    /// Verifies that <see cref="CodeCoverageImporter.ImportAsync"/> calls <c>GetResourceByIdAsync</c> and <c>GetTestRunByIdAsync</c>.
    /// </summary>
    [Fact]
    public async Task ImportAsync_Should_Call_GetResourceByIdAsync_And_GetTestRunByIdAsync()
    {
        // Arrange
        var fileResourceManager = Substitute.For<IFileResourceManager>();
        var testRunManager = Substitute.For<ITestRunManager>();
        var codeCoverageManager = await CreateManagerAsync();
        var logger = Logger;

        var importer = new CodeCoverageImporter(fileResourceManager, logger, codeCoverageManager, testRunManager);

        var userName = "user1";
        var tenantId = "tenant-1";
        var resourceId = 123L;
        var cancellationToken = CancellationToken.None;

        var principal = CreateUser(tenantId);
        var fileResource = new FileResource
        {
            TenantId = tenantId,
            ContentType = CodeCoverageMediaTypes.Cobertura,
            Id = resourceId,
            Name = "coverage.json",
            Data = GetValidCobertura(),
            Length = GetValidCobertura().Length,
            TestRunId = 4
        };
        var testRun = new TestRun
        {
            Name = "Test run 1",
            Id = 4,
            TenantId = tenantId,
            TestProjectId = 1,
            TestRunFields = new List<TestRunField>()
        };
        fileResourceManager.GetResourceByIdAsync(Arg.Any<ClaimsPrincipal>(), resourceId).Returns(Task.FromResult<FileResource?>(fileResource));
        testRunManager.GetTestRunByIdAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<long>()).Returns(Task.FromResult<TestRun?>(testRun));

        // Act
        await importer.ImportAsync(userName, tenantId, resourceId, cancellationToken);

        // Assert
        await fileResourceManager.Received(1).GetResourceByIdAsync(Arg.Any<ClaimsPrincipal>(), resourceId);
        await testRunManager.Received().GetTestRunByIdAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<long>());
    }

    /// <summary>
    /// Verifies that <see cref="CodeCoverageImporter.ImportAsync"/> creates a <see cref="CodeCoverageGroup"/> for a test run.
    /// </summary>
    [Fact]
    public async Task ImportAsync_CreatesTestRunCodeCoverageGroup()
    {
        // Arrange
        var fileResourceManager = Substitute.For<IFileResourceManager>();
        var testRunManager = Substitute.For<ITestRunManager>();
        var codeCoverageManager = await CreateManagerAsync();
        var logger = Logger;

        var importer = new CodeCoverageImporter(fileResourceManager, logger, codeCoverageManager, testRunManager);

        var userName = "user1";
        var tenantId = "tenant-1";
        var resourceId = 123L;
        var cancellationToken = CancellationToken.None;

        var principal = CreateUser(tenantId);
        var fileResource = new FileResource
        {
            TenantId = tenantId,
            ContentType = CodeCoverageMediaTypes.Cobertura,
            Id = resourceId,
            Name = "coverage.json",
            Data = GetValidCobertura(),
            Length = GetValidCobertura().Length,
            TestRunId = 4
        };
        var testRun = new TestRun
        {
            Name = "Test run 1",
            Id = 4,
            TenantId = tenantId,
            TestProjectId = 1,
            TestRunFields = new List<TestRunField>()
        };
        fileResourceManager.GetResourceByIdAsync(Arg.Any<ClaimsPrincipal>(), resourceId).Returns(Task.FromResult<FileResource?>(fileResource));
        testRunManager.GetTestRunByIdAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<long>()).Returns(Task.FromResult<TestRun?>(testRun));

        // Act
        await importer.ImportAsync(userName, tenantId, resourceId, cancellationToken);

        // Assert
        var group = await codeCoverageManager.GetCodeCoverageGroupAsync(principal, 1, CodeCoverageGroupType.TestRun, "4");
        Assert.NotNull(group);
        Assert.Equal("4", group.Name);
        Assert.Equal(2, group.LineCount);
        Assert.Equal(1, group.CoveredLineCount);
    }
}