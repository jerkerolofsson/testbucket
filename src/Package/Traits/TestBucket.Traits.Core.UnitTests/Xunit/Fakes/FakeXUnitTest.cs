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
public class FakeXUnitTest : IXunitTest
{
    public bool Explicit { get; set; }

    public string? SkipReason { get; set; }

    public IXunitTestCase TestCase { get; } = new FakeXUnitTestCase();

    public IXunitTestMethod TestMethod { get; } = new FakeXUnitTestMethod();

    public object?[] TestMethodArguments { get; set; } = [];

    public int Timeout { get; set; }

    public string TestDisplayName { get; set; } = "FakeTestDisplayName";

    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Traits { get; } = new Dictionary<string, IReadOnlyCollection<string>>();

    public string UniqueID { get; } = Guid.NewGuid().ToString();

    ITestCase ITest.TestCase => TestCase;
}
