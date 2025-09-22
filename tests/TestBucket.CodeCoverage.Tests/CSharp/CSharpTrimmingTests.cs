using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.CodeCoverage.CSharp;
using TestBucket.CodeCoverage.Parsers;

namespace TestBucket.CodeCoverage.Tests.CSharp;

/// <summary>
/// Tests that trim names of generated classes and methods, such as async and generic classes..
/// </summary>
[Feature("Code Coverage")]
[Component("Code Coverage")]
[UnitTest]
[EnrichedTest]
[FunctionalTest]
public class CSharpTrimmingTests
{
    /// <summary>
    /// Verifies that a generic class name is cleaned up correctly.
    /// </summary>
    [Fact]
    public void CleanupClassName_WithGenericArgument_Success()
    {
        var displayName = "TestBucket.Domain.Export.Zip.ZipImporter.<DeserializeEntityAsync>d__4";
        var cleanedName = CSharpCleanup.CleanupClassName(displayName);
        Assert.Equal("TestBucket.Domain.Export.Zip.ZipImporter", cleanedName);
    }

    /// <summary>
    /// Verifies that TestBucket.Domain.Export.Zip.ZipImporter.&lt;DeserializeEntityAsync&gt;d__4&lt;T&gt; is cleaned up correctly.
    /// </summary>
    [Fact]
    public void CleanupClassName_WithGenericArgumentDouble_Success()
    {
        var displayName = "TestBucket.Domain.Export.Zip.ZipImporter.<DeserializeEntityAsync>d__4<T>";
        var cleanedName = CSharpCleanup.CleanupClassName(displayName);
        Assert.Equal("TestBucket.Domain.Export.Zip.ZipImporter", cleanedName);
    }


    /// <summary>
    /// Verifies that CleanupMethodName removes template parameters
    /// </summary>
    [Theory]
    [InlineData("<GetOptionsAsync>b__12_0")]
    [InlineData("<GetOptionsAsync>b__2_0")]
    public void CleanupMethodName_RemovesTemplateParameters(string displayName)
    {
        var cleanedName = CSharpCleanup.CleanupMethodName(displayName);
        Assert.Equal("GetOptionsAsync", cleanedName);
    }


    /// <summary>
    /// Verifies that TestBucket.Domain.Export.Zip.ZipImporter.&lt;DeserializeEntityAsync&gt;d__4&lt;T&gt; is cleaned up correctly.
    /// </summary>
    [Fact]
    public async Task ParseTextAsync_WithGenericOverloadedMethods()
    {
        string xml = """
            <coverage line-rate="0.5" branch-rate="0" complexity="10485" version="1.9" timestamp="1757424625" lines-covered="7612" lines-valid="24908" branches-covered="1737" branches-valid="6670">
                <packages>
                    <package line-rate="0.5" branch-rate="0" complexity="10462" name="TestBucket.Domain">
                        <classes>
                            <class line-rate="0" branch-rate="1" complexity="3" name="TestBucket.Domain.Fields.FieldDefinitionManager.&lt;&gt;c" filename="src\TestBucket.Domain\Fields\FieldDefinitionManager.cs">
                              <methods>
                                <method line-rate="0" branch-rate="1" complexity="1" name="&lt;GetOptionsAsync&gt;b__12_0" signature="(string)">
                                  <lines>
                                    <line number="165" hits="0" branch="False" />
                                  </lines>
                                </method>
                                <method line-rate="0" branch-rate="1" complexity="1" name="&lt;GetOptionsAsync&gt;b__12_1" signature="(string)">
                                  <lines>
                                    <line number="165" hits="0" branch="False" />
                                  </lines>
                                </method>
                                <method line-rate="0" branch-rate="1" complexity="1" name="&lt;GetOptionsAsync&gt;b__12_3" signature="(TestBucket.Contracts.Integrations.GenericVisualEntity)">
                                  <lines>
                                    <line number="201" hits="0" branch="False" />
                                  </lines>
                                </method>
                              </methods>
                              <lines>
                                <line number="165" hits="0" branch="False" />
                                <line number="201" hits="0" branch="False" />
                              </lines>
                            </class>
                        </classes>
                    </package>
                </packages>
            </coverage>
            """;

        var parser = new CoberturaParser();
        var report = await parser.ParseTextAsync(xml, TestContext.Current.CancellationToken);

        Assert.Single(report.Packages);
        Assert.Single(report.Packages[0].Classes);
        var fieldDefinitionManagerClass = report.Packages[0].Classes[0];
        Assert.Equal(2, fieldDefinitionManagerClass.Methods.Count);

        Assert.Equal("TestBucket.Domain.Fields.FieldDefinitionManager", report.Packages[0].Classes[0].Name);

        var method1 = report.Packages[0].Classes[0].Methods[0];
        var method2 = report.Packages[0].Classes[0].Methods[1];
        Assert.Equal("GetOptionsAsync", method1.Name);
        Assert.Equal("(string)", method1.Signature);
        Assert.Equal("GetOptionsAsync", method2.Name);
        Assert.Equal("(TestBucket.Contracts.Integrations.GenericVisualEntity)", method2.Signature);
    }
}
