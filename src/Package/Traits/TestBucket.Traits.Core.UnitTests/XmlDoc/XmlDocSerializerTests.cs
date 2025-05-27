using TestBucket.Traits.Core.XmlDoc;
using TestBucket.Traits.Core.XmlDoc.Models;
using TestBucket.Traits.Xunit;

namespace TestBucket.Traits.Core.UnitTests.XmlDoc;

/// <summary>
/// Unit tests for <see cref="XmlDocSerializer"/>.
/// </summary>
[EnrichedTest]
[UnitTest]
[Component("Traits")]
[Feature("Import Test Results")]
[FunctionalTest]
public class XmlDocSerializerTests
{
    /// <summary>
    /// Verifies that ParseXml returns a result containing the name of the assembly
    /// </summary>
    [Fact]
    public void DeserializeXmlDoc_WithAssembly_AssemblyNameParsed()
    {
        string xml = """
            <?xml version="1.0"?>
            <doc>
                <assembly>
                    <name>TestBucket.Traits.Core.UnitTests</name>
                </assembly>
            </doc>
            """;

        // Act
        var xmlDoc = XmlDocSerializer.ParseXml(xml);

        // Assert
        Assert.NotNull(xmlDoc.Assembly);
        Assert.NotNull(xmlDoc.Assembly.Name);
        Assert.Equal("TestBucket.Traits.Core.UnitTests", xmlDoc.Assembly.Name);
    }

    /// <summary>
    /// Verifies that ParseXml returns a document with an empty Members list when no members are present.
    /// </summary>
    [Fact]
    public void ParseXml_NoMembers_ReturnsEmptyMembers()
    {
        string xml = """
            <?xml version="1.0"?>
            <doc>
                <assembly>
                    <name>TestBucket.Traits.Core.UnitTests</name>
                </assembly>
                <members>
                </members>
            </doc>
            """;

        var xmlDoc = XmlDocSerializer.ParseXml(xml);

        Assert.NotNull(xmlDoc.Members);
        Assert.Empty(xmlDoc.Members);
    }

    /// <summary>
    /// Verifies that ParseXml correctly parses a single property with summary
    /// </summary>
    [Fact]
    public void ParseXml_SingleType_ParsesMember()
    {
        string xml = """
            <?xml version="1.0"?>
            <doc>
                <assembly>
                    <name>TestBucket.Traits.Core.UnitTests</name>
                </assembly>
                <members>
                    <member name="T:TestClass">
                        <summary>This is a test type.</summary>
                    </member>
                </members>
            </doc>
            """;

        var xmlDoc = XmlDocSerializer.ParseXml(xml);

        Assert.Single(xmlDoc.Members);
        var member = xmlDoc.Members[0];
        Assert.Equal("TestClass", member.Name);
        Assert.Equal(XmlDocMemberType.Type, member.Type);
        Assert.Equal("This is a test type.", member.Summary);
        Assert.Empty(member.Params);
    }

    /// <summary>
    /// Verifies that ParseXml correctly parses a single property with summary
    /// </summary>
    [Fact]
    public void ParseXml_SingleProperty_ParsesMember()
    {
        string xml = """
            <?xml version="1.0"?>
            <doc>
                <assembly>
                    <name>TestBucket.Traits.Core.UnitTests</name>
                </assembly>
                <members>
                    <member name="P:TestClass.MyProperty">
                        <summary>This is a test property.</summary>
                    </member>
                </members>
            </doc>
            """;

        var xmlDoc = XmlDocSerializer.ParseXml(xml);

        Assert.Single(xmlDoc.Members);
        var member = xmlDoc.Members[0];
        Assert.Equal("TestClass.MyProperty", member.Name);
        Assert.Equal(XmlDocMemberType.Property, member.Type);
        Assert.Equal("This is a test property.", member.Summary);
        Assert.Empty(member.Params);
    }


