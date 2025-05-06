using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Xunit;

namespace TestBucket.Traits.Core.UnitTests.Xunit;

[UnitTest]
public class TestTraitAttributeTests
{
    [Fact]
    public void CustomCategoryTest()
    {
        var attribute = new TestCategoryAttribute("CustomCategory");
        Assert.Equal(TestTraitNames.TestCategory, attribute.Key);
        Assert.Equal("CustomCategory", attribute.Value);
    }

    [Fact]
    public void ApiTest()
    {
        var attribute = new ApiTestAttribute();
        Assert.Equal(TestTraitNames.TestCategory, attribute.Key);
        Assert.Equal("API", attribute.Value);
    }
    [Fact]
    public void UnitTest()
    {
        var attribute = new UnitTestAttribute();
        Assert.Equal(TestTraitNames.TestCategory, attribute.Key);
        Assert.Equal("Unit", attribute.Value);
    }
    [Fact]
    public void IntegrationTest()
    {
        var attribute = new IntegrationTestAttribute();
        Assert.Equal(TestTraitNames.TestCategory, attribute.Key);
        Assert.Equal("Integration", attribute.Value);
    }

    [Fact]
    public void End2EndTest()
    {
        var attribute = new EndToEndTestAttribute();
        Assert.Equal(TestTraitNames.TestCategory, attribute.Key);
        Assert.Equal("E2E", attribute.Value);
    }
}
