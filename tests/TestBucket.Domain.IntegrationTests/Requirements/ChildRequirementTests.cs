using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.IntegrationTests.Requirements
{

    [IntegrationTest]
    [EnrichedTest]
    [FunctionalTest]
    public class ChildRequirementTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// This test verifies that when adding a child requirement, fields that have "inherited: true" are copied
        /// to the child requirement.
        /// 
        /// # Steps
        /// 1. Create a requirement
        /// 2. Assign a value to an inherited field (Milestone)
        /// 3. Create a child requirement
        /// 4. Verify that the child requirement has the same field value as the parent
        /// </summary>
        /// <returns></returns>
        [Fact]
        [CoveredRequirement("when-creating-a-child-requirement-fields-should-be-copied-from-the-parent")]
        public async Task AddChildRequirement_FieldsAreCopiedFromParent()
        {
            // Arrange
            var requirement = new Requirement { Name = "PARENT " + Guid.NewGuid().ToString() };
            await Fixture.Requirements.AddRequirementToNewSpecificationAsync(requirement);
            await Fixture.Requirements.SetMilestoneAsync(requirement, "1.0");

            // Act
            var childRequirement = new Requirement { Name = "CHILD " + Guid.NewGuid().ToString(), ParentRequirementId = requirement.Id };
            await Fixture.Requirements.AddRequirementToNewSpecificationAsync(childRequirement);

            // Assert
            var milestone = await Fixture.Requirements.GetMilestoneAsync(childRequirement);
            Assert.Equal("1.0", milestone);
        }
    }
}
