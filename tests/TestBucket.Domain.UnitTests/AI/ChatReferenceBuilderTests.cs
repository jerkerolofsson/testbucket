using TestBucket.Domain.AI.Agent;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Projects.Models;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.UnitTests.AI
{
    /// <summary>
    /// Unit tests for the <see cref="ChatReferenceBuilder"/> class.
    /// </summary>
    [Feature("Chat")]
    [Component("AI")]
    [UnitTest]
    [FunctionalTest]
    public class ChatReferenceBuilderTests
    {
        /// <summary>
        /// Tests that a chat reference created from a <see cref="TestProject"/> maps properties correctly.
        /// </summary>
        [Fact]
        public void CreateChatReference_FromTestProject_ShouldMapPropertiesCorrectly()
        {
            var project = new TestProject { Name = "Project A", Description = "Description A", Id = 1, Slug = "project-a", ShortName = "PA" };
            var chatReference = ChatReferenceBuilder.Create(project, true);

            Assert.Equal(project.Name, chatReference.Name);
            Assert.Equal(project.Description, chatReference.Text);
            Assert.Equal(project.Id, chatReference.Id);
            Assert.Equal("Project", chatReference.EntityTypeName);
            Assert.True(chatReference.IsActiveDocument);
        }

        /// <summary>
        /// Tests that a chat reference created from a <see cref="TestCase"/> maps properties correctly.
        /// </summary>
        [Fact]
        public void CreateChatReference_FromTestCase_ShouldMapPropertiesCorrectly()
        {
            var testCase = new TestCase { Name = "Test Case A", Description = "Description B", Id = 1 };
            var chatReference = ChatReferenceBuilder.Create(testCase);

            Assert.Equal(testCase.Name, chatReference.Name);
            Assert.Equal(testCase.Description, chatReference.Text);
            Assert.Equal(testCase.Id, chatReference.Id);
            Assert.Equal("TestCase", chatReference.EntityTypeName);
            Assert.False(chatReference.IsActiveDocument);
        }

        /// <summary>
        /// Tests that a chat reference created from a <see cref="TestSuite"/> maps properties correctly.
        /// </summary>
        [Fact]
        public void CreateChatReference_FromTestSuite_ShouldMapPropertiesCorrectly()
        {
            var testSuite = new TestSuite { Name = "Test Suite A", Description = "Description C", Id = 1 };
            var chatReference = ChatReferenceBuilder.Create(testSuite);

            Assert.Equal(testSuite.Name, chatReference.Name);
            Assert.Equal(testSuite.Description, chatReference.Text);
            Assert.Equal(testSuite.Id, chatReference.Id);
            Assert.Equal("TestSuite", chatReference.EntityTypeName);
            Assert.False(chatReference.IsActiveDocument);
        }

        /// <summary>
        /// Tests that a chat reference created from a <see cref="Requirement"/> maps properties correctly.
        /// </summary>
        [Fact]
        public void CreateChatReference_FromRequirement_ShouldMapPropertiesCorrectly()
        {
            var requirement = new Requirement { Name = "Requirement A", Description = "Description D", Id = 1 };
            var chatReference = ChatReferenceBuilder.Create(requirement);

            Assert.Equal(requirement.Name, chatReference.Name);
            Assert.Equal(requirement.Description, chatReference.Text);
            Assert.Equal(requirement.Id, chatReference.Id);
            Assert.Equal("Requirement", chatReference.EntityTypeName);
            Assert.False(chatReference.IsActiveDocument);
        }

        /// <summary>
        /// Tests that a chat reference created from a <see cref="Feature"/> maps properties correctly.
        /// </summary>
        [Fact]
        public void CreateChatReference_FromFeature_ShouldMapPropertiesCorrectly()
        {
            var feature = new Feature { Name = "Feature A", Description = "Description E", Id = 1, GlobPatterns = [] };
            var chatReference = ChatReferenceBuilder.Create(feature);

            Assert.Equal(feature.Name, chatReference.Name);
            Assert.Equal(feature.Description, chatReference.Text);
            Assert.Equal(feature.Id, chatReference.Id);
            Assert.Equal("Feature", chatReference.EntityTypeName);
            Assert.False(chatReference.IsActiveDocument);
        }

        /// <summary>
        /// Tests that a chat reference created from a <see cref="Component"/> maps properties correctly.
        /// </summary>
        [Fact]
        public void CreateChatReference_FromComponent_ShouldMapPropertiesCorrectly()
        {
            var component = new Component { Name = "Component A", Description = "Description F", Id = 1, GlobPatterns = [] };
            var chatReference = ChatReferenceBuilder.Create(component);

            Assert.Equal(component.Name, chatReference.Name);
            Assert.Equal(component.Description, chatReference.Text);
            Assert.Equal(component.Id, chatReference.Id);
            Assert.Equal("Component", chatReference.EntityTypeName);
            Assert.False(chatReference.IsActiveDocument);
        }
    }
}