using System.Xml.Linq;
using TestBucket.AdbProxy.Appium.PageSources;
using TestBucket.Traits.Xunit;
using Xunit;

namespace TestBucket.AdbProxy.UnitTests.Appium;

[EnrichedTest]
[UnitTest]
[Component("Appium")]
[Feature("Test Resources")]
public class XmlPageSourceTests
{
    [Fact]
    public void FromString_ValidXml_ParsesCorrectly()
    {
        // Arrange
        string xml = """
            <hierarchy rotation="90" width="1080" height="1920">
                <TestClass index="0" class="TestClass" package="TestPackage" resource-id="TestId" content-desc="TestDesc" />
            </hierarchy>
        """;

        // Act
        var result = XmlPageSource.FromString(xml);

        // Assert
        Assert.Equal(90, result.Rotation);
        Assert.Equal(1080, result.Width);
        Assert.Equal(1920, result.Height);
        Assert.Single(result.Nodes);
        Assert.Equal("TestClass", result.Nodes[0].TagName);
        Assert.Equal("TestClass", result.Nodes[0].ClassName);
        Assert.Equal("TestPackage", result.Nodes[0].PackageName);
        Assert.Equal("TestId", result.Nodes[0].ResourceId);
        Assert.Equal("TestDesc", result.Nodes[0].ContentDescription);
    }

    [Fact]
    public void FromString_InvalidXml_ThrowsException()
    {
        // Arrange
        string invalidXml = "<hierarchy rotation=\"90\" width=\"1080\" height=\"1920\">"; // Missing closing tag

        // Act & Assert
        Assert.Throws<System.Xml.XmlException>(() => XmlPageSource.FromString(invalidXml));
    }

    [Fact]
    public void FromString_EmptyXml_ReturnsDefaultValues()
    {
        // Arrange
        string emptyXml = "<hierarchy></hierarchy>";

        // Act
        var result = XmlPageSource.FromString(emptyXml);

        // Assert
        Assert.Equal(0, result.Rotation);
        Assert.Equal(0, result.Width);
        Assert.Equal(0, result.Height);
        Assert.Empty(result.Nodes);
    }

    [Fact]
    public void FromString_ValidXml_SetsParentCorrectly()
    {
        // Arrange
        string xml = """
            <hierarchy rotation="90" width="1080" height="1920">
                <ParentNode index="0" class="ParentClass">
                    <ChildNode index="1" class="ChildClass" />
                </ParentNode>
            </hierarchy>
        """;

        // Act
        var result = XmlPageSource.FromString(xml);

        // Assert
        Assert.Single(result.Nodes);
        var parentNode = result.Nodes[0];
        Assert.Equal("ParentNode", parentNode.TagName);
        Assert.Single(parentNode.Nodes);
        var childNode = parentNode.Nodes[0];
        Assert.Equal("ChildNode", childNode.TagName);
        Assert.Equal(parentNode, childNode.Parent);
    }

    [Fact]
    public void FromString_ValidXmlWithBounds_ParsesBoundsCorrectly()
    {
        // Arrange
        string xml = """
            <hierarchy rotation="0" width="1080" height="1920">
                <Node index="0" class="TestClass" bounds="[0,0][1096,2491]" />
            </hierarchy>
        """;

        // Act
        var result = XmlPageSource.FromString(xml);

        // Assert
        Assert.Single(result.Nodes);
        var node = result.Nodes[0];
        Assert.Equal(0, node.X);
        Assert.Equal(0, node.Y);
        Assert.Equal(1096, node.Width);
        Assert.Equal(2491, node.Height);
        Assert.Equal(0, node.Left);
        Assert.Equal(1096, node.Right);
        Assert.Equal(0, node.Top);
        Assert.Equal(2491, node.Bottom);
    }
}
