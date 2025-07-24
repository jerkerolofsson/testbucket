using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SemanticKernel;

namespace TestBucket.Domain.AI.Diagnostics.Filters;
public class FunctionTracingFilter : IFunctionInvocationFilter
{
    private readonly ActivitySource _activitySource;

    public FunctionTracingFilter(ActivitySource activitySource)
    {
        _activitySource = activitySource;
    }

    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        using var activity = _activitySource.StartActivity("SKFunction", ActivityKind.Internal);
        if (activity is not null)
        {
            activity.AddTag("function", context.Function.Name);
            activity.AddTag("plugin.name", context.Function.PluginName);
        }

        await next(context);
    }
}