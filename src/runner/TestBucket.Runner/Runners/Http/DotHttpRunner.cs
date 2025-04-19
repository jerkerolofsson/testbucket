
using System.Globalization;

using DotHttpTest.Runner;
using DotHttpTest.Runner.Models;
using DotHttpTest.Verification.Models;

using TestBucket.Contracts.Testing.Models;
using TestBucket.Formats.Dtos;
using TestBucket.Formats.JUnit;

namespace TestBucket.Runner.Runners.Http;

public class DotHttpRunner : IScriptRunner
{
    public async Task<ScriptResult> RunAsync(Script script, IScriptRunnerObserver observer, CancellationToken cancellationToken)
    {
        var logger = new DotHttpLogger(observer);
        var runnerOptions = new TestPlanRunnerOptionsBuilder()
            .ConfigureClientOptions((optionsBuilder) =>
            {
                optionsBuilder.UseDefaultVariableProvider();
                optionsBuilder.UseJsonVariableProvider();
                optionsBuilder.UseDynamicVariableProvider();
                optionsBuilder.UseEnvironmentVariablesProviders();

                // Add custom variables
                if (script.EnvironmentVariables is not null)
                {
                    foreach (var variable in script.EnvironmentVariables)
                    {
                        optionsBuilder.WithVariable(variable.Key, variable.Value);
                    }
                }
            })
            .ConfigureTestPlan((planBuilder, clientOptions) =>
            {
                planBuilder.LoadHttpText(script.Text, (configure) => { }, clientOptions);
            })
            .AddCallback(logger);

        var runner = runnerOptions.Build();

        var run = new TestRunDto();
        run.StartedTime = DateTimeOffset.UtcNow;

        var testStatus = await runner.RunAsync(cancellationToken);
        run.EndedTime = DateTimeOffset.UtcNow;

        // Format the result as JUnit XML and return it

        TestRunDto testRun = CreateTestRun(testStatus, run, logger);

        var serializer = new JUnitSerializer();
        var xml = serializer.Serialize(testRun);

        var resultXmlPath = Path.Combine(script.WorkingDirectory, "http-results.junit.xml");
        await File.WriteAllTextAsync(resultXmlPath, xml, cancellationToken);

        return new ScriptResult
        {
            ExitCode = 0,
            Success = true,
        };
    }


