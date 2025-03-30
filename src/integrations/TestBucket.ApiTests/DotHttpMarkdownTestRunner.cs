
using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Testing.Models;
using DotHttpTest;
using DotHttpTest.Runner;
using DotHttpTest.Builders;
using DotHttpTest.Reporting.JUnitXml;
using DotHttpTest.Models;
using DotHttpTest.Runner.Models;

namespace TestBucket.ApiTests;

public class DotHttpMarkdownTestRunner : IMarkdownTestRunner
{
    public string Language => "http";

    public async Task<TestRunnerResult> RunAsync(TestExecutionContext context, string code, CancellationToken cancellationToken)
    {
        var optionsBuilder = new ClientOptionsBuilder()
        {

        };
        optionsBuilder.UseDefaultVariableProvider();
        optionsBuilder.UseJsonVariableProvider();
        optionsBuilder.UseDynamicVariableProvider();
        optionsBuilder.UseEnvironmentVariablesProviders();

        // Add custom variables
        if (context.Variables is not null)
        {
            foreach (var variable in context.Variables)
            {
                optionsBuilder.WithVariable(variable.Key, variable.Value);
            }
        }
        var options = optionsBuilder.Build();

        //using var client = new DotHttpClient();
        //var requests = DotHttpRequest.Parse(code, options);

        //TestPlan plan = new TestPlan();
        //TestReport report = new(plan);
        //TestStatus status = new(report);
        //foreach(var request in requests)
        //{
        //    var response = await client.SendAsync(request, status, cancellationToken);
        //}

        var plan = new TestPlanBuilder()
            .LoadHttpText(code, (configure) =>
            {
            }, options)
            .Build();

        var runner = new TestPlanRunner(plan, new TestPlanRunnerOptions());

        var testStatus = await runner.RunAsync(cancellationToken);

        // Format the result as JUnit XML and return it
        var xml = new JUnitXmlWriter("").GetXml(testStatus);
        return new TestRunnerResult { Format = Formats.TestResultFormat.JUnitXml, Result = xml };
    }
}
