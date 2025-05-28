using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Issues.Models;

namespace TestBucket.Domain.Issues.Requests;
public record class CloseIssueRequest(ClaimsPrincipal Principal, LocalIssue Issue) : IRequest
{
}

public class CloseIssueRequestHandler : IRequestHandler<CloseIssueRequest>
{
    private readonly IIssueManager _issueManager;

    public CloseIssueRequestHandler(IIssueManager issueManager)
    {
        _issueManager = issueManager;
    }

    public async ValueTask<Unit> Handle(CloseIssueRequest request, CancellationToken cancellationToken)
    {
        var issue = request.Issue;
        issue.MappedState = MappedIssueState.Closed;
        issue.State = IssueStates.Closed;
        await _issueManager.UpdateLocalIssueAsync(request.Principal, issue);

        return Unit.Value;
    }
}
