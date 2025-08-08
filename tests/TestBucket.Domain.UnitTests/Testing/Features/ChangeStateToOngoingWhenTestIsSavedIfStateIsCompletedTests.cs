using System.Security.Claims;

using NSubstitute;

using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Editor.Models;
using TestBucket.Domain.Settings;
using TestBucket.Domain.States;
using TestBucket.Domain.Testing.Events;
using TestBucket.Domain.Testing.TestCases.Features.ChangeStateToOngoingWhenEditingTests;

namespace TestBucket.Domain.UnitTests.Testing.Features;

/// <summary>
/// Unit tests for the ChangeStateToOngoingWhenTestIsSavedIfStateIsCompleted feature.
/// Verifies that the state changes to Ongoing when editing a completed test case under specific conditions.
/// </summary>
[EnrichedTest]
[UnitTest]
[FunctionalTest]
[Component("Testing")]
[Feature("Review")]
public class ChangeStateToOngoingWhenTestIsSavedIfStateIsCompletedTests
{
    /// <summary>
    /// Verifies IsApplicableRequest returns false when the new test case's TestProjectId is null.
    /// </summary>
    [CoveredRequirement("TB-EDITOR-001")]
    [Fact]
    public void IsApplicableRequest_ReturnsFalse_WhenTestProjectIdIsNull()
    {
        var oldTestCase = new TestCase { Description = "desc", TenantId = "tenant", TestProjectId = 1, Name = "Test", MappedState = MappedTestState.Completed };
        var newTestCase = new TestCase { Description = "desc2", TenantId = "tenant", TestProjectId = null, Name = "Test", MappedState = MappedTestState.Completed };
        var evt = new TestCaseSavingEvent(new ClaimsPrincipal(), oldTestCase, newTestCase);
        Assert.False(ChangeStateToOngoingWhenTestIsSavedIfStateIsCompleted.IsApplicableRequest(evt));
    }

    /// <summary>
    /// Verifies IsApplicableRequest returns false when the new test case's TenantId is null.
    /// </summary>
    [CoveredRequirement("TB-EDITOR-001")]
    [Fact]
    public void IsApplicableRequest_ReturnsFalse_WhenTenantIdIsNull()
    {
        var oldTestCase = new TestCase { Description = "desc", TenantId = "tenant", TestProjectId = 1, Name = "Test", MappedState = MappedTestState.Completed };
        var newTestCase = new TestCase { Description = "desc2", TenantId = null, TestProjectId = 1, Name = "Test", MappedState = MappedTestState.Completed };
        var evt = new TestCaseSavingEvent(new ClaimsPrincipal(), oldTestCase, newTestCase);
        Assert.False(ChangeStateToOngoingWhenTestIsSavedIfStateIsCompleted.IsApplicableRequest(evt));
    }

    /// <summary>
    /// Verifies IsApplicableRequest returns false when the old test case is null.
    /// </summary>
    [CoveredRequirement("TB-EDITOR-001")]
    [Fact]
    public void IsApplicableRequest_ReturnsFalse_WhenOldIsNull()
    {
        var newTestCase = new TestCase { Description = "desc2", TenantId = "tenant", TestProjectId = 1, Name = "Test", MappedState = MappedTestState.Completed };
        var evt = new TestCaseSavingEvent(new ClaimsPrincipal(), null, newTestCase);
        Assert.False(ChangeStateToOngoingWhenTestIsSavedIfStateIsCompleted.IsApplicableRequest(evt));
    }

    /// <summary>
    /// Verifies IsApplicableRequest returns false when the description is unchanged.
    /// </summary>
    [CoveredRequirement("TB-EDITOR-001")]
    [Fact]
    public void IsApplicableRequest_ReturnsFalse_WhenDescriptionUnchanged()
    {
        var oldTestCase = new TestCase { Description = "desc", TenantId = "tenant", TestProjectId = 1, Name = "Test", MappedState = MappedTestState.Completed };
        var newTestCase = new TestCase { Description = "desc", TenantId = "tenant", TestProjectId = 1, Name = "Test", MappedState = MappedTestState.Completed };
        var evt = new TestCaseSavingEvent(new ClaimsPrincipal(), oldTestCase, newTestCase);
        Assert.False(ChangeStateToOngoingWhenTestIsSavedIfStateIsCompleted.IsApplicableRequest(evt));
    }

