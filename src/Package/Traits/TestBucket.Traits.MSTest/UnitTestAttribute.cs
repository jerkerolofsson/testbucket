using TestBucket.Traits.Core;

namespace TestBucket.Traits.MSTest;
public class UnitTestAttribute : TestCategoryBaseAttribute
{
    public override IList<string> TestCategories => ["Unit"];
}
