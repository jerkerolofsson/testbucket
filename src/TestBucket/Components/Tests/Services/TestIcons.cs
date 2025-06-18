using TestBucket.Domain;

namespace TestBucket.Components.Tests.Services;

public static class TestIcons
{
    public static string GetIcon(TestCase testCase)
    {
        if(testCase.ScriptType == ScriptType.Exploratory)
        {
            return TbIcons.BoldDuoTone.MapArrowUp;
        }

        if (testCase.IsTemplate)
        {
            return TbIcons.BoldDuoTone.TestTemplate;
        }
        if (testCase.ExecutionType == TestExecutionType.Automated)
        {
            return TbIcons.BoldDuoTone.CodeSquare;
        }
        if (testCase.ExecutionType == TestExecutionType.Hybrid)
        {
            return Icons.Material.Filled.Api;
        }
        return TbIcons.BoldDuoTone.FileText;
    }
}