    /// <summary>
    /// Verifies IsApplicableRequest returns true when the description is changed and required fields are present.
    /// </summary>
    [CoveredRequirement("TB-EDITOR-001")]
    [Fact]
    public void IsApplicableRequest_ReturnsTrue_WhenDescriptionChanged_AndRequiredFieldsPresent()
    {
        var oldTestCase = new TestCase { Description = "desc", TenantId = "tenant", TestProjectId = 1, Name = "Test", MappedState = MappedTestState.Completed };
        var newTestCase = new TestCase { Description = "desc2", TenantId = "tenant", TestProjectId = 1, Name = "Test", MappedState = MappedTestState.Completed };
        var evt = new TestCaseSavingEvent(new ClaimsPrincipal(), oldTestCase, newTestCase);
        Assert.True(ChangeStateToOngoingWhenTestIsSavedIfStateIsCompleted.IsApplicableRequest(evt));
    }

    /// <summary>
    /// Verifies Handle does nothing when EditorSettings is null.
    /// </summary>
    [CoveredRequirement("TB-EDITOR-001")]
    [Fact]
    public async Task Handle_DoesNothing_WhenEditorSettingsIsNull()
    {
        var stateService = Substitute.For<IStateService>();
        var settingsProvider = Substitute.For<ISettingsProvider>();
        settingsProvider.GetDomainSettingsAsync<EditorSettings>(Arg.Any<string>(), Arg.Any<long>()).Returns(Task.FromResult<EditorSettings?>(null));

        var handler = new ChangeStateToOngoingWhenTestIsSavedIfStateIsCompleted(stateService, settingsProvider);
        var newTestCase = new TestCase { Name = "Test", MappedState = MappedTestState.Completed };
        var notification = new TestCaseSavingEvent(new ClaimsPrincipal(), new TestCase { Name = "Test" }, newTestCase);

        await handler.Handle(notification, CancellationToken.None);

        Assert.Equal(MappedTestState.Completed, newTestCase.MappedState);
    }

    /// <summary>
    /// Verifies Handle does nothing when ChangeStateToOngoingWhenEditingTests is false.
    /// </summary>
    [CoveredRequirement("TB-EDITOR-001")]
    [Fact]
    public async Task Handle_DoesNothing_WhenChangeStateToOngoingWhenEditingTestsIsFalse()
    {
        IStateService stateService = CreateStateServiceWithBothStates();
        ISettingsProvider settingsProvider = CreateSettingProvider(false);

        var handler = new ChangeStateToOngoingWhenTestIsSavedIfStateIsCompleted(stateService, settingsProvider);
        var newTestCase = new TestCase { Name = "Test", MappedState = MappedTestState.Completed, TestProjectId = 1, TenantId = "tenant-1", Description = "123" };
        var oldTestCase = new TestCase { Name = "Test", MappedState = MappedTestState.Completed, TestProjectId = 1, TenantId = "tenant-1", Description = "abc" };
        var notification = new TestCaseSavingEvent(new ClaimsPrincipal(), oldTestCase, newTestCase);

        await handler.Handle(notification, CancellationToken.None);

        Assert.Equal(MappedTestState.Completed, newTestCase.MappedState);
    }

    /// <summary>
    /// Verifies Handle does nothing when Completed state is missing.
    /// </summary>
    [CoveredRequirement("TB-EDITOR-001")]
    [Fact]
    public async Task Handle_DoesNothing_WhenCompletedStateIsMissing()
    {
        var stateService = Substitute.For<IStateService>();
        stateService.GetTestCaseStatesAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<long>()).Returns(new List<TestState>());
        ISettingsProvider settingsProvider = CreateSettingProvider(true);

        var handler = new ChangeStateToOngoingWhenTestIsSavedIfStateIsCompleted(stateService, settingsProvider);
        var newTestCase = new TestCase { Name = "Test", MappedState = MappedTestState.Completed, TestProjectId = 1, TenantId = "tenant-1", Description = "123" };
        var oldTestCase = new TestCase { Name = "Test", MappedState = MappedTestState.Completed, TestProjectId = 1, TenantId = "tenant-1", Description = "abc" };
        var notification = new TestCaseSavingEvent(new ClaimsPrincipal(), oldTestCase, newTestCase);

        await handler.Handle(notification, CancellationToken.None);

