using Mediator;
using TestBucket.Domain.AI.Embeddings;

namespace TestBucket.Domain.IntegrationTests.AI
{
    /// <summary>
    /// Integration tests for generating AI embeddings for test cases.
    /// </summary>
    [IntegrationTest]
    [FunctionalTest]
    [EnrichedTest]
    [Component("AI")]
    [Feature("Classification")]
    [Feature("AI Runner")]
    public class GenerateEmbeddingTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateEmbeddingTests"/> class.
        /// </summary>
        /// <param name="Fixture">The project fixture used for integration testing.</param>
        public GenerateEmbeddingTests(ProjectFixture Fixture) : base(Fixture) { }

        /// <summary>
        /// Verifies that a non-empty embedding vector is returned by the AI embedding generator.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
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
