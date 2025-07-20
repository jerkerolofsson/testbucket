using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit.Sdk;
using Xunit.v3;

namespace TestBucket.Traits.Core.UnitTests.Xunit.Fakes;
[ExcludeFromCodeCoverage]
internal class FakeXUnitTestCase : IXunitTestCase
{
    public Type[]? SkipExceptions => throw new NotImplementedException();

    public string? SkipReason => throw new NotImplementedException();

    public Type? SkipType => throw new NotImplementedException();

    public string? SkipUnless => throw new NotImplementedException();

    public string? SkipWhen => throw new NotImplementedException();

    public IXunitTestClass TestClass => throw new NotImplementedException();

    public int TestClassMetadataToken => throw new NotImplementedException();

    public string TestClassName => throw new NotImplementedException();

    public string TestClassSimpleName => throw new NotImplementedException();

    public IXunitTestCollection TestCollection => throw new NotImplementedException();

    public IXunitTestMethod TestMethod => throw new NotImplementedException();

    public int TestMethodMetadataToken => throw new NotImplementedException();

    public string TestMethodName => throw new NotImplementedException();

    public string[] TestMethodParameterTypesVSTest => throw new NotImplementedException();

    public string TestMethodReturnTypeVSTest => throw new NotImplementedException();

    public int Timeout => throw new NotImplementedException();

    public bool Explicit => throw new NotImplementedException();

    public string? SourceFilePath => throw new NotImplementedException();

    public int? SourceLineNumber => throw new NotImplementedException();

    public string TestCaseDisplayName => throw new NotImplementedException();

    public string? TestClassNamespace => throw new NotImplementedException();

    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Traits => throw new NotImplementedException();

    public string UniqueID => throw new NotImplementedException();

    public int? TestMethodArity => null;

    ITestClass? ITestCase.TestClass => TestClass;

    ITestCollection ITestCase.TestCollection => TestCollection;

    ITestMethod? ITestCase.TestMethod => TestMethod;

    int? ITestCaseMetadata.TestClassMetadataToken => TestClassMetadataToken;

    int? ITestCaseMetadata.TestMethodMetadataToken => TestMethodMetadataToken;

    public ValueTask<IReadOnlyCollection<IXunitTest>> CreateTests()
    {
        throw new NotImplementedException();
    }

    public void PostInvoke()
    {
        throw new NotImplementedException();
    }

    public void PreInvoke()
    {
        throw new NotImplementedException();
    }
}
