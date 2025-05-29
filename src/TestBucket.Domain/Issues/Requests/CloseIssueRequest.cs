using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Issues.Models;

namespace TestBucket.Domain.Issues.Requests;
public record class CloseIssueRequest(ClaimsPrincipal Principal, LocalIssue Issue, string? CommitSha = null) : IRequest
{
}

public class CloseIssueRequestHandler : IRequestHandler<CloseIssueRequest>
{
    private readonly IIssueManager _issueManager;
    private readonly IFieldManager _fieldManager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;

    public CloseIssueRequestHandler(IIssueManager issueManager, IFieldManager fieldManager, IFieldDefinitionManager fieldDefinitionManager)
    {
        _issueManager = issueManager;
        _fieldManager = fieldManager;
        _fieldDefinitionManager = fieldDefinitionManager;
    }

    public async ValueTask<Unit> Handle(CloseIssueRequest request, CancellationToken cancellationToken)
    {
        var issue = request.Issue;
        issue.MappedState = MappedIssueState.Closed;
        issue.State = IssueStates.Closed;
        await _issueManager.UpdateLocalIssueAsync(request.Principal, issue);

        if(!string.IsNullOrEmpty(request.CommitSha) && issue.TestProjectId is not null)
        {
            await _fieldManager.SetIssueFieldAsync(request.Principal, issue.TestProjectId.Value, issue.Id, Traits.Core.TraitType.Commit, request.CommitSha);
        }

        return Unit.Value;
    }
}
