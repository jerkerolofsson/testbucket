namespace TestBucket.Traits.MSTest;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class ApiTestAttribute : TestCategoryBaseAttribute
{
    public override IList<string> TestCategories => ["API"];
}
