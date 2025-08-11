using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCases;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Testing.TestCases.Search;

public class TestCaseFilterSpecificationBuilder
{
    public static List<FilterSpecification<TestCase>> GetTraitFilter(IReadOnlyList<FieldDefinition> definitions, TraitType trait, string value)
    {
        var definition = definitions.FirstOrDefault(x => x.TraitType == trait);
        if(definition is not null)
        {
            return [new FilterTestCasesByStringField(definition.Id, value)];
        }

        return [];
    }

    public static List<FilterSpecification<TestCase>> From(SearchTestQuery query)
    {
        var specifications = ProjectEntityFilterSpecificationBuilder.From<TestCase>(query);

        if(query.Fields is not null)
        {
            foreach(var fieldFilter in query.Fields)
            {
                if(!string.IsNullOrEmpty(fieldFilter.StringValue))
                {
                    specifications.Add(new FilterTestCasesByStringField(fieldFilter.FilterDefinitionId, fieldFilter.StringValue));
                }
            }
        }

        if(query.ExternalDisplayId is not null)
        {
            specifications.Add(new FilterTestCasesByExternalDisplayId(query.ExternalDisplayId));
        }

        if (query.ExcludeAutomated == true)
        {
            specifications.Add(new FilterTestCasesExcludeAutomated());
        }
        if (query.State is not null)
        {
            specifications.Add(new FilterTestCasesByState(query.State));
        }

        if (query.TestSuiteId is not null)
        {
            specifications.Add(new FilterTestCasesByTestSuite(query.TestSuiteId.Value));
        }
        if (query.TestExecutionType is not null)
        {
            specifications.Add(new FilterTestCasesByExecutionType(query.TestExecutionType.Value));
        }
        if (query.ReviewAssignedTo is not null)
        {
            specifications.Add(new FilterTestCasesByReviewAssignedTo(query.ReviewAssignedTo));
        }
        if (query.Text is not null)
        {
            specifications.Add(new FilterTestCasesByText(query.Text));
        }
        if (query.CompareFolder == true)
        {
            specifications.Add(new FilterTestCasesByTestSuiteFolder(query.FolderId, query.Recurse));
        }
       
        return specifications;
    }
}
