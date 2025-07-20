using Mediator;
using TestBucket.Domain.AI.Embeddings;

namespace TestBucket.Domain.IntegrationTests.AI
{
    [IntegrationTest]
    [FunctionalTest]
    [EnrichedTest]
    [Component("AI")]
    [Feature("Classification")]
    [Feature("AI Runner")]
    public class GenerateEmbeddingTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that a non-empty embedding vector is returned
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GenerateEmbedding_GetsResult()
        {
            var principal = Fixture.App.SiteAdministrator;

            // Act
            var mediator = Fixture.Services.GetRequiredService<IMediator>();
            var response = await mediator.Send(new GenerateEmbeddingRequest(principal, Fixture.ProjectId, "passed"), TestContext.Current.CancellationToken);

            Assert.NotNull(response);
            Assert.NotNull(response.EmbeddingVector);
            Assert.NotEqual(0, response.EmbeddingVector.Value.Length);
        }

    }
}
