using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

using TestBucket.Contracts.TestAccounts;
using TestBucket.Contracts.TestResources;
using TestBucket.Domain.Environments;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Tenants.Models;
using TestBucket.Domain.TestAccounts.Allocation;
using TestBucket.Domain.TestAccounts.Mapping;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCases;
using TestBucket.Domain.TestResources.Allocation;
using TestBucket.Domain.TestResources.Mapping;

namespace TestBucket.Domain.Testing.Compiler;

/// <summary>
/// Takes in input that can contain template references, variables, and replaces it into rendered markup with actual values for variables
/// </summary>
internal class TestCompiler : ITestCompiler
{
    private readonly ITestCaseRepository _testCaseRepository;
    private readonly ITestEnvironmentManager _testEnvironmentManager;
    private readonly TestResourceDependencyAllocator _resourceAllocator;
    private readonly TestAccountDependencyAllocator _accountAllocator;

    public TestCompiler(
        ITestCaseRepository testCaseRepository,
        ITestEnvironmentManager testEnvironmentManager,
        TestAccountDependencyAllocator accountAllocator,
        TestResourceDependencyAllocator resourceAllocator)
    {
        _testCaseRepository = testCaseRepository;
        _testEnvironmentManager = testEnvironmentManager;
        _resourceAllocator = resourceAllocator;
        _accountAllocator = accountAllocator;
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
    public async Task ResolveVariablesAsync(
        ClaimsPrincipal principal, 
        TestExecutionContext context, 
        CancellationToken cancellationToken)
    {
        // This will allocate resource and lock them.
        // To unlock resources, use context.Guid as the lock owner.
        // @see ReleaseResourcesRequest
        var resourceBag = await _resourceAllocator.CollectDependenciesAsync(principal, context, cancellationToken);
        context.Resources = new List<TestResourceDto>();
        foreach(var resource in resourceBag.Resources)
        {
            context.Resources.Add(resource.ToDto());
        }

        var accountBag = await _accountAllocator.CollectDependenciesAsync(principal, context, cancellationToken);
        context.Accounts = new List<TestAccountDto>();
        foreach (var account in accountBag.Accounts)
        {
            context.Accounts.Add(account.ToDto());
        }

        if (context.TestRunId is not null)
        {
            context.Variables["TB_RUN_ID"] = context.TestRunId.Value.ToString();
        }
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


        if (context.TestCaseId is not null)
        {
            var tenantId = principal.GetTenantIdOrThrow();
            var testCase = await _testCaseRepository.GetTestCaseByIdAsync(tenantId, context.TestCaseId.Value);
            //if (testCase?.Env is not null)
            //{
            //    foreach (var envVar in testCase.Variables)
            //    {
            //        context.Variables[envVar.Key] = envVar.Value;
            //    }
            //}
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
        var compiledWithTemplate = await CompileMarkupAsync(principal, context, source, FindTestCaseByNameAsync);

        var descriptionWithReplacedVariables = ReplaceVariables(context.Variables, compiledWithTemplate, context);

        return descriptionWithReplacedVariables;
    }

    private static readonly Regex s_regexVariable = new Regex("{{([^}]+)}}");

    /// <summary>
    /// Scans the code for variables
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static HashSet<string> FindVariables(string text)
    {
        var lines = text.Split('\n');
        HashSet<string> variables = [];
        int lineNumber = 0;
        foreach (var line in lines)
        {
            var outputLine = line;

            lineNumber++;
            int pos = 0;
            while (true)
            {
                var match = s_regexVariable.Match(outputLine, pos);
                if (!match.Success)
                {
                    break;
                }

                var start = outputLine[0..match.Index];
                var variablePatternMatch = outputLine[match.Index..(match.Length + match.Index)];
                var end = outputLine[(match.Index + match.Length)..];

                var variable = variablePatternMatch.TrimStart('{').TrimEnd('}');
                if (!variables.Contains(variable))
                {
                    variables.Add(variable);
                }
                pos = match.Index + match.Length;
            }
        }

        return variables;
    }

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
                    // If this is a "special variable", ignore it as these are runtime errors and not static related to compilation
                    if (!variable.StartsWith("resources__") && !variable.StartsWith("accounts__"))
                    {
                        // The variable could not be found
                        context.CompilerErrors.Add(new Contracts.Testing.Models.CompilerError
                        {
                            Code = 1,
                            Line = lineNumber,
                            Column = match.Index + 1,
                            Message = $"The variable {variablePatternMatch} was not found"
                        });
                    }

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

    public static async Task<string> CompileMarkupAsync(
        ClaimsPrincipal principal, 
        TestExecutionContext context, 
        string source, 
        Func<ClaimsPrincipal, string, Task<TestCase?>> FindTestCaseFunc)
    {
        // Directives
        string template = "@template";
        string include = "@include";

        string? templateText = null;
        var body = new StringBuilder();
        bool isFirstLine = true;
        foreach (var line in source.Split('\n'))
        {
            var processedLine = line;
            if(line.Length > 0 && line[0] == '@')
            {
                if(processedLine.StartsWith(template))
                {
                    // The line starts with a @template directive
                    var templateName = processedLine.Substring(template.Length).Trim();
                    var test = await FindTestCaseFunc(principal, templateName);
                    if(test?.Description is not null)
                    {
                        templateText = await CompileMarkupAsync(principal, context, test.Description, FindTestCaseFunc);
                        continue;
                    }
                }

                if (processedLine.StartsWith(include))
                {
                    // The line starts with a @template directive
                    var templateName = processedLine.Substring(include.Length).Trim();
                    var test = await FindTestCaseFunc(principal, templateName);
                    if (test?.Description is not null)
                    {
                        var includedText = await CompileMarkupAsync(principal, context, test.Description, FindTestCaseFunc);
                        foreach (var includedLine in includedText.Split('\n'))
                        {
                            body.Append(includedLine.TrimEnd('\r'));
                            body.Append('\n');
                        }
                        continue;
                    }
                }
            }

            if(!isFirstLine)
            {
                body.Append('\n');
            }
            isFirstLine = false;
            body.Append(processedLine.TrimEnd('\r'));
        }

        var bodyText = body.ToString();
        if(templateText is not null)
        {
            return templateText.Replace("@Body", bodyText);
        }
        return bodyText;
    }
}
