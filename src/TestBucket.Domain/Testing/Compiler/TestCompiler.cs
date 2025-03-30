using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Authentication;

using TestBucket.Domain.Environments;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Tenants.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCases;

namespace TestBucket.Domain.Testing.Compiler;

/// <summary>
/// Takes in input that can contain template references, variables, and replaces it into rendered markup with actual values for variables
/// </summary>
internal class TestCompiler : ITestCompiler
{
    private readonly ITestCaseRepository _testCaseRepository;
    private readonly ITestEnvironmentManager _testEnvironmentManager;

    public TestCompiler(ITestCaseRepository testCaseRepository, ITestEnvironmentManager testEnvironmentManager)
    {
        _testCaseRepository = testCaseRepository;
        _testEnvironmentManager = testEnvironmentManager;
    }

    /// <summary>
    /// Assigns system variable to the context
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task ResolveVariablesAsync(ClaimsPrincipal principal, TestExecutionContext context)
    {
        if(context.TestEnvironmentId is not null)
        {
            var defaultEnvironment = await _testEnvironmentManager.GetTestEnvironmentByIdAsync(principal, context.TestEnvironmentId.Value);
            if (defaultEnvironment is not null)
            {
                foreach (var envVar in defaultEnvironment.Variables)
                {
                    context.Variables[envVar.Key] = envVar.Value;
                }
            }
        }

        if (context.TestCaseId is not null)
        {
            var test = await FindTestCaseByIdAsync(principal, context.TestCaseId.Value);
            if (test is not null)
            {
                context.Variables["TEST_CASE_NAME"] = test.Name;
            }
        }
    }

    private async Task<TestCase?> FindTestCaseByIdAsync(ClaimsPrincipal principal, long id)
    {
        FilterSpecification<TestCase>[] filters = [new FilterByTenant<TestCase>(principal.GetTenantIdOrThrow()), new FilterTestCasesById(id)];
        var tests = await _testCaseRepository.SearchTestCasesAsync(0, 1, filters);
        if (tests.Items.Length > 0)
        {
            var test = tests.Items[0];
            return test;
        }
        return null;
    }


    public async Task<string> CompileAsync(ClaimsPrincipal principal, TestExecutionContext context, string source)
    {
        var compiledWithTemplate = await GetTemplateMarkupAsync(principal, context, source);

        var descriptionWithReplacedVariables = ReplaceVariables(context.Variables, compiledWithTemplate);

        return descriptionWithReplacedVariables;
    }

    public static string ReplaceVariables(Dictionary<string,string> variables, string compiledWithTemplate)
    {
        // @(VARIABLENAME)
        foreach (var variable in variables)
        {
            var pattern = "{{" + variable.Key + "}}";
            compiledWithTemplate = compiledWithTemplate.Replace(pattern, variable.Value);   
        }
        return compiledWithTemplate;
    }

    private async Task<TestCase?> FindTestCaseByNameAsync(ClaimsPrincipal principal, string name)
    {
        FilterSpecification<TestCase>[] filters = [new FilterByTenant<TestCase>(principal.GetTenantIdOrThrow()), new FilterTestCasesByName(name)];
        var tests = await _testCaseRepository.SearchTestCasesAsync(0, 1, filters);
        if (tests.Items.Length > 0)
        {
            var test = tests.Items[0];
            return test;
        }
        return null;
    }

    private async Task<string> GetTemplateMarkupAsync(ClaimsPrincipal principal, TestExecutionContext context, string source)
    {
        string template = "@template";

        string? templateText = null;
        var body = new StringBuilder();
        foreach (var line in source.Split('\n'))
        {
            var processedLine = line;
            if(line.Length > 0 && line[0] == '@')
            {
                if(processedLine.StartsWith(template))
                {
                    var templateName = processedLine.Substring(template.Length).Trim();
                    var test = await FindTestCaseByNameAsync(principal, templateName);
                    if(test?.Description is not null)
                    {
                        templateText = await GetTemplateMarkupAsync(principal, context, test.Description);
                        continue;
                    }
                }
            }

            body.Append(processedLine);
            body.Append('\n');
        }


        var bodyText = body.ToString();
        if(templateText is not null)
        {
            return templateText.Replace("@Body", bodyText);
        }
        return bodyText;
    }
}
