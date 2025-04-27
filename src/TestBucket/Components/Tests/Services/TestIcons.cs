using TestBucket.Domain;

namespace TestBucket.Components.Tests.Services;

public static class TestIcons
{
    public static string GetIcon(TestCase testCase)
    {
        if (testCase.IsTemplate)
        {
            return Icons.Material.Filled.DocumentScanner;
        }
        if (testCase.ExecutionType == TestExecutionType.Automated)
        {
            return TbIcons.BoldDuoTone.CodeSquare;
        }
        if (testCase.ExecutionType == TestExecutionType.Hybrid)
        {
            return Icons.Material.Filled.Api;
        }
        return TbIcons.BoldDuoTone.File;
    }
}
