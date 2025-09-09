using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.CodeCoverage.CSharp;

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
}
