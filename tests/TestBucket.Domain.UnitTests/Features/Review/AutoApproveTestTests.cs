using NSubstitute;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using TestBucket.Domain.Features.Review;
using TestBucket.Domain.Editor.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestCases;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Shared;
using Xunit;

namespace TestBucket.Domain.UnitTests.Features.Review;
public class AutoApproveTestTests
{
    /// <summary>
    /// Tests that Handle approves the test when all votes are positive and the setting is enabled.
    /// </summary>
    [Fact]
    public async Task Handle_ApprovesTest_WhenAllVotesPositiveAndSettingEnabled()
    {
        // Arrange
        var settingsProvider = Substitute.For<ISettingsProvider>();
        var testCaseManager = Substitute.For<ITestCaseManager>();
        var principal = new ClaimsPrincipal();
        var testCase = new TestCase
        {
            TenantId = "tenant1",
            TestProjectId = 1,
            Name = "TestName",
            MappedState = TestBucket.Contracts.Testing.States.MappedTestState.Review,
            ReviewAssignedTo = new List<AssignedReviewer>
            {
                new AssignedReviewer { UserName = "user1", Role = "reviewer", Vote = 1 },
                new AssignedReviewer { UserName = "user2", Role = "reviewer", Vote = 1 }
            }
        };
        var reviewSettings = new ReviewSettings { AutomaticallyApproveTestIfReviewFinishedWithOnlyPositiveVotes = true };
        settingsProvider.GetDomainSettingsAsync<ReviewSettings>("tenant1", 1).Returns(reviewSettings);
        var autoApprove = new AutoApproveTest(settingsProvider, testCaseManager);
        var evt = Substitute.For<TestBucket.Domain.Testing.Events.TestCaseSavingEvent>();
        evt.New.Returns(testCase);
        evt.Principal.Returns(principal);

        // Act
        await autoApprove.Handle(evt, CancellationToken.None);

        // Assert
        await testCaseManager.Received(1).ApproveTestAsync(principal, testCase);
    }

    /// <summary>
    /// Tests that Handle does not approve the test when the setting is disabled.
    /// </summary>
    [Fact]
    public async Task Handle_DoesNotApprove_WhenSettingDisabled()
    {
        var settingsProvider = Substitute.For<ISettingsProvider>();
        var testCaseManager = Substitute.For<ITestCaseManager>();
        var principal = new ClaimsPrincipal();
        var testCase = new TestCase
        {
            TenantId = "tenant1",
            TestProjectId = 1,
            Name = "TestName",
            MappedState = TestBucket.Contracts.Testing.States.MappedTestState.Review,
            ReviewAssignedTo = new List<AssignedReviewer>
            {
                new AssignedReviewer { UserName = "user1", Role = "reviewer", Vote = 1 },
                new AssignedReviewer { UserName = "user2", Role = "reviewer", Vote = 1 }
            }
        };
        var reviewSettings = new ReviewSettings { AutomaticallyApproveTestIfReviewFinishedWithOnlyPositiveVotes = false };
        settingsProvider.GetDomainSettingsAsync<ReviewSettings>("tenant1", 1).Returns(reviewSettings);
        var autoApprove = new AutoApproveTest(settingsProvider, testCaseManager);
        var evt = Substitute.For<TestBucket.Domain.Testing.Events.TestCaseSavingEvent>();
        evt.New.Returns(testCase);
        evt.Principal.Returns(principal);

        await autoApprove.Handle(evt, CancellationToken.None);

        await testCaseManager.DidNotReceive().ApproveTestAsync(principal, testCase);
    }

