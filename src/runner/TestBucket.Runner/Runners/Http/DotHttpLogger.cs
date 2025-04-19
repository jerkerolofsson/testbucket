using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DotHttpTest.Models;
using DotHttpTest.Runner;
using DotHttpTest.Runner.Models;

namespace TestBucket.Runner.Runners.Http;

internal class DotHttpLogger : ITestPlanRunnerProgressHandler
{

    private readonly IScriptRunnerObserver _observer;

    public DotHttpLogger(IScriptRunnerObserver observer)
    {
        _observer = observer;
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

    public Task OnRequestCompletedAsync(DotHttpResponse response, TestStatus currentState)
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
        return Task.CompletedTask;
    }

    public Task OnRequestFailedAsync(DotHttpRequest request, Exception ex)
    {
        _observer.OnStdOut(ex.ToString());
        return Task.CompletedTask;
    }
}
