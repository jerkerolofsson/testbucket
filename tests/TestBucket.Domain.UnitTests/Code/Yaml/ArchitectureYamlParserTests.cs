using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Code.Yaml;

namespace TestBucket.Domain.UnitTests.Code.Yaml
{
    /// <summary>
    /// Tests related to the architecture yaml file
    /// </summary>
    [Feature("Architecture 1.0")]
    [UnitTest]
    [EnrichedTest]
    public class ArchitectureYamlParserTests
    {
        /// <summary>
        /// Verifies that when validating an empty yaml document, it fails correctly
        /// </summary>
        [Fact]
        public void ValidateYaml_WithEmptyDocument_GivesError()
        {
            string yaml = "";
            var errors = new ArchitectureYamlSerializer().Validate(yaml);
            Assert.NotEmpty(errors);
        }

        /// <summary>
        /// Verifies that there are no errors when validating a valid yaml with a system
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
        /// Verifies that there is an error when validating a yaml file without system
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
    }
}
