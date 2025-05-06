using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Xunit;

using Xunit.Sdk;
using Xunit.v3;

namespace TestBucket.Traits.Core.UnitTests.Xunit.Fakes;

[ExcludeFromCodeCoverage]
[IntegrationTest]
[FunctionalTest]
internal class FakeXunitTestClass : IXunitTestClass
{
    public IReadOnlyCollection<IBeforeAfterTestAttribute> BeforeAfterTestAttributes => throw new NotImplementedException();

    public Type Class => typeof(FakeXunitTestClass);

    public IReadOnlyCollection<Type> ClassFixtureTypes => throw new NotImplementedException();

    public IReadOnlyCollection<ConstructorInfo>? Constructors => throw new NotImplementedException();

    public IReadOnlyCollection<MethodInfo> Methods => Class.GetMethods();

    public ITestCaseOrderer? TestCaseOrderer => throw new NotImplementedException();

    public IXunitTestCollection TestCollection => throw new NotImplementedException();

    public string TestClassName => nameof(FakeXunitTestClass);

    public string? TestClassNamespace => "TestBucket.Traits.Core.UnitTests.Xunit.Fakes";

    public string TestClassSimpleName => TestClassName;

    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Traits => new Dictionary<string, IReadOnlyCollection<string>>();

    public string UniqueID => Guid.NewGuid().ToString();

    ITestCollection ITestClass.TestCollection => TestCollection;
}
