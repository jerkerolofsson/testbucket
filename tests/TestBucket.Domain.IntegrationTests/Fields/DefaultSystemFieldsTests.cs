using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Milestones;

namespace TestBucket.Domain.IntegrationTests.Fields
{
    /// <summary>
    /// Integration tests for verifying the default system fields configuration and behavior.
    /// </summary>
    [IntegrationTest]
    [FunctionalTest]
    [EnrichedTest]
    public class DefaultSystemFieldsTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that the default "Approved" requirement field is correctly defined,
        /// has the correct target (Requirement), and requires the correct permission (Approve).
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task DefaultApproveRequirementField_CorrectlyDefined()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IFieldDefinitionManager>();
            var principal = Fixture.App.SiteAdministrator;

            // Act

            // Assert
            var fields = await manager.GetDefinitionsAsync(principal, Fixture.ProjectId);
            var field = fields.Where(x => x.TraitType == Traits.Core.TraitType.Approved).FirstOrDefault();
            Assert.NotNull(field);
            Assert.Equal(FieldTarget.Requirement, field.Target);
            Assert.Equal(PermissionLevel.Approve, field.RequiredPermission);
        }
    }
}