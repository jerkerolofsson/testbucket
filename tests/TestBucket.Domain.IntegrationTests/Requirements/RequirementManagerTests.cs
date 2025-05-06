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
    public class RequirementManagerTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [FunctionalTest]
        public async Task AddRequirement_ToSpecificationRoot()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IRequirementManager>();
            var principal = Fixture.App.SiteAdministrator;

            var requirementSpec = new RequirementSpecification { Name = Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId, TeamId = Fixture.TeamId };
            await manager.AddRequirementSpecificationAsync(principal, requirementSpec);

            var requirement = new Requirement { Name = Guid.NewGuid().ToString(), RequirementSpecificationId = requirementSpec.Id };
            await manager.AddRequirementAsync(principal, requirement);
        }


        [Fact]
        [FunctionalTest]
        public async Task SearchRequirementsInSpecification_WithTwoSpecs_CorrectResult()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IRequirementManager>();
            var principal = Fixture.App.SiteAdministrator;

            // Add two specs
            var requirementSpec1 = new RequirementSpecification { Name = Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId, TeamId = Fixture.TeamId };
            var requirementSpec2 = new RequirementSpecification { Name = Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId, TeamId = Fixture.TeamId };
            await manager.AddRequirementSpecificationAsync(principal, requirementSpec1);
            await manager.AddRequirementSpecificationAsync(principal, requirementSpec2);

            var requirement1 = new Requirement { Name = Guid.NewGuid().ToString(), RequirementSpecificationId = requirementSpec1.Id };
            await manager.AddRequirementAsync(principal, requirement1);

            var requirement2 = new Requirement { Name = Guid.NewGuid().ToString(), RequirementSpecificationId = requirementSpec2.Id };
            await manager.AddRequirementAsync(principal, requirement2);

            var result = await manager.SearchRequirementsAsync(principal, new SearchRequirementQuery { RequirementSpecificationId = requirementSpec1.Id, CompareFolder = false });
            Assert.Equal(1, result.TotalCount);
        }
    }
}
