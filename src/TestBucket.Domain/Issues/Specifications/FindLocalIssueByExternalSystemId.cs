﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Issues.Specifications;

/// <summary>
/// Finds issues created from a specific extension configuration
/// </summary>
internal class FindLocalIssueByExternalSystemId : FilterSpecification<LocalIssue>
{
    private readonly long _id;

    public FindLocalIssueByExternalSystemId(long id)
    {
        _id = id;
    }

    protected override Expression<Func<LocalIssue, bool>> GetExpression()
    {
        return x => (x.ExternalSystemId == _id);
    }
}
