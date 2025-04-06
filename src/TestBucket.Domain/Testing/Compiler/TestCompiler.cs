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
    /// 
    /// 1. Test suite
    /// 2. Test case
    /// 3. Environment
    /// 
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task ResolveVariablesAsync(ClaimsPrincipal principal, TestExecutionContext context)
    {
        context.Variables["TB_RUN_ID"] = context.TestRunId.ToString();
        context.Variables["TB_PROJECT_ID"] = context.ProjectId.ToString();

        if (context.TestEnvironmentId is not null)
        {
            context.Variables["TB_ENVIRONMENT_ID"] = context.TestEnvironmentId.Value.ToString();
        }

        if (context.TestCaseId is not null)
        {
            var test = await FindTestCaseByIdAsync(principal, context.TestCaseId.Value);
            if (test is not null)
            {
                context.Variables["TEST_CASE_NAME"] = test.Name;
                if (test.ModifiedBy is not null)
                {
                    context.Variables["TEST_CASE_MODIFIED_BY"] = test.ModifiedBy;
                }
                if (test.CreatedBy is not null)
                {
                    context.Variables["TEST_CASE_CREATED_BY"] = test.CreatedBy;
                }

                if (test.TestParameters is not null)
                {
                    foreach (var envVar in test.TestParameters)
                    {
                        context.Variables[envVar.Key] = envVar.Value;
                    }
                }
            }
        }

        if (context.TestSuiteId is not null)
        {
            var tenantId = principal.GetTenantIdOrThrow();
            var testSuite = await _testCaseRepository.GetTestSuiteByIdAsync(tenantId, context.TestSuiteId.Value);
            if (testSuite?.Variables is not null)
            {
                foreach (var envVar in testSuite.Variables)
                {
                    context.Variables[envVar.Key] = envVar.Value;
                }
            }
        }

        if (context.TestEnvironmentId is not null)
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
        context.CompilerErrors.Clear();
        var compiledWithTemplate = await GetTemplateMarkupAsync(principal, context, source);

        var descriptionWithReplacedVariables = ReplaceVariables(context.Variables, compiledWithTemplate, context);

        return descriptionWithReplacedVariables;
    }

    private static readonly Regex s_regexVariable = new Regex("{{([^}]+)}}");

    public static string ReplaceVariables(Dictionary<string,string> variables, string compiledWithTemplate, TestExecutionContext context)
    {
        var lines = compiledWithTemplate.Split('\n');
        List<string> outputLines = new List<string>(lines.Length);

        int lineNumber = 0;
        foreach (var line in lines)
        {
            var outputLine = line;

            lineNumber++;
            int count = 0;
            int pos = 0;
            while (true)
            {
                var match = s_regexVariable.Match(outputLine, pos);
                if (!match.Success)
                {
                    break;
                }

                var start = outputLine[0..match.Index];
                var variablePatternMatch = outputLine[match.Index..(match.Length+match.Index)];
                var end = outputLine[(match.Index + match.Length)..];

                var variable = variablePatternMatch.TrimStart('{').TrimEnd('}');
                if (variables.TryGetValue(variable, out var value))
                {
                    pos = match.Index + 1;

                    outputLine = start + value + end;
                }
                else
                {
                    // The variable could not be found
                    context.CompilerErrors.Add(new Contracts.Testing.Models.CompilerError
                    {
                        Code = 1,
                        Line = lineNumber,
                        Column = match.Index + 1,
                        Message = $"The variable {variablePatternMatch} was not found"
                    });

                    pos = match.Index + match.Length;
                }

                count++;
                if (count > 10000)
                {
                    throw new Exception("Failed to replace variable, perhaps a variable value contains a variable itself");
                }
            }
            outputLines.Add(outputLine);
        }

        //foreach (var variable in variables)
        //{
        //    var pattern = "{{" + variable.Key + "}}";
        //    compiledWithTemplate = compiledWithTemplate.Replace(pattern, variable.Value);   
        //}
        return string.Join("\n", outputLines);
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
