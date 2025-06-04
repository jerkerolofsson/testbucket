using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Core.XmlDoc;
using TestBucket.Traits.Xunit;

namespace TestBucket.Traits.Core.UnitTests.XmlDoc;

/// <summary>
/// Tests for XmlDocMarkdownBuilder
/// </summary>
[EnrichedTest]
[UnitTest]
[Component("Traits")]
[Feature("Import Test Results")]
[FunctionalTest]
public class XmlDocMarkdownBuilderTests
{
    /// <summary>
    /// Verifies that the test case description generated from an XML doc method contains the method without signature and withoput parameters
    /// in the markdown table.
    /// </summary>
    [Fact]
    public void BuildXmlDocSummary_WithMethod_ContainsMethodWithoutSignature()
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

        var builder = new XmlDocMarkdownBuilder();
        builder.AddMethod(xmlDoc, xmlDoc.Members.Where(x => x.Type == Core.XmlDoc.Models.XmlDocMemberType.Method).First());

        // Act
        var summary = builder.Build();

        // Assert
        Assert.Contains("| ConvertFromStringToKnownType_IsValid |", summary); // Method without signature
    }
}
