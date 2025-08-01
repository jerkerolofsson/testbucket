using TestBucket.Traits.Core;

namespace TestBucket.Traits.MSTest;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class EndToEndTestAttribute : TestCategoryBaseAttribute
{
    public override IList<string> TestCategories => ["E2E"];
}
