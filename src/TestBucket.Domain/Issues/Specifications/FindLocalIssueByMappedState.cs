using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Issues.Specifications;

/// <summary>
/// Finds issues with a specific state
/// </summary>
internal class FindLocalIssueByMappedState : FilterSpecification<LocalIssue>
{
    private readonly MappedIssueState _state;

    public FindLocalIssueByMappedState(MappedIssueState state)
    {
        _state = state;
    }

    protected override Expression<Func<LocalIssue, bool>> GetExpression()
    {
        return x => x.MappedState == _state;
    }
}
