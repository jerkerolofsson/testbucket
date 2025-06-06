using TestBucket.Contracts.Code.Models;
using TestBucket.Domain.Code.Yaml;

namespace TestBucket.Domain.UnitTests.Code.Yaml
{
    /// <summary>
    /// Contains unit tests for the <see cref="ArchitectureYamlSerializer"/> class,
    /// focusing on validation, serialization, and parsing of architecture YAML files.
    /// </summary>
    [Feature("Architecture 1.0")]
    [UnitTest]
    [EnrichedTest]
    public class ArchitectureYamlParserTests
    {
        /// <summary>
        /// Verifies that when validating an empty YAML document, validation fails and returns errors.
        /// </summary>
        [Fact]
        public void ValidateYaml_WithEmptyDocument_GivesError()
        {
            string yaml = "";
            var errors = new ArchitectureYamlSerializer().Validate(yaml);
            Assert.NotEmpty(errors);
        }

        /// <summary>
        /// Verifies that a valid YAML document containing a system passes validation with no errors.
        /// </summary>
        [Fact]
        public void ValidateYaml_WithSystem_NoError()
        {
            string yaml = """
                systems:
                  Rocket:
                    paths: ["src/**/*"]
                """;
            var errors = new ArchitectureYamlSerializer().Validate(yaml);
            Assert.Empty(errors);
        }

        /// <summary>
        /// Verifies that a YAML document without a system section fails validation and returns errors.
        /// </summary>
        [Fact]
        public void ValidateYaml_WithoutSystem_GivesError()
        {
            string yaml = """
                features:
                  Feature1:
                    paths: ["src/**/*"]
                """;
            var errors = new ArchitectureYamlSerializer().Validate(yaml);
            Assert.NotEmpty(errors);
        }

        /// <summary>
        /// Verifies that serializing and then parsing a <see cref="ProjectArchitectureModel"/> results in an equivalent model.
        /// </summary>
        [Fact]
        public void Serialize_And_Parse_RoundTrip_Works()
        {
            var model = new ProjectArchitectureModel
            {
                Systems = new Dictionary<string, ArchitecturalComponent>
                {
                    ["Rocket"] = new ArchitecturalComponent
                    {
                        Description = "Rocket system",
                        DevLead = "Alice",
                        TestLead = "Bob",
                        Paths = new List<string> { "src/rocket/**/*" }
                    }
                },
                Components = new Dictionary<string, ArchitecturalComponent>(),
                Layers = new Dictionary<string, ArchitecturalComponent>(),
                Features = new Dictionary<string, ArchitecturalComponent>()
            };

            var serializer = new ArchitectureYamlSerializer();
            var yaml = serializer.Serialize(model);
            var parsed = serializer.Parse(yaml);

            Assert.NotNull(parsed);
            Assert.True(parsed.Systems.ContainsKey("Rocket"));
            Assert.Equal("Rocket system", parsed.Systems["Rocket"].Description);
            Assert.Equal("Alice", parsed.Systems["Rocket"].DevLead);
            Assert.Equal("Bob", parsed.Systems["Rocket"].TestLead);
            var paths = parsed.Systems["Rocket"].Paths;
            Assert.NotNull(paths);
            Assert.Contains("src/rocket/**/*", paths);
        }

        /// <summary>
        /// Verifies that parsing invalid YAML throws an <see cref="ArgumentException"/>.
        /// </summary>
        [Fact]
        public void Parse_InvalidYaml_ThrowsArgumentException()
        {
            string invalidYaml = "systems: [unclosed";
            var serializer = new ArchitectureYamlSerializer();
            Assert.Throws<ArgumentException>(() => serializer.Parse(invalidYaml));
        }


        /// <summary>
        /// Verifies that serializing a model with multiple sections produces valid YAML and passes validation.
        /// </summary>
        [Fact]
        public void Serialize_ModelWithMultipleSections_ProducesValidYaml()
        {
            var model = new ProjectArchitectureModel
            {
                Systems = new Dictionary<string, ArchitecturalComponent>
                {
                    ["Rocket"] = new ArchitecturalComponent { Description = "Rocket" }
                },
                Components = new Dictionary<string, ArchitecturalComponent>
                {
                    ["Engine"] = new ArchitecturalComponent { Description = "Engine" }
                },
                Layers = new Dictionary<string, ArchitecturalComponent>
                {
                    ["Data"] = new ArchitecturalComponent { Description = "Data Layer" }
                },
                Features = new Dictionary<string, ArchitecturalComponent>
                {
                    ["Telemetry"] = new ArchitecturalComponent { Description = "Telemetry" }
                }
            };

            var serializer = new ArchitectureYamlSerializer();
            var yaml = serializer.Serialize(model);
            var errors = serializer.Validate(yaml);

            Assert.False(string.IsNullOrWhiteSpace(yaml));
            Assert.Empty(errors);
        }
    }
}