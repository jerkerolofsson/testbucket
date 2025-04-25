using TestBucket.Contracts.Testing.Models;

namespace TestBucket.IntegrationTests.Features.Duplication;

[Feature("Duplication 1.0")]
[EnrichedTest]
[IntegrationTest]
public class DuplicateTestCaseTests(TestBucketApp App)
{
    [Fact]
    public async Task DuplicateTestCaseAsync()
    {
        await App.Client.TestRepository.AddTestAsync(CreateTestCase());
    }

    private TestCaseDto CreateTestCase()
    {
        return new TestCaseDto { Name = "name1", Description = "description1", TenantId = App.Tenant };
    }
}
