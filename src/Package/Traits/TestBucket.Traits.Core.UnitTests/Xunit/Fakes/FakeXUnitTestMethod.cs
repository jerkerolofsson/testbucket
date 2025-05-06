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
internal class FakeXUnitTestMethod : IXunitTestMethod
{
    public IReadOnlyCollection<IBeforeAfterTestAttribute> BeforeAfterTestAttributes => throw new NotImplementedException();

    public IReadOnlyCollection<IDataAttribute> DataAttributes => throw new NotImplementedException();

    public IReadOnlyCollection<IFactAttribute> FactAttributes => throw new NotImplementedException();

    public bool IsGenericMethodDefinition => false;

    public MethodInfo Method => typeof(FakeXUnitTestMethod).GetMethod(nameof(FakeTestMethod))!;

    public IReadOnlyCollection<ParameterInfo> Parameters => Method.GetParameters();

    public Type ReturnType => typeof(void);

    public object?[] TestMethodArguments => [];

    public IXunitTestClass TestClass => new FakeXunitTestClass();

    public string MethodName => Method.Name;

    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Traits => throw new NotImplementedException();

    public string UniqueID => Guid.NewGuid().ToString();

    ITestClass ITestMethod.TestClass => TestClass;

    public string GetDisplayName(string baseDisplayName, object?[]? testMethodArguments, Type[]? methodGenericTypes)
    {
        throw new NotImplementedException();
    }

    public MethodInfo MakeGenericMethod(Type[] genericTypes)
    {
        throw new NotImplementedException();
    }

    public Type[]? ResolveGenericTypes(object?[] arguments)
    {
        throw new NotImplementedException();
    }

    public object?[] ResolveMethodArguments(object?[] arguments)
    {
        throw new NotImplementedException();
    }

    [TestDescription("Hello World")]
    [TraitAttachmentPropertyAttribute("CustomAttribute", "TheValue")]
    public void FakeTestMethod()
    {

    }
}
