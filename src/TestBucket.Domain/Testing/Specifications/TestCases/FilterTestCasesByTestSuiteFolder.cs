using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCases;

/// <summary>
/// Filters test cases that are direct children in a folder or if includeAllDescendants = true, any descendant
/// </summary>
public class FilterTestCasesByTestSuiteFolder : FilterSpecification<TestCase>
{
    private readonly long? _id;
    private readonly bool _includeAllDescendants;

    public FilterTestCasesByTestSuiteFolder(long? id, bool includeAllDescendants)
    {
        _id = id;
        _includeAllDescendants = includeAllDescendants;
    }

    protected override Expression<Func<TestCase, bool>> GetExpression()
    {
        if (_includeAllDescendants)
        {
            if (_id is null)
            {
                return x => true;
            }

            return x => x.PathIds != null && x.PathIds.Contains(_id.Value);
        }
        else
        {
            return x => x.TestSuiteFolderId == _id;
        }
    }
}
