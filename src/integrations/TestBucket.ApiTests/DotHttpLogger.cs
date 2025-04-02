using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DotHttpTest.Models;
using DotHttpTest.Runner;
using DotHttpTest.Runner.Models;

namespace TestBucket.ApiTests
{
    internal class DotHttpLogger : ITestPlanRunnerProgressHandler
    {

        private readonly StringBuilder _stringBuilder = new();
        public DotHttpLogger()
        {
        }
        public override string ToString()
        {
            return _stringBuilder.ToString();
        }

        public Task OnTestCompletedAsync(TestStatus state)
        {
            _stringBuilder.AppendLine("- Test completed");
            return Task.CompletedTask;
        }

        public Task OnStageStartedAsync(TestPlanStage stage, TestStatus currentState)
        {
            _stringBuilder.AppendLine($"- Starting stage {stage.Id}");
            return Task.CompletedTask;
        }

        public Task OnStageCompletedAsync(TestPlanStage stage, TestStatus currentState)
        {
            _stringBuilder.AppendLine($"- Stage {stage.Id} complete");
            return Task.CompletedTask;
        }

        public Task OnRequestCompletedAsync(DotHttpResponse response, TestStatus currentState)
        {
            var isSuccess = response.IsSuccessStatusCode;
            if (isSuccess)
            {
                _stringBuilder.AppendLine($"- Request completed sucessfully: {response.StatusCode}");
            }
            else
            {
                _stringBuilder.AppendLine($"- Request failed: {response.StatusCode}");
            }
            return Task.CompletedTask;
        }

        public Task OnRequestFailedAsync(DotHttpRequest request, Exception ex)
        {
            _stringBuilder.AppendLine(ex.ToString());
            return Task.CompletedTask;
        }
    }
}
