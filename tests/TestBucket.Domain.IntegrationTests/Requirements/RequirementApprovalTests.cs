namespace TestBucket.Domain.IntegrationTests.Requirements
{
    [IntegrationTest]
    [EnrichedTest]
    public class RequirementApprovalTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [CoveredRequirement("approval-of-requirements")]
        [FunctionalTest]
        [TestDescription("""
            Verifies that a requirement can be successfully approved
            """)]
        public async Task ApproveRequirement_WithPermission_Success()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IRequirementManager>();
            var principal = Fixture.App.SiteAdministrator;

            var requirement = new Requirement { Name = Guid.NewGuid().ToString() };
            await Fixture.Requirements.AddRequirementToNewSpecificationAsync(requirement);

            // Act
            await manager.ApproveRequirementAsync(principal, requirement);

            var approvedRequirement = await manager.GetRequirementByIdAsync(principal, requirement.Id);
            Assert.NotNull(approvedRequirement?.RequirementFields);
            var approvedField = approvedRequirement.RequirementFields.Where(x => x.FieldDefinition != null && x.FieldDefinition.TraitType == Traits.Core.TraitType.Approved).FirstOrDefault();
            Assert.NotNull(approvedField);
            Assert.Equal(true, approvedField.BooleanValue);
        }

        [Fact]
        [CoveredRequirement("approval-of-requirements")]

        [FunctionalTest]
        [TestDescription("""
            Verifies that a log entry is added when approving a requirement
            """)]
        public async Task ApproveRequirement_LogEntryIsAdded()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IRequirementManager>();
            var principal = Fixture.App.SiteAdministrator;

            var requirement = new Requirement { Name = Guid.NewGuid().ToString() };
            await Fixture.Requirements.AddRequirementToNewSpecificationAsync(requirement);

            // Act
            await manager.ApproveRequirementAsync(principal, requirement);

            // Assert
            var approvedRequirement = await manager.GetRequirementByIdAsync(principal, requirement.Id);
            Assert.NotNull(approvedRequirement?.Comments);
            Assert.Single(approvedRequirement.Comments);
            Assert.Equal("approved", approvedRequirement.Comments.First().LoggedAction);
        }

        [Fact]
        [CoveredRequirement("approval-of-requirements")]
        [SecurityTest]
        [TestDescription("""
            Verifies that a user without approval permission for requirements cannot approve a requirement
            """)]
        public async Task ApproveRequirement_WithoutPermission_Failure()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IRequirementManager>();
            var principal = Fixture.App.SiteAdministrator;

            var user = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = Fixture.App.Tenant;
                builder.Add(PermissionEntityType.RequirementSpecification, PermissionLevel.Read);
                builder.Add(PermissionEntityType.RequirementSpecification, PermissionLevel.Write);
                builder.Add(PermissionEntityType.Project, PermissionLevel.Read);
                builder.Add(PermissionEntityType.Requirement, PermissionLevel.Read);
                builder.Add(PermissionEntityType.Requirement, PermissionLevel.Write);
            });

            var requirementSpec = new RequirementSpecification { Name = Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId, TeamId = Fixture.TeamId };
            await manager.AddRequirementSpecificationAsync(user, requirementSpec);

            var requirement = new Requirement { Name = Guid.NewGuid().ToString(), RequirementSpecificationId = requirementSpec.Id };
            await manager.AddRequirementAsync(user, requirement);

            // Act
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await manager.ApproveRequirementAsync(user, requirement);
            });
        }
    }
}
