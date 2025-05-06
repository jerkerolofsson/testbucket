using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Xunit;

namespace TestBucket.Traits.Core.UnitTests;

[Feature("Traits")]
[UnitTest]
public class AutomationTraitNamesTests
{
    [Fact]
    public void Assembly_IsCorrect()
    {
        Assert.Equal(nameof(AutomationTraitNames.Assembly), AutomationTraitNames.Assembly);
    }

    [Fact]
    public void ClassName_IsCorrect()
    {
        Assert.Equal(nameof(AutomationTraitNames.ClassName), AutomationTraitNames.ClassName);
    }

    [Fact]
    public void Module_IsCorrect()
    {
        Assert.Equal(nameof(AutomationTraitNames.Module), AutomationTraitNames.Module);
    }
}
