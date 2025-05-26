using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

namespace TestBucket.Domain.Testing.TestRuns.Events;
public record class CloseRunRequest(ClaimsPrincipal Principal, long TestRunId) : IRequest;

public class CloseRunHandler : IRequestHandler<CloseRunRequest>
{
    private readonly ITestRunManager _manager;

    public CloseRunHandler(ITestRunManager manager)
    {
        _manager = manager;
    }

    public async ValueTask<Unit> Handle(CloseRunRequest request, CancellationToken cancellationToken)
    {
        var run = await _manager.GetTestRunByIdAsync(request.Principal, request.TestRunId);
        if(run is not null)
        {
            run.Open = false;
            await _manager.SaveTestRunAsync(request.Principal, run);
        }

        return Unit.Value;
    }
}