    /// <summary>
    /// Tests that Handle does not approve the test when not all votes are positive.
    /// </summary>
    [Fact]
    public async Task Handle_DoesNotApprove_WhenNotAllVotesPositive()
    {
        var settingsProvider = Substitute.For<ISettingsProvider>();
        var testCaseManager = Substitute.For<ITestCaseManager>();
        var principal = new ClaimsPrincipal();
        var testCase = new TestCase
        {
            TenantId = "tenant1",
            TestProjectId = 1,
            Name = "TestName",
            MappedState = TestBucket.Contracts.Testing.States.MappedTestState.Review,
            ReviewAssignedTo = new List<AssignedReviewer>
            {
                new AssignedReviewer { UserName = "user1", Role = "reviewer", Vote = 1 },
                new AssignedReviewer { UserName = "user2", Role = "reviewer", Vote = 0 }
            }
        };
        var reviewSettings = new ReviewSettings { AutomaticallyApproveTestIfReviewFinishedWithOnlyPositiveVotes = true };
        settingsProvider.GetDomainSettingsAsync<ReviewSettings>("tenant1", 1).Returns(reviewSettings);
        var autoApprove = new AutoApproveTest(settingsProvider, testCaseManager);
        var evt = Substitute.For<TestBucket.Domain.Testing.Events.TestCaseSavingEvent>();
        evt.New.Returns(testCase);
        evt.Principal.Returns(principal);

        await autoApprove.Handle(evt, CancellationToken.None);

        await testCaseManager.DidNotReceive().ApproveTestAsync(principal, testCase);
    }

    /// <summary>
    /// Tests that Handle does not approve the test when tenant or project ID is missing.
    /// </summary>
    /// <param name="tenantId">The tenant ID to test.</param>
    /// <param name="projectId">The project ID to test.</param>
    [Theory]
    [InlineData(null, 1)]
    [InlineData("tenant1", null)]
    public async Task Handle_DoesNotApprove_WhenTenantOrProjectIdMissing(string? tenantId, long? projectId)
    {
        var settingsProvider = Substitute.For<ISettingsProvider>();
        var testCaseManager = Substitute.For<ITestCaseManager>();
        var principal = new ClaimsPrincipal();
        var testCase = new TestCase
        {
            TenantId = tenantId,
            TestProjectId = projectId,
            Name = "TestName",
            MappedState = TestBucket.Contracts.Testing.States.MappedTestState.Review,
            ReviewAssignedTo = new List<AssignedReviewer>
            {
                new AssignedReviewer { UserName = "user1", Role = "reviewer", Vote = 1 }
            }
        };
        var reviewSettings = new ReviewSettings { AutomaticallyApproveTestIfReviewFinishedWithOnlyPositiveVotes = true };
        settingsProvider.GetDomainSettingsAsync<ReviewSettings>(tenantId!, projectId).Returns(reviewSettings);
        var autoApprove = new AutoApproveTest(settingsProvider, testCaseManager);
        var evt = Substitute.For<TestBucket.Domain.Testing.Events.TestCaseSavingEvent>();
        evt.New.Returns(testCase);
        evt.Principal.Returns(principal);

        await autoApprove.Handle(evt, CancellationToken.None);

        await testCaseManager.DidNotReceive().ApproveTestAsync(principal, testCase);
    }

    /// <summary>
    /// Tests that Handle does not approve the test when ReviewAssignedTo is null or empty.
    /// </summary>
    [Fact]
    public async Task Handle_DoesNotApprove_WhenReviewAssignedToIsNullOrEmpty()
    {
        var settingsProvider = Substitute.For<ISettingsProvider>();
        var testCaseManager = Substitute.For<ITestCaseManager>();
        var principal = new ClaimsPrincipal();
        var testCase = new TestCase
        {
            TenantId = "tenant1",
            TestProjectId = 1,
            Name = "TestName",
            MappedState = TestBucket.Contracts.Testing.States.MappedTestState.Review,
            ReviewAssignedTo = null
        };
        var reviewSettings = new ReviewSettings { AutomaticallyApproveTestIfReviewFinishedWithOnlyPositiveVotes = true };
        settingsProvider.GetDomainSettingsAsync<ReviewSettings>("tenant1", 1).Returns(reviewSettings);
        var autoApprove = new AutoApproveTest(settingsProvider, testCaseManager);
        var evt = Substitute.For<TestBucket.Domain.Testing.Events.TestCaseSavingEvent>();
        evt.New.Returns(testCase);
        evt.Principal.Returns(principal);

        await autoApprove.Handle(evt, CancellationToken.None);

        await testCaseManager.DidNotReceive().ApproveTestAsync(principal, testCase);

        // Also test empty list
        testCase.ReviewAssignedTo = new List<AssignedReviewer>();
        await autoApprove.Handle(evt, CancellationToken.None);
        await testCaseManager.DidNotReceive().ApproveTestAsync(principal, testCase);
    }
}
