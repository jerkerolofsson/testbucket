
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Xunit;

namespace TestBucket.Traits.Core.UnitTests.Xunit;

[UnitTest]
public class QCharAttributeTests
{
    [Fact]
    public void FunctionalTest()
    {
        var attribute = new FunctionalTestAttribute();
        Assert.Equal(TestTraitNames.QualityCharacteristic, attribute.Name);
        Assert.Equal("Functional Suitability", attribute.Value);
    }

    [Fact]
    public void ReliabilityTest()
    {
        var attribute = new ReliabilityTestAttribute();
        Assert.Equal(TestTraitNames.QualityCharacteristic, attribute.Name);
        Assert.Equal("Reliability", attribute.Value);
    }

    [Fact]
    public void SecurityTest()
    {
        var attribute = new SecurityTestAttribute();
        Assert.Equal(TestTraitNames.QualityCharacteristic, attribute.Name);
        Assert.Equal("Security", attribute.Value);
    }

    [Fact]
    public void PerformanceTest()
    {
        var attribute = new PerformanceTestAttribute();
        Assert.Equal(TestTraitNames.QualityCharacteristic, attribute.Name);
        Assert.Equal("Performance Efficiency", attribute.Value);
    }
}
