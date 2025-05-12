using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.IntegrationTests.Fixtures;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestSuites;
using TestBucket.Traits.Xunit;
using Xunit;

namespace TestBucket.Domain.IntegrationTests.Requirements
{
    [IntegrationTest]
    [EnrichedTest]
    public class RequirementFolderTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [FunctionalTest]
        [TestDescription("""
            Verifies that the Path is updated on a requirement when it is moved to a folder
            """)]
        public async Task MoveRequirement_IntoFolder_PathUpdated()
        {
            // Arrange
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IRequirementManager>();
            var principal = Fixture.App.SiteAdministrator;
            var requirement = await Fixture.Requirements.AddRequirementToNewSpecificationAsync();

            var folder = await Fixture.Requirements.AddFolderAsync("folder1", requirement.RequirementSpecificationId);

            // Act 
            requirement.RequirementSpecificationFolderId = folder.Id;
            await Fixture.Requirements.UpdateAsync(requirement);

            // Assert
            Assert.NotNull(requirement.Path);
            Assert.NotNull(requirement.PathIds);
            Assert.Single(requirement.PathIds);
            Assert.Equal(folder.Id, requirement.PathIds[0]);
            Assert.Equal(folder.Name, requirement.Path);
        }

        [Fact]
        [FunctionalTest]
        [TestDescription("""
            Verifies that the path is updated on a requirement when renaming a folder
            """)]
        public async Task RenameFolder_WithRequirement_PathUpdated()
        {
            // Arrange
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IRequirementManager>();
            var principal = Fixture.App.SiteAdministrator;
            var requirement = await Fixture.Requirements.AddRequirementToNewSpecificationAsync();

            var folder = await Fixture.Requirements.AddFolderAsync("folder1", requirement.RequirementSpecificationId);
            requirement.RequirementSpecificationFolderId = folder.Id;
            await Fixture.Requirements.UpdateAsync(requirement);

            // Act 
            folder.Name = "folder2";
            await Fixture.Requirements.UpdateAsync(folder);

            // Assert
            requirement = await Fixture.Requirements.GetRequirementByIdAsync(requirement.Id);
            Assert.NotNull(requirement);
            Assert.NotNull(requirement.Path);
            Assert.NotNull(requirement.PathIds);
            Assert.Single(requirement.PathIds);
            Assert.Equal(folder.Id, requirement.PathIds[0]);
            Assert.Equal(folder.Name, requirement.Path);
        }
    }
}
