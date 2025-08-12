using Mediator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.AI.Embeddings;
using TestBucket.Embeddings;

namespace TestBucket.Domain.IntegrationTests.AI
{
    /// <summary>
    /// Tests that verifies the cosine similarity calculation between embedding vectors
    /// 
    /// This is used by 
    /// - Classification 
    /// - AI Runner to evaluate if the test passed or failed based on LLM output
    /// 
    /// If you ever end up debugging this due to the long time it takes to run, note that the first time an embedding is generated, it may take a long time due to the model being downloaded. 
    /// Subsequent runs will be much faster.
    /// </summary>
    /// <param name="Fixture"></param>
    [IntegrationTest]
    [FunctionalTest]
    [EnrichedTest]
    [Component("AI")]
    [Feature("Classification")]
    [Feature("AI Runner")]
    public class CosineSimilarityTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that embedding1 is more similar to embedding2 than to embedding3
        /// </summary>
        /// <returns></returns>
        [InlineData("good",             "good",     "bad")]
        [InlineData("good",             "ok",       "bad")]
        [InlineData("passed",           "ok",       "failed")]
        [InlineData("passed",           "passing",  "failed")]
        [InlineData("failing",          "failed",   "passed")]
        [InlineData("error",            "failed",   "passed")]
        [InlineData("police", "cop", "rain")]
        [InlineData("open player view", "enter player view", "open a bottle of water")]
        [Theory]
        public async Task CalculateCosineSimilarity_WithTwoEmbeddings_SimilarityAsExpected(string embedding1, string embedding2, string embedding3)
        {
            var principal = Fixture.App.SiteAdministrator;

            // Act
            var mediator = Fixture.Services.GetRequiredService<IMediator>();
            var embedding1Vector = (await mediator.Send(new GenerateEmbeddingRequest(principal, Fixture.ProjectId, embedding1), TestContext.Current.CancellationToken)).EmbeddingVector;
            var embedding2Vector = (await mediator.Send(new GenerateEmbeddingRequest(principal, Fixture.ProjectId, embedding2), TestContext.Current.CancellationToken)).EmbeddingVector;
            var embedding3Vector = (await mediator.Send(new GenerateEmbeddingRequest(principal, Fixture.ProjectId, embedding3), TestContext.Current.CancellationToken)).EmbeddingVector;

            Assert.NotNull(embedding1Vector);
            Assert.NotNull(embedding2Vector);
            Assert.NotNull(embedding3Vector);

            var similarity1And2 = CosineSimilarity.Calculate(embedding1Vector.Value, embedding2Vector.Value);
            var similarity2And3 = CosineSimilarity.Calculate(embedding2Vector.Value, embedding3Vector.Value);

            Assert.True(similarity1And2 > similarity2And3, $"Expected similarity between '{embedding1}' and '{embedding2}' to be greater than similarity between '{embedding2}' and '{embedding3}', but got {similarity1And2} vs {similarity2And3}.");
        }
    }
}
