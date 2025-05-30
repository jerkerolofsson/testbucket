using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Fields;

namespace TestBucket.Domain.Testing.TestRuns.Events;
public record class SetCommitForRunRequest(ClaimsPrincipal Principal, long TestRunId, string CommitSha) : IRequest;

public class SetCommitForRunHandler : IRequestHandler<SetCommitForRunRequest>
{
    private readonly ITestRunManager _manager;
    private readonly IFieldManager _fieldManager;

    public SetCommitForRunHandler(ITestRunManager manager, IFieldManager fieldManager)
    {
        _manager = manager;
        _fieldManager = fieldManager;
    }

    public async ValueTask<Unit> Handle(SetCommitForRunRequest request, CancellationToken cancellationToken)
    {
        var run = await _manager.GetTestRunByIdAsync(request.Principal, request.TestRunId);
        if(run?.TestProjectId is not null)
        {
            await _fieldManager.SetTestRunFieldAsync(request.Principal, run.TestProjectId.Value, run.Id, Traits.Core.TraitType.Commit, request.CommitSha);
        }

        return Unit.Value;
    }
}