    /// <summary>
    /// Verifies that ParseXml correctly parses a single property with summary
    /// </summary>
    [Fact]
    public void ParseXml_MethodWithArguments_ParsesMember()
    {
        string xml = """
            <?xml version="1.0"?>
            <doc>
                <assembly>
                    <name>TestBucket.Traits.Core.UnitTests</name>
                </assembly>
                <members>
                    <member name="M:TestBucket.Traits.Core.UnitTests.TraitTypeConverterTests.ConvertFromStringToKnownType_IsValid(System.String,TestBucket.Traits.Core.TraitType)">
                        <summary>
                            Verifies that converting from known trait name strings returns the correct <see cref="T:TestBucket.Traits.Core.TraitType"/> value.
                        </summary>
                        <param name="traitName">The trait name string to convert.</param>
                        <param name="expectedType">The expected <see cref="T:TestBucket.Traits.Core.TraitType"/> value.</param>
                    </member>
                </members>
            </doc>
            """;

        var xmlDoc = XmlDocSerializer.ParseXml(xml);

        Assert.Single(xmlDoc.Members);
        var member = xmlDoc.Members[0];
        Assert.Equal("TestBucket.Traits.Core.UnitTests.TraitTypeConverterTests.ConvertFromStringToKnownType_IsValid(System.String,TestBucket.Traits.Core.TraitType)", member.Name);
        Assert.Equal(XmlDocMemberType.Method, member.Type);
        Assert.Empty(member.Params);
    }

    /// <summary>
    /// Verifies that ParseXml correctly parses a single property with summary
    /// </summary>
    [Fact]
    public void ParseXml_SingleField_ParsesMember()
    {
        string xml = """
            <?xml version="1.0"?>
            <doc>
                <assembly>
                    <name>TestBucket.Traits.Core.UnitTests</name>
                </assembly>
                <members>
                    <member name="F:TestClass._foobar">
                        <summary>This is a test field.</summary>
                    </member>
                </members>
            </doc>
            """;

        var xmlDoc = XmlDocSerializer.ParseXml(xml);

        Assert.Single(xmlDoc.Members);
        var member = xmlDoc.Members[0];
        Assert.Equal("TestClass._foobar", member.Name);
        Assert.Equal(XmlDocMemberType.Field, member.Type);
        Assert.Equal("This is a test field.", member.Summary);
        Assert.Empty(member.Params);
    }

    /// <summary>
    /// Verifies that ParseXml correctly parses a single method with summary and parameters.
    /// </summary>
    [Fact]
    public void ParseXml_SingleMethod_ParsesMemberAndParams()
    {
        string xml = """
            <?xml version="1.0"?>
            <doc>
                <assembly>
                    <name>TestBucket.Traits.Core.UnitTests</name>
                </assembly>
                <members>
                    <member name="M:TestClass.TestMethod">
                        <summary>This is a test method.</summary>
                        <param name="param1">The first parameter.</param>
                        <param name="param2">The second parameter.</param>
                    </member>
                </members>
            </doc>
            """;

        var xmlDoc = XmlDocSerializer.ParseXml(xml);

        Assert.Single(xmlDoc.Members);
        var member = xmlDoc.Members[0];
        Assert.Equal("TestClass.TestMethod", member.Name);
        Assert.Equal(XmlDocMemberType.Method, member.Type);
        Assert.Equal("This is a test method.", member.Summary);
        Assert.Equal(2, member.Params.Count);
        Assert.Equal("param1", member.Params[0].Name);
        Assert.Equal("The first parameter.", member.Params[0].Description);
        Assert.Equal("param2", member.Params[1].Name);
        Assert.Equal("The second parameter.", member.Params[1].Description);
    }

    /// <summary>
    /// Verifies that ParseXml ignores members with missing or invalid name attributes.
    /// </summary>
    [Fact]
    public void ParseXml_MemberWithMissingName_IgnoresMember()
    {
        string xml = """
            <?xml version="1.0"?>
            <doc>
                <assembly>
                    <name>TestBucket.Traits.Core.UnitTests</name>
                </assembly>
                <members>
                    <member>
                        <summary>Should be ignored.</summary>
                    </member>
                </members>
            </doc>
            """;

        var xmlDoc = XmlDocSerializer.ParseXml(xml);

        Assert.Empty(xmlDoc.Members);
    }

    /// <summary>
    /// Verifies that ParseXml throws an exception for invalid XML.
    /// </summary>
    [Fact]
    public void ParseXml_InvalidXml_ThrowsException()
    {
        string xml = "<doc><assembly><name>Test</name></assembly><members><member></doc>"; // Malformed XML

        Assert.ThrowsAny<System.Xml.XmlException>(() => XmlDocSerializer.ParseXml(xml));
    }
}