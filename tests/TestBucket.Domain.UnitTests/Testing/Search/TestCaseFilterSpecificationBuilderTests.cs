using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Specifications.TestCases;
using TestBucket.Domain.Testing.TestCases.Search;

namespace TestBucket.Domain.UnitTests.Testing.Search;

/// <summary>
/// Tests related to building test case filter lists from a request entity
/// </summary>
[FunctionalTest]
[EnrichedTest]
[UnitTest]
[Component("Testing")]
[Feature("Review")]
[Feature("Search")]
public class TestCaseFilterSpecificationBuilderTests
{
    /// <summary>
    /// Verifies that TestCaseFilterSpecificationBuilder.From adds a filter that will filter on the ReviewAssignedTo property
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-002")]
    public void From_WithReviewAssignedTo_AddsCorrectFilter()
    {
        var filters = TestCaseFilterSpecificationBuilder.From(new SearchTestQuery { ReviewAssignedTo = "admin@admin.com" });

        Assert.Single(filters);
        Assert.Contains(filters, x => x is FilterTestCasesByReviewAssignedTo filter && filter.User == "admin@admin.com");
    }

    /// <summary>
    /// Verifies that TestCaseFilterSpecificationBuilder.From adds a filter that will filter on the State property
    /// </summary>
    [Fact]
    public void From_WitState_AddsCorrectFilter()
    {
        var filters = TestCaseFilterSpecificationBuilder.From(new SearchTestQuery { State = "hello" });

        Assert.Single(filters);
        Assert.Contains(filters, x => x is FilterTestCasesByState filter && filter.State == "hello");
    }
}
