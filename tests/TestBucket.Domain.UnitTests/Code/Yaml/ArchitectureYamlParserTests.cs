using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Code.Yaml;

namespace TestBucket.Domain.UnitTests.Code.Yaml
{
    [Feature("Architecture 1.0")]
    [UnitTest]
    [EnrichedTest]
    public class ArchitectureYamlParserTests
    {
        [Fact]
        public void ValidateYaml_WithEmptyDocument_GivesError()
        {
            string yaml = "";
            var errors = new ArchitectureYamlSerializer().Validate(yaml);
            Assert.NotEmpty(errors);
        }

        [Fact]
        public void ValidateYaml_WithSystem_NoError()
        {
            string yaml = """
                systems:
                  Rocket:
                    paths: src/**/*
                """;
            var errors = new ArchitectureYamlSerializer().Validate(yaml);
            Assert.Empty(errors);
        }

        [Fact]
        public void ValidateYaml_WithoutSystem_GivesError()
        {
            string yaml = """
                features:
                  Feature1:
                    paths: src/**/*
                """;
            var errors = new ArchitectureYamlSerializer().Validate(yaml);
            Assert.NotEmpty(errors);
        }
    }
}
