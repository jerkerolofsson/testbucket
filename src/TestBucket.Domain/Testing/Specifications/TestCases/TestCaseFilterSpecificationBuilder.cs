using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Testing.Specifications.TestCases;

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

    public static List<FilterSpecification<TestCase>> From(SearchTestQuery query, IReadOnlyList<FieldDefinition> definitions)
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

        if (query.TestSuiteId is not null)
        {
            specifications.Add(new FilterTestCasesByTestSuite(query.TestSuiteId.Value));
        }
        if (query.Text is not null)
        {
            specifications.Add(new FilterTestCasesByText(query.Text));
        }
        if (query.CompareFolder)
        {
            specifications.Add(new FilterTestCasesByTestSuiteFolder(query.FolderId, query.Recurse));
        }
        if(query.Category is not null)
        {
            specifications.AddRange(GetTraitFilter(definitions, TraitType.TestCategory, query.Category));
        }
        if (query.Priority is not null)
        {
            specifications.AddRange(GetTraitFilter(definitions, TraitType.TestPriority, query.Priority));
        }

        return specifications;
    }
}
