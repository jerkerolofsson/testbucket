using System.Security.Claims;

using NSubstitute;

using TestBucket.Domain.Editor.Models;
using TestBucket.Domain.Features.Review;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Shared;
using TestBucket.Domain.Testing.Events;
using TestBucket.Domain.Testing.TestCases;

namespace TestBucket.Domain.UnitTests.Features.Review;

/// <summary>
/// Approval of test under review
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Component("Testing")]
[Feature("Review")]
public class AutoApproveTestTests
{
    private const string TenantId = "tenant-1";

    /// <summary>
    /// Simulates impersonation for a specific tenant.
    /// </summary>
    /// <returns>A ClaimsPrincipal representing the impersonated user.</returns>
    private ClaimsPrincipal Impersonate() => Impersonation.Impersonate(TenantId);

    /// <summary>
    /// Tests that Handle approves the test when all votes are positive and the setting is enabled.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-005")]
    public async Task Handle_ApprovesTest_WhenAllVotesPositiveAndSettingEnabled()
    {
        // Arrange
        var settingsProvider = Substitute.For<ISettingsProvider>();
        var testCaseManager = Substitute.For<ITestCaseManager>();
        var principal = Impersonate();
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
        var evt = new TestCaseSavingEvent(principal, null, testCase);

        // Act
        await autoApprove.Handle(evt, CancellationToken.None);

        // Assert
        await testCaseManager.Received(1).ApproveTestAsync(principal, testCase);
    }

    /// <summary>
    /// Tests that Handle does not approve the test when the setting is disabled.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-005")]
    public async Task Handle_DoesNotApprove_WhenSettingDisabled()
    {
        var settingsProvider = Substitute.For<ISettingsProvider>();
        var testCaseManager = Substitute.For<ITestCaseManager>();
        var principal = Impersonate();
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
        var evt = new TestCaseSavingEvent(principal, null, testCase);

        await autoApprove.Handle(evt, CancellationToken.None);

        await testCaseManager.DidNotReceive().ApproveTestAsync(principal, testCase);
    }

    /// <summary>
    /// Tests that Handle does not approve the test when not all votes are positive.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-005")]
    public async Task Handle_DoesNotApprove_WhenNotAllVotesPositive()
    {
        var settingsProvider = Substitute.For<ISettingsProvider>();
        var testCaseManager = Substitute.For<ITestCaseManager>();
        var principal = Impersonate();
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
        var evt = new TestCaseSavingEvent(principal, null, testCase);

        await autoApprove.Handle(evt, CancellationToken.None);

        await testCaseManager.DidNotReceive().ApproveTestAsync(principal, testCase);
    }

    /// <summary>
    /// Tests that Handle does not approve the test when tenant or project ID is missing.
    /// </summary>
    /// <param name="tenantId">The tenant ID to test.</param>
    /// <param name="projectId">The project ID to test.</param>
    [Theory]
    [InlineData("tenant-1", null)]
    [CoveredRequirement("TB-REVIEW-005")]
    public async Task Handle_DoesNotApprove_WhenTenantOrProjectIdMissing(string tenantId, long? projectId)
    {
        var settingsProvider = Substitute.For<ISettingsProvider>();
        var testCaseManager = Substitute.For<ITestCaseManager>();
        var principal = Impersonate();
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
        settingsProvider.GetDomainSettingsAsync<ReviewSettings>(tenantId, projectId).Returns(reviewSettings);
        var autoApprove = new AutoApproveTest(settingsProvider, testCaseManager);
        var evt = new TestCaseSavingEvent(principal, null, testCase);

        await autoApprove.Handle(evt, CancellationToken.None);

        await testCaseManager.DidNotReceive().ApproveTestAsync(principal, testCase);
    }


    /// <summary>
    /// Tests that Handle does not approve the test when ReviewAssignedTo is null or empty.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-005")]
    public async Task Handle_DoesNotApprove_WhenReviewAssignedToIsNullOrEmpty()
    {
        var settingsProvider = Substitute.For<ISettingsProvider>();
        var testCaseManager = Substitute.For<ITestCaseManager>();
        var principal = Impersonate();
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
        var evt = new TestCaseSavingEvent(principal, null, testCase);

        await autoApprove.Handle(evt, CancellationToken.None);

        await testCaseManager.DidNotReceive().ApproveTestAsync(principal, testCase);

        // Also test empty list
        testCase.ReviewAssignedTo = [];
        await autoApprove.Handle(evt, CancellationToken.None);

        await testCaseManager.DidNotReceive().ApproveTestAsync(principal, testCase);
    }
}
