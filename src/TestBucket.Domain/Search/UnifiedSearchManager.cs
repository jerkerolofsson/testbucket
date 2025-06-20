using TestBucket.Domain.Fields;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Requirements.Specifications;
using TestBucket.Domain.Search.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestCases.Search;

namespace TestBucket.Domain.Search
{
    public class UnifiedSearchManager : IUnifiedSearchManager
    {
        private readonly ITestCaseRepository _testCaseRepo;
        private readonly IFieldDefinitionManager _fieldDefinitionManager;
        private readonly IRequirementRepository _requirementRepo;
        private readonly ISettingsManager _settingsManager;

        public UnifiedSearchManager(ITestCaseRepository testCaseRepo, IFieldDefinitionManager fieldDefinitionManager, IRequirementRepository requirementRepo, ISettingsManager settingsManager)
        {
            _testCaseRepo = testCaseRepo;
            _fieldDefinitionManager = fieldDefinitionManager;
            _requirementRepo = requirementRepo;
            _settingsManager = settingsManager;
        }

        /// <summary>
        /// Searches for various items
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="testProject"></param>
        /// <param name="text"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<SearchResult>> SearchAsync(ClaimsPrincipal principal, TestProject? testProject, string text, CancellationToken cancellationToken)
        {
            var tenantId = principal.GetTenantIdOrThrow();

            var result = new List<SearchResult>();

            SearchForSettingLinks(principal,testProject,text,result);
            await SearchForTestsAsync(testProject, text, tenantId, result);
            await SearchForRequirementsAsync(testProject, text, tenantId, result);

            return result;
        }

        private void SearchForSettingLinks(ClaimsPrincipal principal, TestProject? testProject, string text, List<SearchResult> result)
        {
            var context = new SettingContext { Principal = principal, ProjectId = testProject?.Id };
            var links = _settingsManager.SearchLinks(context, text);
            foreach(var link in links)
            {
                result.Add(new SearchResult(link));
            }
        }

        private async Task SearchForRequirementsAsync(TestProject? testProject, string text, string tenantId, List<SearchResult> result)
        {
            var query = new SearchRequirementQuery { Text = text, Offset = 0, Count = 5, ProjectId = testProject?.Id, CompareFolder = false };

            IReadOnlyList<FieldDefinition> fields = [];
            var filters = RequirementSpecificationBuilder.From(query);

            filters = [new FilterByTenant<Requirement>(tenantId), .. filters];
            var requirements = await _requirementRepo.SearchRequirementsAsync(filters, query.Offset, query.Count);
            foreach (var requirement in requirements.Items)
            {
                result.Add(new SearchResult(requirement));
            }
        }
        private async Task SearchForTestsAsync(TestProject? testProject, string text, string tenantId, List<SearchResult> result)
        {
            var query = new SearchTestQuery { Text = text, Offset = 0, Count = 5, ProjectId = testProject?.Id, CompareFolder = false };

            var filters = TestCaseFilterSpecificationBuilder.From(query);

            filters = [new FilterByTenant<TestCase>(tenantId), .. filters];
            var tests = await _testCaseRepo.SearchTestCasesAsync(query.Offset, query.Count, filters);
            foreach (var test in tests.Items)
            {
                result.Add(new SearchResult(test));
            }
        }
    }
}
