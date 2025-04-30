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
    [UnitTest]
    [EnrichedTest]
    public class ResolveCommitImpactTests
    {
        [Fact]
        [TestDescription("Verifies that when a files is changed, but the file is not mapped to a feature, no feature is identified")]
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

        [Fact]
        [TestDescription("Verifies that when a files is changed, the correct system is identified")]
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

        [Fact]
        [TestDescription("Verifies that when a files is changed, the correct system is identified")]
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

        [Fact]
        [TestDescription("Verifies that when a files is changed, the correct system is identified")]
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

        [Fact]
        [TestDescription("Verifies that when a files is changed, the correct layer is identified")]
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

        [Fact]
        [TestDescription("Verifies that when a files is changed, systems, components, layers, and features are all identified")]
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

        [Fact]
        [TestDescription("Verifies that when two files are changed related to the same feature, only one is found")]
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
