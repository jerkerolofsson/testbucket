using TestBucket.Traits.Core;

namespace TestBucket.Traits.MSTest;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class IntegrationTestAttribute : TestCategoryBaseAttribute
{
    public override IList<string> TestCategories => ["Integration"];
}
