using Mediator;

using TestBucket.Domain.Environments;
using TestBucket.Domain.Environments.Models;
using TestBucket.Domain.Projects;
using TestBucket.Domain.TestAccounts.Allocation;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestSuites;
using TestBucket.Domain.TestResources.Allocation;

namespace TestBucket.Domain.Testing.Compiler;
public class TestExecutionContextBuilder
{
    private readonly ITestCompiler _compiler;
    private readonly IProjectManager _projectManager;
    private readonly ITestRunManager _testRunManager;
    private readonly ITestEnvironmentManager _testEnvironmentManager;
    private readonly ITestSuiteManager _testSuiteManager;
    private readonly IMediator _mediator;

    public TestExecutionContextBuilder(ITestCompiler compiler, IProjectManager projectManager, ITestRunManager testRunManager, ITestEnvironmentManager testEnvironmentManager, ITestSuiteManager testSuiteManager, IMediator mediator)
    {
        _compiler = compiler;
        _projectManager = projectManager;
        _testRunManager = testRunManager;
        _testEnvironmentManager = testEnvironmentManager;
        _testSuiteManager = testSuiteManager;
        _mediator = mediator;
    }

    public async Task<TestExecutionContext?> BuildAsync(ClaimsPrincipal principal, CompilationOptions options, List<CompilerError> errors, CancellationToken cancellationToken)
    {
        var testCase = options.TestCase;
        if (testCase.TestProjectId is not null && testCase.TenantId is not null)
        {
            if (testCase.TeamId is null)
            {
                var project = await _projectManager.GetTestProjectByIdAsync(principal, testCase.TestProjectId.Value);
                testCase.TeamId = project?.TeamId;
            }
            if (testCase.TeamId is null)
            {
                return null;
            }

            TestRun? testRun;
            TestEnvironment? testEnvironment = null;
            if (options.TestRunId is not null)
            {
                testRun = await _testRunManager.GetTestRunByIdAsync(principal, options.TestRunId.Value);
                if (testRun?.TestEnvironmentId is not null)
                {
                    testEnvironment = await _testEnvironmentManager.GetTestEnvironmentByIdAsync(principal, testRun.TestEnvironmentId.Value);
                }
            }
            if (testEnvironment is null)
            {
                var defaultEnvironment = await _testEnvironmentManager.GetDefaultTestEnvironmentAsync(principal, testCase.TestProjectId.Value);
                testEnvironment = defaultEnvironment;
            }

            var context = new TestExecutionContext()
            {
                Guid = options.ContextGuid,
                TenantId = testCase.TenantId,
                TestCaseId = testCase.Id,
                TestSuiteId = testCase.TestSuiteId,
                ProjectId = testCase.TestProjectId.Value,
                TestRunId = options.TestRunId,
                TeamId = testCase.TeamId.Value,
                TestEnvironmentId = testEnvironment?.Id,
                Dependencies = []
            };

            if (string.IsNullOrWhiteSpace(options.Text))
            {
                context.CompiledText = options.Text;
                return context;
            }

            if (options.AllocateResources)
            {
                if (testCase.Dependencies is not null)
                {
                    context.Dependencies = [.. testCase.Dependencies];
                }

                var suite = await _testSuiteManager.GetTestSuiteByIdAsync(principal, testCase.TestSuiteId);
                if (suite?.Dependencies is not null)
                {
                    context.Dependencies.AddRange(suite.Dependencies);
                }
            }

            await _compiler.ResolveVariablesAsync(principal, context, cancellationToken);
            CompilerError[] resolveErrors = [..context.CompilerErrors];

            var result = await _compiler.CompileAsync(principal, context, options.Text);

            errors.Clear();
            errors.AddRange(resolveErrors);
            errors.AddRange(context.CompilerErrors);

            if (options.ReleaseResourceDirectly)
            {
                await ReleaseResourcesAsync(context.Guid, testCase.TenantId);
            }

            context.CompiledText = result;
            return context;
        }
        return null;
    }

    public async Task ReleaseResourcesAsync(TestExecutionContext context)
    {
        await ReleaseResourcesAsync(context.Guid, context.TenantId ?? "");
    }

    private async ValueTask ReleaseResourcesAsync(string guid, string tenantId)
    {
        await _mediator.Send(new ReleaseResourcesRequest(guid, tenantId));
        await _mediator.Send(new ReleaseAccountsRequest(guid, tenantId));
    }
}
