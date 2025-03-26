using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Fields;
using TestBucket.Domain.Search.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Tenants.Models;
using TestBucket.Domain.Testing;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCases;

namespace TestBucket.Domain.Search
{
    public class UnifiedSearchManager : IUnifiedSearchManager
    {
        private readonly ITestCaseRepository _testCaseRepo;
        private readonly IFieldDefinitionManager _fieldDefinitionManager;

        public UnifiedSearchManager(ITestCaseRepository testCaseRepo, IFieldDefinitionManager fieldDefinitionManager)
        {
            _testCaseRepo = testCaseRepo;
            _fieldDefinitionManager = fieldDefinitionManager;
        }

        public async Task<List<SearchResult>> SearchAsync(ClaimsPrincipal principal, TestProject? testProject, string text, CancellationToken cancellationToken)
        {
            var tenantId = principal.GetTenantIdOrThrow();

            var result = new List<SearchResult>();

            var query = new SearchTestQuery { Text = text, Offset = 0, Count = 5, ProjectId = testProject?.Id, CompareFolder = false };

            IReadOnlyList<FieldDefinition> fields = [];
            var filters = TestCaseFilterSpecificationBuilder.From(query, fields);

            filters = [new FilterByTenant<TestCase>(tenantId), .. filters];
            var tests = await _testCaseRepo.SearchTestCasesAsync(query.Offset, query.Count, filters);
            foreach(var test in tests.Items)
            {
                result.Add(new SearchResult(test));
            }

            return result;
        }
    }
}
