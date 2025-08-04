using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using DotHttpTest.Models;
using DotHttpTest.Runner;
using DotHttpTest.Runner.Models;

using TestBucket.Contracts.Automation.Api;

namespace TestBucket.Runner.Runners.Http;

internal class DotHttpLogger : ITestPlanRunnerProgressHandler
{

    private readonly IScriptRunnerObserver _observer;
    private readonly string _workingDirectory;
    private readonly bool _saveMessages;
    private int _counter = 0;

    public DotHttpLogger(IScriptRunnerObserver observer, string workingDirectory, bool saveMessages)
    {
        _observer = observer;
        _workingDirectory = workingDirectory;
        _saveMessages = saveMessages;
    }
  
    public Task OnTestCompletedAsync(TestStatus state)
    {
        _observer.OnStdOut("- Test completed");
        return Task.CompletedTask;
    }

    public Task OnStageStartedAsync(TestPlanStage stage, TestStatus currentState)
    {
        _observer.OnStdOut($"- Starting stage {stage.Id}");
        return Task.CompletedTask;
    }

    public Task OnStageCompletedAsync(TestPlanStage stage, TestStatus currentState)
    {
        _observer.OnStdOut($"- Stage {stage.Id} complete");
        return Task.CompletedTask;
    }

    public async Task OnRequestCompletedAsync(DotHttpResponse response, TestStatus currentState)
    {
        var isSuccess = response.IsSuccessStatusCode;
        if (isSuccess)
        {
            _observer.OnStdOut($"- Request completed sucessfully: {response.StatusCode}");
        }
        else
        {
            _observer.OnStdOut($"- Request failed: {response.StatusCode}");
        }

        if (_saveMessages)
        {
            await SaveRequestAndResponseToFileAsync(response, currentState);
        }

        _counter++;
    }

    private async Task SaveRequestAndResponseToFileAsync(DotHttpResponse response, TestStatus currentState)
    {
        var request = response.Request;

        if (request?.Url is not null)
        {
            var responseHeaders = new HeadersCollectionDto();
            foreach (var header in response.Headers)
            {
                if (header.Values is not null && header.Name is not null)
                {
                    foreach (var value in header.Values)
                    {
                        responseHeaders.Add(new HeaderDto(header.Name, value));
                    }
                }
            }
            var responseDto = new HttpResponseMessageDto((int)response.StatusCode, response.ReasonPhrase ?? "", responseHeaders, response.ContentBytes);
            
            var method = request.Method?.ToString(currentState, null) ?? "GET";
            var url = request.Url.ToString(currentState, null);
            var requestHeaders = MapHeaders(currentState, request.Headers);
            byte[] body = [];
            if (request.Body is not null)
            {
                body = request.Body.ToByteArray(Encoding.UTF8, currentState, null);
            }

            var requestDto = new HttpRequestMessageDto(method, url, requestHeaders, body);
            requestDto.RequestName = request.RequestName;

            var testResult = new HttpMessageTestResult(requestDto, responseDto);
            foreach(var result in response.Results)
            {
                var resultDto = new HttpVerificationCheckResult(result.IsSuccess, result.Check.VerifierId, result.Check.PropertyId, result.Check.Operation.ToString(), result.Check.ExpectedValue, result.ActualValue, result.Error);
                testResult.Checks.Add(resultDto);
            }
            testResult.HttpRequestDuration = response.Metrics.HttpRequestDuration.Value;
            testResult.HttpRequestSending = response.Metrics.HttpRequestSending.Value;
            testResult.HttpResponseReceiving = response.Metrics.HttpRequestReceiving.Value;
            await File.WriteAllTextAsync(Path.Combine(_workingDirectory, $"http_{_counter.ToString("d8")}.http.result"), JsonSerializer.Serialize(testResult));
        }
    }

    private static HeadersCollectionDto MapHeaders(TestStatus currentState, ExpressionListHeaderCollection headerCollection)
    {
        HeadersCollectionDto headers = new();
        foreach (var line in headerCollection.Select(x => x.ToString(currentState, null)))
        {
            var p = line.IndexOf(':');
            var name = line[0..p].Trim();
            var value = line[(p+1)..].Trim();
            headers.Add(new HeaderDto(name,value));
        }
        return headers;
    }

    public Task OnRequestFailedAsync(DotHttpRequest request, Exception ex)
    {
        _observer.OnStdOut(ex.ToString());
        return Task.CompletedTask;
    }
}
