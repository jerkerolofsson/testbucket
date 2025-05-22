using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields.Helpers;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;

namespace TestBucket.Domain.UnitTests.Fields
{
    /// <summary>
    /// Contains unit tests for the <see cref="FieldPermissionHelper"/> and related permission logic for custom fields.
    /// </summary>
    [Feature("Custom Fields")]
    [UnitTest]
    [Component("Fields")]
    [EnrichedTest]
    [SecurityTest]
    public class FieldPermissionHelperTests
    {
        /// <summary>
        /// Verifies that a user with the required permission level for a requirement field has access.
        /// </summary>
        [Fact]
        public void HasPermissionToApproveRequirement_WithValidAccess_IsTrue()
        {
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "1234";
                builder.Add(PermissionEntityType.Requirement, PermissionLevel.Approve);
            });

            var fieldDefinition = new FieldDefinition { Name = "name", RequiredPermission = PermissionLevel.Approve, Target = FieldTarget.Requirement };

            var hasAccess = principal.HasPermission(fieldDefinition);
            Assert.True(hasAccess, "Expected user to have access");
        }

        /// <summary>
        /// Verifies that a user with approval permissions for both requirements and test cases has access to a field targeting both.
        /// </summary>
        [Fact]
        public void HasPermissionToApproveWhenTargetBothRequirementAndTest_WithValidAccess_IsTrue()
        {
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "1234";
                builder.Add(PermissionEntityType.Requirement, PermissionLevel.Approve);
                builder.Add(PermissionEntityType.TestCase, PermissionLevel.Approve);
            });

            var fieldDefinition = new FieldDefinition { Name = "name", RequiredPermission = PermissionLevel.Approve, Target = FieldTarget.Requirement | FieldTarget.TestCase };

            var hasAccess = principal.HasPermission(fieldDefinition);
            Assert.True(hasAccess, "Expected user to have access");
        }

        /// <summary>
        /// Verifies that a user with partial approval permissions does not have access to a field targeting both requirements and test cases.
        /// </summary>
        [Fact]
        public void HasPermissionToApproveWhenTargetBothRequirementAndTest_WithPartialAccess_IsFalse()
        {
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "1234";
                builder.Add(PermissionEntityType.Requirement, PermissionLevel.Approve);
                builder.Add(PermissionEntityType.TestCase, PermissionLevel.Read);
            });

            var fieldDefinition = new FieldDefinition { Name = "name", RequiredPermission = PermissionLevel.Approve, Target = FieldTarget.Requirement | FieldTarget.TestCase };

            var hasAccess = principal.HasPermission(fieldDefinition);
            Assert.False(hasAccess, "Expected user to not have access as field targets requirement and test, but user only has approval for requirement");
        }

        /// <summary>
        /// Verifies that a user without the required approval permission does not have access to a requirement field.
        /// </summary>
        [Fact]
        public void HasPermissionToApproveWhenTargetBothRequirementAndTest_WithNoValidAccess_IsFalse()
        {
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "1234";
                builder.Add(PermissionEntityType.Requirement, PermissionLevel.Read);
                builder.Add(PermissionEntityType.TestCase, PermissionLevel.Approve);
            });

            var fieldDefinition = new FieldDefinition { Name = "name", RequiredPermission = PermissionLevel.Approve, Target = FieldTarget.Requirement };

            var hasAccess = principal.HasPermission(fieldDefinition);
            Assert.False(hasAccess, "Expected user to not have access");
        }

        /// <summary>
        /// Verifies that <see cref="FieldPermissionHelper.GetPermissionEntityTypeFromField(FieldTarget)"/> returns the correct <see cref="PermissionEntityType"/>
        /// for each <see cref="FieldTarget"/> value.
        /// </summary>
        /// <param name="target">The field target to test.</param>
        /// <param name="expected">The expected permission entity type.</param>
        [InlineData(FieldTarget.TestRun, PermissionEntityType.TestRun)]
        [InlineData(FieldTarget.TestCaseRun, PermissionEntityType.TestCaseRun)]
        [InlineData(FieldTarget.TestCase, PermissionEntityType.TestCase)]
        [InlineData(FieldTarget.TestSuite, PermissionEntityType.TestSuite)]
        [InlineData(FieldTarget.TestSuiteFolder, PermissionEntityType.TestSuite)]
        [InlineData(FieldTarget.Requirement, PermissionEntityType.Requirement)]
        [InlineData(FieldTarget.RequirementSpecificationFolder, PermissionEntityType.RequirementSpecification)]
        [InlineData(FieldTarget.RequirementSpecification, PermissionEntityType.RequirementSpecification)]
        [Theory]
        public void GetPermissionEntityTypeFromField_CorrectReturnType(FieldTarget target, PermissionEntityType expected)
        {
            var entityType = FieldPermissionHelper.GetPermissionEntityTypeFromField(target);
            Assert.Equal(expected, entityType);
        }
    }
}