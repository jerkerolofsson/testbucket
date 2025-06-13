using TestBucket.AI.Xunit;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Formats.Dtos;

namespace TestBucket.IntegrationTests.TestCases
{
    public class TestCaseMcpTests(McpFixture McpFixture) : IClassFixture<McpFixture>
    {
        [Fact]
        public async Task GetNumberOfTestCasesAsync()
        {
            // Arrange
            var testCase = new TestCaseDto
            {
                TenantId = McpFixture.TestBucket.Tenant,
                TestCaseName = "Test Case 1",
                ExecutionType = TestExecutionType.Automated,
                Traits = new(),
                ProjectSlug = McpFixture.ProjectSlug,
                TeamSlug = McpFixture.TeamSlug,
                TestSuiteSlug = McpFixture.TestSuiteSlug,
            };
            testCase.Traits.Traits.Add(new TestTrait(Traits.Core.TraitType.TestCategory, "Unit"));
            await McpFixture.TestBucket.Client.TestRepository.AddTestAsync(testCase);
            

            // Act
            var response = await McpFixture.McpClient.TestCallToolAsync("get-number-of-testcases-by-category");

            // Assert
            response.ShouldBeSuccess();
            response.ShouldHaveContent();
        }
    }
}
