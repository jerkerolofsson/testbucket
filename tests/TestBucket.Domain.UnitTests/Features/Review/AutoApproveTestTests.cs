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
/// Tests that verify that auto approval of tests works
/// </summary>
[UnitTest]
[FunctionalTest]
[Component("Testing")]
[Feature("Review")]
[EnrichedTest]
public class AutoApproveTestTests
{
    private const string TenantId = "tenant1";

    /// <summary>
    /// Tests that Handle approves the test when all votes are positive and the setting is enabled.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-005")]
    public async Task Handle_ApprovesTest_WhenAllVotesPositiveAndSettingEnabled()
    {
        // Arrange
        var testCaseManager = Substitute.For<ITestCaseManager>();
        var principal = new ClaimsPrincipal();
        var testCase = new TestCase
        {
            TenantId = TenantId,
            TestProjectId = 1,
            Name = "TestName",
            MappedState = TestBucket.Contracts.Testing.States.MappedTestState.Review,
            ReviewAssignedTo = new List<AssignedReviewer>
            {
                new AssignedReviewer { UserName = "user1", Role = "reviewer", Vote = 1 },
                new AssignedReviewer { UserName = "user2", Role = "reviewer", Vote = 1 }
            }
        };
        ISettingsProvider settingsProvider = CreateSettingsProvider(TenantId, 1, true);

        var autoApprove = new AutoApproveTest(settingsProvider, testCaseManager);
        var evt = new TestCaseSavingEvent(GetPrincipal(TenantId), null, testCase);

        // Act
        await autoApprove.Handle(evt, CancellationToken.None);

        // Assert
        await testCaseManager.Received(1).ApproveTestAsync(Arg.Any<ClaimsPrincipal>(), testCase);
    }

    /// <summary>
    /// Tests that Handle does not approve the test when the setting is disabled.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-005")]
    public async Task Handle_DoesNotApprove_WhenSettingDisabled()
    {
        var testCaseManager = Substitute.For<ITestCaseManager>();
        var principal = new ClaimsPrincipal();
        var testCase = new TestCase
        {
            TenantId = TenantId,
            TestProjectId = 1,
            Name = "TestName",
            MappedState = TestBucket.Contracts.Testing.States.MappedTestState.Review,
            ReviewAssignedTo = new List<AssignedReviewer>
            {
                new AssignedReviewer { UserName = "user1", Role = "reviewer", Vote = 1 },
                new AssignedReviewer { UserName = "user2", Role = "reviewer", Vote = 1 }
            }
        };
        ISettingsProvider settingsProvider = CreateSettingsProvider(TenantId, 1, false);

        var autoApprove = new AutoApproveTest(settingsProvider, testCaseManager);
        var evt = new TestCaseSavingEvent(GetPrincipal(TenantId), null, testCase);

        await autoApprove.Handle(evt, CancellationToken.None);

        await testCaseManager.DidNotReceive().ApproveTestAsync(Arg.Any<ClaimsPrincipal>(), testCase);
    }

    /// <summary>
    /// Tests that Handle does not approve the test when not all votes are positive.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-005")]
    public async Task Handle_DoesNotApprove_WhenNotAllVotesPositive()
    {
        var testCaseManager = Substitute.For<ITestCaseManager>();
        var principal = new ClaimsPrincipal();
        var testCase = new TestCase
        {
            TenantId = TenantId,
            TestProjectId = 1,
            Name = "TestName",
            MappedState = TestBucket.Contracts.Testing.States.MappedTestState.Review,
            ReviewAssignedTo = new List<AssignedReviewer>
            {
                new AssignedReviewer { UserName = "user1", Role = "reviewer", Vote = 1 },
                new AssignedReviewer { UserName = "user2", Role = "reviewer", Vote = 0 }
            }
        };
        ISettingsProvider settingsProvider = CreateSettingsProvider(TenantId, 1, true);
        var autoApprove = new AutoApproveTest(settingsProvider, testCaseManager);
        var evt = new TestCaseSavingEvent(GetPrincipal(TenantId), null, testCase);

        await autoApprove.Handle(evt, CancellationToken.None);

        await testCaseManager.DidNotReceive().ApproveTestAsync(Arg.Any<ClaimsPrincipal>(), testCase);
    }

    /// <summary>
    /// Tests that Handle does not approve the test when tenant or project ID is missing.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-005")]
    public async Task Handle_DoesNotApprove_WhenProjectIdMissing()
    {
        var testCaseManager = Substitute.For<ITestCaseManager>();
        var principal = new ClaimsPrincipal();
        var testCase = new TestCase
        {
            TenantId = TenantId,
            TestProjectId = null,
            Name = "TestName",
            MappedState = TestBucket.Contracts.Testing.States.MappedTestState.Review,
            ReviewAssignedTo = new List<AssignedReviewer>
            {
                new AssignedReviewer { UserName = "user1", Role = "reviewer", Vote = 1 }
            }
        };
        ISettingsProvider settingsProvider = CreateSettingsProvider(TenantId, 1, true);
        var autoApprove = new AutoApproveTest(settingsProvider, testCaseManager);
        var evt = new TestCaseSavingEvent(GetPrincipal(TenantId), null, testCase);

        await autoApprove.Handle(evt, CancellationToken.None);

        await testCaseManager.DidNotReceive().ApproveTestAsync(Arg.Any<ClaimsPrincipal>(), testCase);
    }


    /// <summary>
    /// Tests that Handle does not approve the test when ReviewAssignedTo is null or empty.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-005")]
    public async Task Handle_DoesNotApprove_WhenReviewAssignedToIsNullOrEmpty()
    {
        var testCaseManager = Substitute.For<ITestCaseManager>();
        var principal = new ClaimsPrincipal();
        var testCase = new TestCase
        {
            TenantId = TenantId,
            TestProjectId = 1,
            Name = "TestName",
            MappedState = TestBucket.Contracts.Testing.States.MappedTestState.Review,
            ReviewAssignedTo = null
        };
        ISettingsProvider settingsProvider = CreateSettingsProvider(TenantId, 1, true);

        var autoApprove = new AutoApproveTest(settingsProvider, testCaseManager);
        var evt = new TestCaseSavingEvent(GetPrincipal(TenantId), null, testCase);

        await autoApprove.Handle(evt, CancellationToken.None);

        await testCaseManager.DidNotReceive().ApproveTestAsync(Arg.Any<ClaimsPrincipal>(), testCase);

        // Also test empty list
        testCase.ReviewAssignedTo = new List<AssignedReviewer>();
        await autoApprove.Handle(evt, CancellationToken.None);
        await testCaseManager.DidNotReceive().ApproveTestAsync(Arg.Any<ClaimsPrincipal>(), testCase);
    }


    private static ISettingsProvider CreateSettingsProvider(string? tenantId, long? projectId, bool enabled)
    {
        var settingsProvider = Substitute.For<ISettingsProvider>();
        var reviewSettings = new ReviewSettings { AutomaticallyApproveTestIfReviewFinishedWithOnlyPositiveVotes = enabled };
        settingsProvider.GetDomainSettingsAsync<ReviewSettings>(tenantId!, projectId).Returns(reviewSettings);
        return settingsProvider;
    }

    private ClaimsPrincipal GetPrincipal(string? tenantId)
    {
        if (tenantId is null)
        {
            return Impersonation.Impersonate(x =>
            {
                x.UserName = "user1";
            });
        }
        return Impersonation.Impersonate(tenantId);
    }
}
