using TestBucket.Domain;
using TestBucket.Domain.Automation.Pipelines.Models;

namespace TestBucket.Components.Tests.Services;

public static class TestIcons
{
    public static string GetIcon(Pipeline pipeline)
    {
        if (pipeline.CiCdSystem?.ToLower() == "gitlab")
        {
            return TbIcons.Brands.Gitlab;
        }
        if (pipeline.CiCdSystem?.ToLower() == "github")
        {
            return Icons.Custom.Brands.GitHub;
        }
        return TbIcons.BoldDuoTone.Rocket;
    }

    public static string GetIcon(TestSuiteFolder folder)
    {
        if (!string.IsNullOrEmpty(folder.Icon))
        {
            return folder.Icon;
        }

        return TbIcons.BoldOutline.Folder;
    }
    public static string GetIcon(TestSuite suite)
    {
        if (!string.IsNullOrEmpty(suite.Icon))
        {
            return suite.Icon;
        }

        return TbIcons.BoldDuoTone.Box;
    }
    public static string GetIcon(TestRepositoryFolder folder)
    {
        if (!string.IsNullOrEmpty(folder.Icon))
        {
            return folder.Icon;
        }

        return TbIcons.BoldOutline.Folder;
    }
    public static string GetIcon(TestLabFolder folder)
    {
        if (!string.IsNullOrEmpty(folder.Icon))
        {
            return folder.Icon;
        }

        return TbIcons.BoldOutline.Folder;
    }

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
