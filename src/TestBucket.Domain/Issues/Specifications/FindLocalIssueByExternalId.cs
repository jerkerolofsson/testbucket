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
internal class FindLocalIssueByExternalDisplayId : FilterSpecification<LocalIssue>
{
    private readonly string _id;

    public FindLocalIssueByExternalDisplayId(string id)
    {
        _id = id;
    }

    protected override Expression<Func<LocalIssue, bool>> GetExpression()
    {
        return x => x.ExternalDisplayId == _id;
    }
}
