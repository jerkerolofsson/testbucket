using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SemanticKernel;

namespace TestBucket.Domain.AI.Diagnostics.Filters;
public class PromptTracingFilter : IPromptRenderFilter
{
    private readonly ActivitySource _activitySource;

    public PromptTracingFilter(ActivitySource activitySource)
    {
        _activitySource = activitySource;
    }


    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        using var activity = _activitySource.StartActivity("SKPrompt", ActivityKind.Internal);
        if (activity is not null)
        {
            activity.AddTag("prompt", context.RenderedPrompt);
        }

        await next(context);
    }
}