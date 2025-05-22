using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Xunit;

namespace TestBucket.Traits.Core.UnitTests.Xunit;

/// <summary>
/// Contains unit tests for quality characteristic test attributes, verifying their trait names and values.
/// </summary>
[UnitTest]
[EnrichedTest]
public class QCharAttributeTests
{
    /// <summary>
    /// Verifies that <see cref="FunctionalTestAttribute"/> has the correct trait name and value.
    /// </summary>
    [Fact]
    public void FunctionalTest()
    {
        var attribute = new FunctionalTestAttribute();
        Assert.Equal(TestTraitNames.QualityCharacteristic, attribute.Name);
        Assert.Equal("Functional Suitability", attribute.Value);
    }

    /// <summary>
    /// Verifies that <see cref="ReliabilityTestAttribute"/> has the correct trait name and value.
    /// </summary>
    [Fact]
    public void ReliabilityTest()
    {
        var attribute = new ReliabilityTestAttribute();
        Assert.Equal(TestTraitNames.QualityCharacteristic, attribute.Name);
        Assert.Equal("Reliability", attribute.Value);
    }

    /// <summary>
    /// Verifies that <see cref="SecurityTestAttribute"/> has the correct trait name and value.
    /// </summary>
    [Fact]
    public void SecurityTest()
    {
        var attribute = new SecurityTestAttribute();
        Assert.Equal(TestTraitNames.QualityCharacteristic, attribute.Name);
        Assert.Equal("Security", attribute.Value);
    }

    /// <summary>
    /// Verifies that <see cref="PerformanceTestAttribute"/> has the correct trait name and value.
    /// </summary>
    [Fact]
    public void PerformanceTest()
    {
        var attribute = new PerformanceTestAttribute();
        Assert.Equal(TestTraitNames.QualityCharacteristic, attribute.Name);
        Assert.Equal("Performance Efficiency", attribute.Value);
    }
}