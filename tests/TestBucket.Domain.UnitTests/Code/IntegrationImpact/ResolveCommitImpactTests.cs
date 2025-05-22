using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Code.Services.IntegrationImpact;
using TestBucket.Domain.Code.Yaml;
using TestBucket.Domain.Code.Yaml.Models;
using TestBucket.Contracts.Code.Models;

namespace TestBucket.Domain.UnitTests.Code.IntegrationImpact
{
    /// <summary>
    /// Contains unit tests for <see cref="ResolveCommitImpactHandler"/> and related commit impact resolution logic.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    public class ResolveCommitImpactTests
    {
        /// <summary>
        /// Verifies that when a file is changed, but the file is not mapped to a feature, no feature is identified.
        /// </summary>
        [Fact]
        public async Task ResolveCommitImpact_WithNoMatchingFeature_EmptyListReturned()
        {
            // Arrange
            ProjectArchitectureModel model = new ProjectArchitectureModel();
            model.Features["feature2"] = new ArchitecturalComponent() { Paths = ["src/data/area2/**/*"] };

            var commit = new Commit
            {
                Reference = "a",
                Sha = "b",
                CommitFiles = [
                    new CommitFile() { Additions = 10, Path = "src/data/area1/test1.cs", Sha = "a"}
                ]
            };

            var request = new ResolveCommitImpactRequest(commit, model);
            var handler = new ResolveCommitImpactHandler();

            var impact = await handler.Handle(request, TestContext.Current.CancellationToken);
            Assert.Empty(impact.Features);
        }

        /// <summary>
        /// Verifies that when multiple features exist but only one matches the changed file, only the correct feature is identified.
        /// </summary>
        [Fact]
        public async Task ResolveCommitImpact_WithMultipleFeaturesButOnlyOneMatching_OneFeatureIdentified()
        {
            // Arrange
            ProjectArchitectureModel model = new ProjectArchitectureModel();
            model.Features["feature1"] = new ArchitecturalComponent() { Paths = ["src/data/area1/**/*"] };
            model.Features["feature2"] = new ArchitecturalComponent() { Paths = ["src/data/area2/**/*"] };

            var commit = new Commit
            {
                Reference = "a",
                Sha = "b",
                CommitFiles = [
                    new CommitFile() { Additions = 10, Path = "src/data/area2/test1.cs", Sha = "a"}
                ]
            };

            var request = new ResolveCommitImpactRequest(commit, model);
            var handler = new ResolveCommitImpactHandler();

            var impact = await handler.Handle(request, TestContext.Current.CancellationToken);
            Assert.Single(impact.Features);
            Assert.Equal("feature2", impact.Features[0]);
        }

        /// <summary>
        /// Verifies that when a file impacts a layer, the correct system is identified.
        /// </summary>
        [Fact]
        public async Task ResolveCommitImpact_WithFileImpactingLayer_CorrectSystemIdentified()
        {
            var commit = new Commit
            {
                Reference = "a",
                Sha = "b",
                CommitFiles = [
                    new CommitFile() { Additions = 10, Path = "src/api/area1/test1.cs", Sha = "a"}
                ]
            };

            ProjectArchitectureModel model = GetTestProjectArchitectureModel();

            var request = new ResolveCommitImpactRequest(commit, model);
            var handler = new ResolveCommitImpactHandler();

            var impact = await handler.Handle(request, TestContext.Current.CancellationToken);
            Assert.Single(impact.Systems);
            Assert.Equal("src", impact.Systems[0]);
        }

        /// <summary>
        /// Verifies that when a file impacts another system without components, the correct system is identified and other lists are empty.
        /// </summary>
        [Fact]
        public async Task ResolveCommitImpact_WithFileImpactingOtherSystemWithoutComponents_CorrectSystemIdentified()
        {
            var commit = new Commit
            {
                Reference = "a",
                Sha = "b",
                CommitFiles = [
                    new CommitFile() { Additions = 10, Path = "tests/api/area1/test1.cs", Sha = "a"}
                ]
            };

            ProjectArchitectureModel model = GetTestProjectArchitectureModel();

            var request = new ResolveCommitImpactRequest(commit, model);
            var handler = new ResolveCommitImpactHandler();

            var impact = await handler.Handle(request, TestContext.Current.CancellationToken);
            Assert.Single(impact.Systems);
            Assert.Empty(impact.Layers);
            Assert.Empty(impact.Components);
            Assert.Empty(impact.Features);
            Assert.Equal("tests", impact.Systems[0]);
        }

        /// <summary>
        /// Verifies that when a file impacts a layer, the correct layer is identified.
        /// </summary>
        [Fact]
        public async Task ResolveCommitImpact_WithFileImpactingLayer_CorrectLayerIdentified()
        {
            var commit = new Commit
            {
                Reference = "a",
                Sha = "b",
                CommitFiles = [
                    new CommitFile() { Additions = 10, Path = "src/api/area1/test1.cs", Sha = "a"}
                ]
            };

            ProjectArchitectureModel model = GetTestProjectArchitectureModel();

            var request = new ResolveCommitImpactRequest(commit, model);
            var handler = new ResolveCommitImpactHandler();

            var impact = await handler.Handle(request, TestContext.Current.CancellationToken);
            Assert.Single(impact.Layers);
            Assert.Equal("api", impact.Layers[0]);
        }

        /// <summary>
        /// Verifies that when a file impacts many types, all types (features, layers, components) are identified.
        /// </summary>
        [Fact]
        public async Task ResolveCommitImpact_WithFileImpactingManyTypes_AllTypesIdentified()
        {
            var commit = new Commit
            {
                Reference = "a",
                Sha = "b",
                CommitFiles = [
                    new CommitFile() { Additions = 10, Path = "src/data/area1/test1.cs", Sha = "a"}
                ]
            };

            ProjectArchitectureModel model = GetTestProjectArchitectureModel();

            var request = new ResolveCommitImpactRequest(commit, model);
            var handler = new ResolveCommitImpactHandler();

            var impact = await handler.Handle(request, TestContext.Current.CancellationToken);
            Assert.Single(impact.Features);
            Assert.Single(impact.Layers);
            Assert.Single(impact.Components);
        }

        /// <summary>
        /// Verifies that when two files are changed related to the same feature, only one feature is found.
        /// </summary>
        [Fact]
        public async Task ResolveCommitImpact_WithTwoModifiedFilesInSameFeature_SingleFeatureFound()
        {
            var commit = new Commit
            {
                Reference = "a",
                Sha = "b",
                CommitFiles = [
                    new CommitFile() { Additions = 10, Path = "src/data/area1/test1.cs", Sha = "a"},
                    new CommitFile() { Additions = 10, Path = "src/data/area1/test2.cs", Sha = "a"},
                ]
            };

            ProjectArchitectureModel model = GetTestProjectArchitectureModel();

            var request = new ResolveCommitImpactRequest(commit, model);
            var handler = new ResolveCommitImpactHandler();

            var impact = await handler.Handle(request, TestContext.Current.CancellationToken);
            Assert.Single(impact.Features);
            Assert.Single(impact.Components);
            Assert.Single(impact.Layers);
            Assert.Single(impact.Systems);
        }

        /// <summary>
        /// Returns a test <see cref="ProjectArchitectureModel"/> with predefined systems, layers, components, and features for use in tests.
        /// </summary>
        /// <returns>A <see cref="ProjectArchitectureModel"/> instance with test data.</returns>
        private ProjectArchitectureModel GetTestProjectArchitectureModel()
        {
            var srcSystem = new ArchitecturalComponent() { Paths = ["src/**/*"] };
            var testSystem = new ArchitecturalComponent() { Paths = ["tests/**/*"] };

            var apiLayer = new ArchitecturalComponent() { Paths = ["src/api/**/*"] };
            var dataLayer = new ArchitecturalComponent() { Paths = ["src/data/**/*"] };
            var component1 = new ArchitecturalComponent() { Paths = ["src/data/area1/**/*"] };
            var component2 = new ArchitecturalComponent() { Paths = ["src/data/area2/**/*"] };
            var apiComponent = new ArchitecturalComponent() { Paths = ["src/api/area1/**/*"] };

            var model = new ProjectArchitectureModel();
            model.Features["feature1"] = component1;
            model.Features["feature2"] = component2;

            model.Components["area1"] = component1;
            model.Components["area2"] = component2;
            model.Components["api1"] = apiComponent;

            model.Layers["data"] = dataLayer;
            model.Layers["api"] = apiLayer;

            model.Systems["src"] = srcSystem;
            model.Systems["tests"] = testSystem;

            return model;
        }
    }
}