using TestBucket.Domain.AI;
using TestBucket.Domain.AI.Models;

namespace TestBucket.Domain.UnitTests.AI
{
    /// <summary>
    /// Contains test cases for <see cref="LlmModels"/>.
    /// </summary>
    [Feature("Chat")]
    [Component("AI")]
    [UnitTest]
    [FunctionalTest]
    [EnrichedTest]
    public class LlmModelsTests
    {
        /// <summary>
        /// Tests that <see cref="LlmModels.GetModelByName(string)"/> returns the correct model when the name exists.
        /// </summary>
        [Fact]
        public void GetModelByName_ReturnsCorrectModel_WhenNameExists()
        {
            // Arrange
            var expectedName = "Llama3.2 1b";

            // Act
            var model = LlmModels.GetModelByName(expectedName);

            // Assert
            Assert.NotNull(model);
            Assert.Equal(expectedName, model!.Name);
        }

        /// <summary>
        /// Tests that <see cref="LlmModels.GetModelByName(string)"/> returns null when the model name does not exist.
        /// </summary>
        [Fact]
        public void GetModelByName_ReturnsNull_WhenNameDoesNotExist()
        {
            // Arrange
            var nonExistentName = "NonExistentModel";

            // Act
            var model = LlmModels.GetModelByName(nonExistentName);

            // Assert
            Assert.Null(model);
        }

        /// <summary>
        /// Tests that <see cref="LlmModels.GetModels(ModelCapability)"/> returns models with the required capability.
        /// </summary>
        [Fact]
        public void GetModels_ReturnsModelsWithRequiredCapability()
        {
            // Arrange
            var requiredCapability = ModelCapability.Classification;

            // Act
            var models = LlmModels.GetModels(requiredCapability).ToList();

            // Assert
            Assert.NotEmpty(models);
            Assert.All(models, m => Assert.True((m.Capabilities & requiredCapability) == requiredCapability));
        }

        /// <summary>
        /// Tests that <see cref="LlmModels.GetNames(ModelCapability)"/> returns model names with the required capability.
        /// </summary>
        [Fact]
        public void GetNames_ReturnsNamesWithRequiredCapability()
        {
            // Arrange
            var requiredCapability = ModelCapability.Tools;

            // Act
            var names = LlmModels.GetNames(requiredCapability);

            // Assert
            Assert.NotEmpty(names);
            foreach (var name in names)
            {
                var model = LlmModels.Models.Values.FirstOrDefault(m => m.Name == name);
                Assert.NotNull(model);
                Assert.True((model!.Capabilities & requiredCapability) == requiredCapability);
            }
        }
    }
}