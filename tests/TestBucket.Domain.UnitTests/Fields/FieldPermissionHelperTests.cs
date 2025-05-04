using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields.Helpers;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;

namespace TestBucket.Domain.UnitTests.Fields
{
    [Feature("Custom Fields")]
    [UnitTest]
    [EnrichedTest]
    public class FieldPermissionHelperTests
    {
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


        [InlineData(FieldTarget.TestRun, PermissionEntityType.TestRun)]
        [InlineData(FieldTarget.TestCaseRun, PermissionEntityType.TestCaseRun)]
        [InlineData(FieldTarget.TestCase, PermissionEntityType.TestCase)]
        [InlineData(FieldTarget.TestSuite, PermissionEntityType.TestSuite)]
        [InlineData(FieldTarget.TestSuiteFolder, PermissionEntityType.TestSuite)]
        [InlineData(FieldTarget.Requirement, PermissionEntityType.Requirement)]
        [InlineData(FieldTarget.RequirementSpecificationFolder, PermissionEntityType.RequirementSpecification)]
        [InlineData(FieldTarget.RequirementSpecification, PermissionEntityType.RequirementSpecification)]
        [TestDescription("Verifies that the correct permission level is returned for a field definition based on the target")]
        [Theory]
        public void GetPermissionEntityTypeFromField_CorrectReturnType(FieldTarget target, PermissionEntityType expected)
        {
            var entityType = FieldPermissionHelper.GetPermissionEntityTypeFromField(target);
            Assert.Equal(expected, entityType);
        }
    }
}
