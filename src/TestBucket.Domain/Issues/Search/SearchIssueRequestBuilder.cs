using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Issues.Specifications;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Specifications.TestCases;

namespace TestBucket.Domain.Issues.Search;
internal class SearchIssueRequestBuilder
{
    public static IReadOnlyList<FilterSpecification<LocalIssue>> Build(SearchIssueQuery request)
    {
        var filters = ProjectEntityFilterSpecificationBuilder.From<LocalIssue>(request);

        if (!string.IsNullOrEmpty(request.Text))
        {
            filters.Add(new FindLocalIssueByText(request.Text));
        }
        if (!string.IsNullOrEmpty(request.ExternalSystemName))
        {
            filters.Add(new FindLocalIssueByExternalSystemName(request.ExternalSystemName));
        }
        if (request.ExternalSystemId != null)
        {
            filters.Add(new FindLocalIssueByExternalSystemId(request.ExternalSystemId.Value));
        }
        if (request.Type != null)
        {
            filters.Add(new FindLocalIssueByType(request.Type));
        }
        if (request.State != null)
        {
            filters.Add(new FindLocalIssueByState(request.State));
        }
        if (request.MappedState != null)
        {
            filters.Add(new FindLocalIssueByMappedState(request.MappedState.Value));
        }
       
        if (request.Fields.Count > 0)
        {
            foreach (var field in request.Fields)
            {
                if (!string.IsNullOrEmpty(field.StringValue))
                {
                    filters.Add(new FilterLocalIssueByStringField(field.FilterDefinitionId, field.StringValue));
                }
            }
        }
        return filters;
    }

}