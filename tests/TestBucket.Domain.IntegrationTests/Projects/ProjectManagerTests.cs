using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.IntegrationTests.Projects
{
    /// <summary>
    /// Tests for managing projects
    /// </summary>
    /// <param name="Fixture"></param>
    [IntegrationTest]
    [EnrichedTest]
    [Component("Projects")]
    [FunctionalTest]
    public class ProjectManagerTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that a project short name is set when creating a new project
        /// </summary>
        /// <returns></returns>
        [Fact]
        [FunctionalTest]
        public async Task ShortNameSet_WhenCreatingProject()
        {
            // Arrange
            var project = await Fixture.CreateProjectAsync("Disco Fever");

            // Act
            try
            {
                Assert.NotNull(project.ShortName);
                Assert.Equal("DF", project.ShortName);
            }
            finally
            {
                await Fixture.DeleteProjectAsync(project);
            }
        }
    }
}
