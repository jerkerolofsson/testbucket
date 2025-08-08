using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using NSubstitute;

using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Features.Review;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Shared;
using TestBucket.Domain.Tenants.Models;
using TestBucket.Domain.Testing.Events;

using Xunit;

namespace TestBucket.Domain.UnitTests.Features.Review;

/// <summary>
/// Unit tests for the AssignDefaultReviewers class.
/// </summary>
public class AssignDefaultReviewersTests
{
    private const string TenantId = "tenant-1";

    /// <summary>
    /// Simulates impersonation for a specific tenant.
    /// </summary>
    /// <returns>A ClaimsPrincipal representing the impersonated user.</returns>
    private ClaimsPrincipal Impersonate() => Impersonation.Impersonate(TenantId);

    /// <summary>
    /// Tests that default reviewers are assigned when the state changes to Review.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-004")]
    public async Task Handle_ShouldAssignDefaultReviewers_WhenStateChangesToReview()
    {
        // Arrange
        var testCaseRepository = Substitute.For<ITestCaseRepository>();
        var assignDefaultReviewers = new AssignDefaultReviewers(testCaseRepository);
        var testCaseSavingEvent = new TestCaseSavingEvent(
            Impersonate(),
            new TestCase { MappedState = MappedTestState.Draft, Name = "test1", TestProjectId = 1 },
            new TestCase { MappedState = MappedTestState.Review, TenantId = TenantId, TestSuiteId = 1, Name = "test1", TestProjectId = 1 }
        );

        var testSuite = new TestSuite
        {
            Name = "suite1",
            Id = 1,
            DefaultReviewers = new List<AssignedReviewer>
            {
                new AssignedReviewer { UserName = "user1", Role = "Reviewer" },
                new AssignedReviewer { UserName = "user2", Role = "Reviewer" }
            }
        };

        testCaseRepository.GetTestSuiteByIdAsync(TenantId, 1).Returns(testSuite);

        // Act
        await assignDefaultReviewers.Handle(testCaseSavingEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(testCaseSavingEvent.New.ReviewAssignedTo);
        Assert.Equal(2, testCaseSavingEvent.New.ReviewAssignedTo.Count);
        Assert.Contains(testCaseSavingEvent.New.ReviewAssignedTo, r => r.UserName == "user1");
        Assert.Contains(testCaseSavingEvent.New.ReviewAssignedTo, r => r.UserName == "user2");
    }

    /// <summary>
    /// Tests that reviewers are not reassigned if they are already assigned.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-004")]
    public async Task Handle_ShouldNotAssignReviewers_WhenReviewersAlreadyAssigned()
    {
        // Arrange
        var testCaseRepository = Substitute.For<ITestCaseRepository>();
        var assignDefaultReviewers = new AssignDefaultReviewers(testCaseRepository);
        var testCaseSavingEvent = new TestCaseSavingEvent(
            Impersonate(),
            new TestCase { MappedState = MappedTestState.Draft, Name = "test1", TestProjectId = 1 },
            new TestCase
            {
                Name = "test1",
                TestProjectId = 1,
                MappedState = MappedTestState.Review,
                TenantId = TenantId,
                TestSuiteId = 1,
                ReviewAssignedTo = new List<AssignedReviewer>
                {
                    new AssignedReviewer { UserName = "existingUser", Role = "Reviewer" }
                }
            }
        );

        // Act
        await assignDefaultReviewers.Handle(testCaseSavingEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(testCaseSavingEvent.New.ReviewAssignedTo);
        Assert.Single(testCaseSavingEvent.New.ReviewAssignedTo);
        Assert.Contains(testCaseSavingEvent.New.ReviewAssignedTo, r => r.UserName == "existingUser");
    }

    /// <summary>
    /// Tests that no reviewers are assigned if the test suite has no default reviewers.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-004")]
    public async Task Handle_ShouldNotAssignReviewers_WhenTestSuiteHasNoDefaultReviewers()
    {
        // Arrange
        var testCaseRepository = Substitute.For<ITestCaseRepository>();
        var assignDefaultReviewers = new AssignDefaultReviewers(testCaseRepository);
        var testCaseSavingEvent = new TestCaseSavingEvent(
            Impersonate(),
            new TestCase { MappedState = MappedTestState.Draft, Name = "test1", TestProjectId = 1 },
            new TestCase { MappedState = MappedTestState.Review, TenantId = TenantId, TestSuiteId = 1, Name = "test1", TestProjectId = 1 }
        );

        var testSuite = new TestSuite { DefaultReviewers = new List<AssignedReviewer>(), Name = "suite1", Id = 1 };
        testCaseRepository.GetTestSuiteByIdAsync(TenantId, 1).Returns(testSuite);

        // Act
        await assignDefaultReviewers.Handle(testCaseSavingEvent, CancellationToken.None);

        // Assert
        Assert.Null(testCaseSavingEvent.New.ReviewAssignedTo);
    }

    /// <summary>
    /// Tests that no reviewers are assigned if the state does not change to Review.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-004")]
    public async Task Handle_ShouldNotAssignReviewers_WhenStateDoesNotChangeToReview()
    {
        // Arrange
        var testCaseRepository = Substitute.For<ITestCaseRepository>();
        var assignDefaultReviewers = new AssignDefaultReviewers(testCaseRepository);
        var testCaseSavingEvent = new TestCaseSavingEvent(
            Impersonate(),
            new TestCase { MappedState = MappedTestState.Draft, Name = "test1", TestProjectId = 1 },
            new TestCase { MappedState = MappedTestState.Draft, Name = "test1", TestProjectId = 1 }
        );

        // Act
        await assignDefaultReviewers.Handle(testCaseSavingEvent, CancellationToken.None);

        // Assert
        Assert.Null(testCaseSavingEvent.New.ReviewAssignedTo);
    }

    /// <summary>
    /// Tests that no reviewers are assigned if the TenantId or TestSuiteId is null.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-004")]
    public async Task Handle_ShouldNotAssignReviewers_WhenTenantIdOrTestSuiteIdIsNull()
    {
        // Arrange
        var testCaseRepository = Substitute.For<ITestCaseRepository>();
        var assignDefaultReviewers = new AssignDefaultReviewers(testCaseRepository);
        var testCaseSavingEvent = new TestCaseSavingEvent(
            Impersonate(),
            new TestCase { MappedState = MappedTestState.Draft, Name="test1", TestProjectId = 1 },
            new TestCase { MappedState = MappedTestState.Review, Name = "test1", TestProjectId = 1 }
        );

        // Act
        await assignDefaultReviewers.Handle(testCaseSavingEvent, CancellationToken.None);

        // Assert
        Assert.Null(testCaseSavingEvent.New.ReviewAssignedTo);
    }
}
