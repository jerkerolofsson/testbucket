using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts.TestResources;
using TestBucket.Domain.TestResources.Mapping;
using TestBucket.Domain.TestResources.Models;
using Xunit;

namespace TestBucket.Domain.UnitTests.TestResources
{
    /// <summary>
    /// Unit tests for the TestResourceMapping class.
    /// </summary>
    /// <remarks>
    /// This class contains tests for the mapping methods ToDbo and ToDto.
    /// </remarks>
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    [Component("Test Resources")]
    public class TestResourceMappingTests
    {
        /// <summary>
        /// Tests the ToDbo method to ensure it correctly maps a TestResourceDto to a TestResource.
        /// </summary>
        /// <remarks>
        /// Verifies that all properties are mapped correctly and default values are set as expected.
        /// </remarks>
        [Fact]
        public void ToDbo_MapsDtoToDboCorrectly()
        {
            // Arrange
            var dto = new TestResourceDto
            {
                Name = "Test Resource",
                ResourceId = "12345",
                Owner = "Owner1",
                Types = new[] { "Type1", "Type2" },
                Manufacturer = "Manufacturer1",
                Model = "Model1",
                Variables = new Dictionary<string, string> { { "Key1", "Value1" } },
                Health = HealthStatus.Healthy
            };

            // Act
            var dbo = dto.ToDbo();

            // Assert
            Assert.Equal(dto.Name, dbo.Name);
            Assert.Equal(dto.ResourceId, dbo.ResourceId);
            Assert.Equal(dto.Owner, dbo.Owner);
            Assert.Equal(dto.Types, dbo.Types);
            Assert.Equal(dto.Manufacturer, dbo.Manufacturer);
            Assert.Equal(dto.Model, dbo.Model);
            Assert.Equal(dto.Variables, dbo.Variables);
            Assert.Equal(dto.Health, dbo.Health);
            Assert.True(dbo.Enabled);
            Assert.False(dbo.Locked);
        }

        /// <summary>
        /// Tests the ToDto method to ensure it correctly maps a TestResource to a TestResourceDto.
        /// </summary>
        /// <remarks>
        /// Verifies that all properties are mapped correctly.
        /// </remarks>
        [Fact]
        public void ToDto_MapsDboToDtoCorrectly()
        {
            // Arrange
            var dbo = new TestResource
            {
                Name = "Test Resource",
                ResourceId = "12345",
                Owner = "Owner1",
                Types = new[] { "Type1", "Type2" },
                Manufacturer = "Manufacturer1",
                Model = "Model1",
                Variables = new Dictionary<string, string> { { "Key1", "Value1" } },
                Health = HealthStatus.Healthy
            };

            // Act
            var dto = dbo.ToDto();

            // Assert
            Assert.Equal(dbo.Name, dto.Name);
            Assert.Equal(dbo.ResourceId, dto.ResourceId);
            Assert.Equal(dbo.Owner, dto.Owner);
            Assert.Equal(dbo.Types, dto.Types);
            Assert.Equal(dbo.Manufacturer, dto.Manufacturer);
            Assert.Equal(dbo.Model, dto.Model);
            Assert.Equal(dbo.Variables, dto.Variables);
            Assert.Equal(dbo.Health, dto.Health);
        }
    }
}
