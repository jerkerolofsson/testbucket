using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.States;

namespace TestBucket.Domain.UnitTests.States;

/// <summary>
/// Unit tests for the <see cref="StateTranslator"/> class, verifying state translation logic between internal and external representations.
/// </summary>
[UnitTest]
[Component("States & Types")]
[EnrichedTest]
[FunctionalTest]
public class StateTranslatorTests
{
    /// <summary>
    /// Creates a list of <see cref="IssueState"/> objects for use in tests.
    /// </summary>
    /// <returns>A list of <see cref="IssueState"/> instances with various names, mapped states, and aliases.</returns>
    private List<IssueState> GetIssueStates()
    {
        var states = new List<IssueState>();

        states.Add(new IssueState
        {
            Name = "Open",
            MappedState = MappedIssueState.Open
        });

        states.Add(new IssueState
        {
            Name = "Special",
            MappedState = MappedIssueState.Other
        });

        states.Add(new IssueState
        {
            Name = "In Progress",
            MappedState = MappedIssueState.InProgress,
            Aliases = ["ongoing", "in-progress"]
        });

        states.Add(new IssueState
        {
            Name = "Done",
            MappedState = MappedIssueState.Closed,
            Aliases = []
        });

        return states;
    }

    /// <summary>
    /// Tests that <see cref="StateTranslator.GetInternalIssueState(string)"/> returns the correct state when the name matches exactly.
    /// </summary>
    [Fact]
    public void GetInternalIssueState_ExactNameMatch_ReturnsCorrectState()
    {
        var translator = new StateTranslator(GetIssueStates());
        var result = translator.GetInternalIssueState("Open");
        Assert.Equal("Open", result.Name);
        Assert.Equal(MappedIssueState.Open, result.MappedState);
    }

    /// <summary>
    /// Tests that <see cref="StateTranslator.GetInternalIssueState(string)"/> returns the correct state when an alias matches.
    /// </summary>
    [Fact]
    public void GetInternalIssueState_AliasMatch_ReturnsCorrectState()
    {
        var translator = new StateTranslator(GetIssueStates());
        var result = translator.GetInternalIssueState("in-progress");
        Assert.Equal("In Progress", result.Name);
        Assert.Equal(MappedIssueState.InProgress, result.MappedState);
    }

    /// <summary>
    /// Tests that <see cref="StateTranslator.GetInternalIssueState(string)"/> returns the "Other" state when no match is found.
    /// </summary>
    [Fact]
    public void GetInternalIssueState_NoMatch_ReturnsOther()
    {
        var translator = new StateTranslator(GetIssueStates());
        var result = translator.GetInternalIssueState("UnknownState");
        Assert.Equal("Other", result.Name);
        Assert.Equal(MappedIssueState.Other, result.MappedState);
    }

    /// <summary>
    /// Tests that <see cref="StateTranslator.GetExternalIssueState(MappedIssueState, string, string[])"/> returns the correct external name when the name matches exactly.
    /// </summary>
    [Fact]
    public void GetExternalIssueState_ExactNameMatch_ReturnsExternalName()
    {
        var translator = new StateTranslator(GetIssueStates());
        var result = translator.GetExternalIssueState(MappedIssueState.Open, "Open", new[] { "Open", "OPEN", "Closed" });
        Assert.Equal("Open", result);
    }

    /// <summary>
    /// Tests that <see cref="StateTranslator.GetExternalIssueState(MappedIssueState, string, string[])"/> returns the correct external name when an alias matches.
    /// </summary>
    [Fact]
    public void GetExternalIssueState_AliasMatch_ReturnsExternalName()
    {
        var translator = new StateTranslator(GetIssueStates());
        var result = translator.GetExternalIssueState(MappedIssueState.InProgress, "In Progress", new[] { "ongoing", "something" });
        Assert.Equal("ongoing", result);
    }

    /// <summary>
    /// Tests that <see cref="StateTranslator.GetExternalIssueState(MappedIssueState, string, string[])"/> returns the correct external name when a name or alias matches, ignoring the mapped state.
    /// </summary>
    [Fact]
    public void GetExternalIssueState_NameOrAliasMatchIgnoringMappedState_ReturnsExternalName()
    {
        var translator = new StateTranslator(GetIssueStates());
        var result = translator.GetExternalIssueState(MappedIssueState.Open, "Open", new[] { "Done", "in-progress" });
        Assert.Equal("in-progress", result);
    }

    /// <summary>
    /// Tests that <see cref="StateTranslator.GetExternalIssueState(MappedIssueState, string, string[])"/> returns the first external name when no match is found.
    /// </summary>
    [Fact]
    public void GetExternalIssueState_NoMatch_ReturnsFirstExternalName()
    {
        var translator = new StateTranslator(GetIssueStates());
        var result = translator.GetExternalIssueState(MappedIssueState.Open, "Open", new[] { "NotAState", "Another" });
        Assert.Equal("NotAState", result);
    }
}