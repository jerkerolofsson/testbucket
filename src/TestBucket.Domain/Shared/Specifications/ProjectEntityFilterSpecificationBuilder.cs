using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCases;

public class ProjectEntityFilterSpecificationBuilder
{
    public static List<FilterSpecification<T>> From<T>(SearchQuery query) where T : ProjectEntity
    {
        var specifications = new List<FilterSpecification<T>>();

        if (query.TeamId is not null)
        {
            specifications.Add(new FilterByTeam<T>(query.TeamId.Value));
        }
        if (query.ProjectId is not null)
        {
            specifications.Add(new FilterByProject<T>(query.ProjectId.Value));
        }
        if (query.CreatedFrom is not null || query.CreatedUntil is not null)
        {
            specifications.Add(new FilterByCreated<T>(query.CreatedFrom, query.CreatedUntil));
        }


        return specifications;
    }
}
