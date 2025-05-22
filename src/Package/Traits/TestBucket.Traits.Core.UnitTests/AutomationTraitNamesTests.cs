using TestBucket.Traits.Xunit;

namespace TestBucket.Traits.Core.UnitTests;

/// <summary>
/// Unit tests for <see cref="AutomationTraitNames"/> to verify that trait name constants are correct.
/// </summary>
[Feature("Import Test Results")]
[Component("Traits")]
[UnitTest]
[EnrichedTest]
[EnrichedTest]

public class AutomationTraitNamesTests
{
    /// <summary>
    /// Verifies that <see cref="AutomationTraitNames.Assembly"/> returns the correct constant name.
    /// </summary>
    [Fact]
    public void Assembly_IsCorrect()
    {
        Assert.Equal(nameof(AutomationTraitNames.Assembly), AutomationTraitNames.Assembly);
    }

    /// <summary>
    /// Verifies that <see cref="AutomationTraitNames.ClassName"/> returns the correct constant name.
    /// </summary>
    [Fact]
    public void ClassName_IsCorrect()
    {
        Assert.Equal(nameof(AutomationTraitNames.ClassName), AutomationTraitNames.ClassName);
    }

    /// <summary>
    /// Verifies that <see cref="AutomationTraitNames.Module"/> returns the correct constant name.
    /// </summary>
    [Fact]
    public void Module_IsCorrect()
    {
        Assert.Equal(nameof(AutomationTraitNames.Module), AutomationTraitNames.Module);
    }
}