    private TestRunDto CreateTestRun(TestStatus status, TestRunDto run, DotHttpLogger logger)
    {
        var report = status.TestReport;
        var plan = status.TestReport.TestPlan;
        run.Name = plan.Name + $" ()";
        run.ExternalId = plan.Name;

        var suite = new TestSuiteRunDto();
        suite.Name = run.Name;
        suite.StartedTime = run.StartedTime;
        suite.EndedTime = run.EndedTime;
        suite.SystemOut = logger.ToString();

        foreach (var stage in report.Stages)
        {
            var test = new TestCaseRunDto
            {
                Name = stage.PlannedStage?.Attributes.Name,
                ExternalId = stage.PlannedStage?.Attributes.Name,
                Result = TestResult.Passed
            };
            var failedChecks = new List<VerificationCheckResult>();
            foreach (var result in stage.Results)
            {
                failedChecks.AddRange(result.FailedChecks);
            }

            var messageBuilder = new StringBuilder();

            messageBuilder.AppendLine($"# Stage {stage.PlannedStage?.Attributes.Name}");
            messageBuilder.AppendLine($"{stage.PassCount} passed, {stage.FailCount} failed");
            messageBuilder.AppendLine();

            if (stage.Results.Count > 0)
            {
                foreach (var result in stage.Results.Take(1))
                {
                    var request = result.Request;
                    if (request is null)
                    {
                        continue;
                    }
                    messageBuilder.AppendLine($"# {request.RequestName}");
                    messageBuilder.AppendLine($"| Attribute | Value     |");
                    messageBuilder.AppendLine($"| --------- | --------- |");
                    messageBuilder.AppendLine($"| Method    | {request.Method?.ToString(status)} |");
                    messageBuilder.AppendLine($"| URL       | {request.Url?.ToString(status)} |");
                    if (status.PreviousResponse is not null)
                    {
                        var response = status.PreviousResponse;
                        var statusCode = (int)response.StatusCode;
                        messageBuilder.AppendLine($"| Status Code   | {statusCode} |");
                        messageBuilder.AppendLine($"| Reason Phrase | {response.ReasonPhrase} |");
                    }

                    int requests = stage.Results.Count;
                    var duration = stage.Results.Sum(x => x.HttpRequestDuration.Value);
                    var sentBytes = stage.Results.Sum(x => x.HttpBytesSent.Value);
                    var receivedBytes = stage.Results.Sum(x => x.HttpBytesReceived.Value);
                    var rps = stage.Results.Count / duration;
                    messageBuilder.AppendLine($"| Requests   | {requests} |");
                    messageBuilder.AppendLine($"| Duration   | {TimeSpan.FromSeconds(duration)} |");
                    messageBuilder.AppendLine($"| Requests/s | {rps.ToString(CultureInfo.InvariantCulture)} |");
                    messageBuilder.AppendLine($"| Received bytes   | {receivedBytes} |");
                    messageBuilder.AppendLine($"| Sent bytes   | {sentBytes} |");

                    //messageBuilder.AppendLine($"| Bytes Sent | {result.HttpBytesSent.Value} |");
                    //messageBuilder.AppendLine($"| Bytes Received | {result.HttpBytesReceived.Value} |");
                    //messageBuilder.AppendLine($"| Duration  | {result.HttpRequestDuration.Value}{result.HttpRequestDuration.Unit} |");
                    //messageBuilder.AppendLine($"| Sending   | {result.HttpRequestSending.Value}{result.HttpRequestSending.Unit} |");
                    //messageBuilder.AppendLine($"| Receiving | {result.HttpRequestReceiving.Value}{result.HttpRequestReceiving.Unit} |");

                    messageBuilder.AppendLine("## Request");
                    messageBuilder.AppendLine("```http");
                    messageBuilder.AppendLine($"{request.Method?.ToString(status)} {request.Url?.ToString(status)} {request.Version?.ToString(status)} ");
                    foreach (var header in request.Headers)
                    {
                        messageBuilder.AppendLine($"{header?.ToString(status)}");
                    }
                    messageBuilder.AppendLine("```");

                    if (status.PreviousResponse is not null)
                    {
                        var response = status.PreviousResponse;
                        var statusCode = (int)response.StatusCode;
                        messageBuilder.AppendLine("## Response");
                        messageBuilder.AppendLine("```http");
                        messageBuilder.AppendLine($"{statusCode} {response.ReasonPhrase}");
                        foreach (var header in response.Headers)
                        {
                            if (header.Values is not null)
                            {
                                foreach (var value in header.Values)
                                {
                                    messageBuilder.AppendLine($"{header.Name}: {value}");
                                }
                            }
                        }
                        messageBuilder.AppendLine();

                        if (response.ContentBytes is not null)
                        {
                            if (response.ContentBytes.Length < 10 * 1024)
                            {
                                if (response.Content.Headers.ContentType?.MediaType?.StartsWith("text") == true ||
                                    response.Content.Headers.ContentType?.MediaType?.StartsWith("application/json") == true ||
                                    response.Content.Headers.ContentType?.MediaType?.StartsWith("application/xml") == true)
                                {
                                    messageBuilder.AppendLine(Encoding.UTF8.GetString(response.ContentBytes));
                                }
                                else
                                {
                                    messageBuilder.AppendLine($"...{response.ContentBytes.Length} bytes payload...");
                                }
                            }
                            else
                            {
                                messageBuilder.AppendLine($"...{response.ContentBytes.Length} bytes payload...");
                            }
                        }
                        messageBuilder.AppendLine("```");

                    }
                }

            }

            if (failedChecks.Count > 0)
            {
                test.Result = TestResult.Failed;
                messageBuilder.AppendLine();
                messageBuilder.AppendLine("## Failed Checks:");

                messageBuilder.AppendLine($"| URL | Expected  | Actual | Error     |");
                messageBuilder.AppendLine($"| --- | --------- | --------- | --------- |");
                foreach (var check in failedChecks)
                {
                    messageBuilder.AppendLine($"| {check.Request.Url} | {check.Check?.ExpectedValue} | {check.ActualValue} | {check.Error} |");
                    messageBuilder.AppendLine(check.Error);
                }
            }
            test.Message = messageBuilder.ToString();

            suite.Tests.Add(test);
        }

        run.Suites.Add(suite);

        return run;
    }
}
