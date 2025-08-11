using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Fields.Events;
using TestBucket.Domain.Fields.Models;
using TestBucket.Domain.States;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestCases;
using TestBucket.Domain.Testing.TestCases.Features.ChangeStateToCompletedWhenApproved;
using TestBucket.Traits.Core;
using Xunit;

namespace TestBucket.Domain.UnitTests.Testing.Features;

/// <summary>
/// Unit tests for <see cref="ChangeStateToCompletedWhenApprovedFieldChanges"/> feature.
/// Verifies that test case state changes to Completed when the Approved field changes to true, and does nothing otherwise.
/// </summary>
[UnitTest]
[FunctionalTest]
[Component("Testing")]
[Feature("Review")]
[EnrichedTest]
public class ChangeStateToCompletedWhenApprovedFieldChangesTests
{
    private static ClaimsPrincipal Principal => new ClaimsPrincipal();
    private static long TestCaseId => 123;
    private static long ProjectId => 456;

    private static TestCaseField CreateField(bool? value, TraitType traitType = TraitType.Approved)
    {
        return new TestCaseField
        {
            TestCaseId = TestCaseId,
            BooleanValue = value,
            FieldDefinition = new FieldDefinition { Id = 1, TraitType = traitType, Type = FieldType.Boolean, Name = "Approved" },
            FieldDefinitionId = 1
        };
    }

    private static IStateService CreateStateServiceWithCompletedState()
    {
        var stateService = Substitute.For<IStateService>();
        var completedState = new TestState { MappedState = MappedTestState.Completed, Name = "Completed" };
        stateService.GetTestCaseStatesAsync(Arg.Any<ClaimsPrincipal>(), ProjectId).Returns(new[] { completedState });
        return stateService;
    }
    private static TestCase CreateTestCase(long id, long? projectId, MappedTestState? mappedState)
    {
        return new TestCase { Id = id, TestProjectId = projectId, MappedState = mappedState, Name = "Test" };
    }

    /// <summary>
    /// Changes state to Completed when Approved field is set to true and test case is not already completed.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-003")]
    public async Task Handle_ChangesStateToCompleted_WhenApprovedTrueAndNotCompleted()
    {
        var testCase = CreateTestCase(TestCaseId, ProjectId, MappedTestState.Ongoing);
        var testCaseManager = Substitute.For<ITestCaseManager>();
        testCaseManager.GetTestCaseByIdAsync(Arg.Any<ClaimsPrincipal>(), TestCaseId).Returns(testCase);


        var stateService = CreateStateServiceWithCompletedState();
        var handler = new ChangeStateToCompletedWhenApprovedFieldChanges(testCaseManager, stateService);
        var notification = new TestCaseFieldChangedNotification(Principal, CreateField(true), null);

        await handler.Handle(notification, CancellationToken.None);

        Assert.Equal(MappedTestState.Completed, testCase.MappedState);
        Assert.Equal("Completed", testCase.State);
        await testCaseManager.Received(1).SaveTestCaseAsync(Arg.Any<ClaimsPrincipal>(), testCase);
    }

    /// <summary>
    /// Does nothing when the field definition is null.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-003")]
    public async Task Handle_DoesNothing_WhenFieldDefinitionIsNull()
    {
        var testCaseManager = Substitute.For<ITestCaseManager>();
        var stateService = CreateStateServiceWithCompletedState();

        var handler = new ChangeStateToCompletedWhenApprovedFieldChanges(testCaseManager, stateService);
        var field = new TestCaseField { TestCaseId = TestCaseId, BooleanValue = true, FieldDefinition = null, FieldDefinitionId = 1 };
        var notification = new TestCaseFieldChangedNotification(Principal, field, null);

        await handler.Handle(notification, CancellationToken.None);

        await testCaseManager.DidNotReceive().SaveTestCaseAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<TestCase>());
    }

    /// <summary>
    /// Does nothing when the trait type is not Approved.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-003")]
    public async Task Handle_DoesNothing_WhenTraitTypeIsNotApproved()
    {
        var testCaseManager = Substitute.For<ITestCaseManager>();
        var stateService = CreateStateServiceWithCompletedState();

        var handler = new ChangeStateToCompletedWhenApprovedFieldChanges(testCaseManager, stateService);
        var notification = new TestCaseFieldChangedNotification(Principal, CreateField(true, TraitType.Custom), null);

        await handler.Handle(notification, CancellationToken.None);

        await testCaseManager.DidNotReceive().SaveTestCaseAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<TestCase>());
    }

    /// <summary>
    /// Does nothing when the boolean value is not true.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-003")]
    public async Task Handle_DoesNothing_WhenBooleanValueIsNotTrue()
    {
        var testCaseManager = Substitute.For<ITestCaseManager>();
        IStateService stateService = CreateStateServiceWithCompletedState();
        var handler = new ChangeStateToCompletedWhenApprovedFieldChanges(testCaseManager, stateService);
        var notification = new TestCaseFieldChangedNotification(Principal, CreateField(false), null);
        await handler.Handle(notification, CancellationToken.None);
        await testCaseManager.DidNotReceive().SaveTestCaseAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<TestCase>());
    }

    /// <summary>
    /// Does nothing when the test case is already completed.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-003")]
    public async Task Handle_DoesNothing_WhenTestCaseAlreadyCompleted()
    {
        var testCase = CreateTestCase(TestCaseId, ProjectId, MappedTestState.Completed);
        var testCaseManager = Substitute.For<ITestCaseManager>();
        testCaseManager.GetTestCaseByIdAsync(Principal, TestCaseId).Returns(testCase);

        IStateService stateService = CreateStateServiceWithCompletedState();
        var handler = new ChangeStateToCompletedWhenApprovedFieldChanges(testCaseManager, stateService);
        var notification = new TestCaseFieldChangedNotification(Principal, CreateField(true), null);

        await handler.Handle(notification, CancellationToken.None);

        await testCaseManager.DidNotReceive().SaveTestCaseAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<TestCase>());
    }


    /// <summary>
    /// Does nothing when the completed state is not found.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-003")]
    public async Task Handle_DoesNothing_WhenCompletedStateNotFound()
    {
        var testCase = CreateTestCase(TestCaseId, ProjectId, MappedTestState.Ongoing);
        var testCaseManager = Substitute.For<ITestCaseManager>();
        testCaseManager.GetTestCaseByIdAsync(Principal, TestCaseId).Returns(testCase);

        var stateService = Substitute.For<IStateService>();
        stateService.GetTestCaseStatesAsync(Arg.Any<ClaimsPrincipal>(), ProjectId).Returns(new TestState[0]);
        var handler = new ChangeStateToCompletedWhenApprovedFieldChanges(testCaseManager, stateService);
        var notification = new TestCaseFieldChangedNotification(Principal, CreateField(true), null);

        await handler.Handle(notification, CancellationToken.None);

        await testCaseManager.DidNotReceive().SaveTestCaseAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<TestCase>());
    }
}