        Assert.Equal(MappedTestState.Completed, newTestCase.MappedState);
    }

    /// <summary>
    /// Verifies Handle does nothing when Ongoing state is missing.
    /// </summary>
    [CoveredRequirement("TB-EDITOR-001")]
    [Fact]
    public async Task Handle_DoesNothing_WhenOngoingStateIsMissing()
    {
        var stateService = Substitute.For<IStateService>();
        var completedState = new TestState { MappedState = MappedTestState.Completed };
        stateService.GetTestCaseStatesAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<long>()).Returns(new List<TestState> { completedState });
        ISettingsProvider settingsProvider = CreateSettingProvider(true);

        var handler = new ChangeStateToOngoingWhenTestIsSavedIfStateIsCompleted(stateService, settingsProvider);
        var newTestCase = new TestCase { Name = "Test", MappedState = MappedTestState.Completed, TestProjectId = 1, TenantId = "tenant-1", Description = "123" };
        var oldTestCase = new TestCase { Name = "Test", MappedState = MappedTestState.Completed, TestProjectId = 1, TenantId = "tenant-1", Description = "abc" };
        var notification = new TestCaseSavingEvent(new ClaimsPrincipal(), oldTestCase, newTestCase);

        await handler.Handle(notification, CancellationToken.None);

        Assert.Equal(MappedTestState.Completed, newTestCase.MappedState);
    }

    private static IStateService CreateStateServiceWithBothStates()
    {
        var stateService = Substitute.For<IStateService>();
        var completedState = new TestState { MappedState = MappedTestState.Completed };
        var ongoingState = new TestState { MappedState = MappedTestState.Ongoing };
        stateService.GetTestCaseStatesAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<long>()).Returns(new List<TestState> { completedState, ongoingState });
        return stateService;
    }

    /// <summary>
    /// Verifies that the state changes to Ongoing if the description is changed when the state is Completed and the setting is enabled
    /// </summary>
    [CoveredRequirement("TB-EDITOR-001")]
    [Fact]
    public async Task Handle_ChangesStateToOngoing_WhenDescriptionIsChanged()
    {
        IStateService stateService = CreateStateServiceWithBothStates();
        ISettingsProvider settingsProvider = CreateSettingProvider(true);

        var handler = new ChangeStateToOngoingWhenTestIsSavedIfStateIsCompleted(stateService, settingsProvider);
        var oldTestCase = new TestCase { Name = "Test", MappedState = MappedTestState.Completed, Description = "123", TestProjectId = 1, TenantId = "tenant-1" };
        var newTestCase = new TestCase { Name = "Test", MappedState = MappedTestState.Completed, Description = "abc", TestProjectId = 1, TenantId = "tenant-1" };
        var notification = new TestCaseSavingEvent(new ClaimsPrincipal(), oldTestCase, newTestCase);

        await handler.Handle(notification, CancellationToken.None);

        Assert.Equal(MappedTestState.Ongoing, newTestCase.MappedState);
    }

    /// <summary>
    /// Verifies Handle does nothing when new state is Ongoing.
    /// </summary>
    [CoveredRequirement("TB-EDITOR-001")]
    [Fact]
    public async Task Handle_DoesNothing_WhenNewStateIsOngoing()
    {
        IStateService stateService = CreateStateServiceWithBothStates();
        ISettingsProvider settingsProvider = CreateSettingProvider(true);

        var handler = new ChangeStateToOngoingWhenTestIsSavedIfStateIsCompleted(stateService, settingsProvider);
        var newTestCase = new TestCase { Name = "Test", MappedState = MappedTestState.Completed, TestProjectId = 1, TenantId = "tenant-1", Description = "123" };
        var oldTestCase = new TestCase { Name = "Test", MappedState = MappedTestState.Ongoing, TestProjectId = 1, TenantId = "tenant-1", Description = "abc" };
        var notification = new TestCaseSavingEvent(new ClaimsPrincipal(), oldTestCase, newTestCase);

        await handler.Handle(notification, CancellationToken.None);

        Assert.Equal(MappedTestState.Ongoing, newTestCase.MappedState);
    }

    /// <summary>
    /// Verifies Handle does nothing when new state is Review.
    /// </summary>
    [CoveredRequirement("TB-EDITOR-001")]
    [Fact]
    public async Task Handle_DoesNothing_WhenNewStateIsReview()
    {
        IStateService stateService = CreateStateServiceWithBothStates();
        ISettingsProvider settingsProvider = CreateSettingProvider(true);

        var handler = new ChangeStateToOngoingWhenTestIsSavedIfStateIsCompleted(stateService, settingsProvider);
        var newTestCase = new TestCase { Name = "Test", MappedState = MappedTestState.Completed, TestProjectId = 1, TenantId = "tenant-1", Description = "123" };
        var oldTestCase = new TestCase { Name = "Test", MappedState = MappedTestState.Review, TestProjectId = 1, TenantId = "tenant-1", Description = "abc" };
        var notification = new TestCaseSavingEvent(new ClaimsPrincipal(), oldTestCase, newTestCase);

        await handler.Handle(notification, CancellationToken.None);

        Assert.Equal(MappedTestState.Ongoing, newTestCase.MappedState);
    }

    private static ISettingsProvider CreateSettingProvider(bool enabled)
    {
        var settingsProvider = Substitute.For<ISettingsProvider>();
        settingsProvider.GetDomainSettingsAsync<EditorSettings>(Arg.Any<string>(), Arg.Any<long>()).Returns(Task.FromResult<EditorSettings?>(new EditorSettings { ChangeStateToOngoingWhenEditingTests = enabled }));
        return settingsProvider;
    }
}
