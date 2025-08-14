using System.Linq;
using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields.SystemFields;
using TestBucket.Domain.Identity.Permissions;
using Xunit;

namespace TestBucket.Domain.UnitTests.Fields
{
    /// <summary>
    /// Unit tests for <see cref="SystemFieldDefinitions"/>.
    /// </summary>
    [Feature("Custom Fields")]
    [UnitTest]
    [Component("Fields")]
    [EnrichedTest]
    [FunctionalTest]
    public class SystemFieldDefinitionsTests
    {
        /// <summary>
        /// Verifies that the <see cref="SystemFieldDefinitions.Fixed"/> property returns all required system fields.
        /// </summary>
        [Fact]
        public void Fixed_ShouldContainAllSystemFields()
        {
            // Act
            var fields = SystemFieldDefinitions.Fixed;

            // Assert
            Assert.NotNull(fields);

            var names = fields.Select(f => f.Name).ToArray();
            Assert.Contains("Label", names);
            Assert.Contains("Feature", names);
            Assert.Contains("Milestone", names);
            Assert.Contains("Branch", names);
            Assert.Contains("Commit", names);
            Assert.Contains("Approved", names);
            Assert.Contains("Q-Char", names);
        }

        /// <summary>
        /// Verifies that all system fields are defined by the system.
        /// </summary>
        [Fact]
        public void AllFields_ShouldBeDefinedBySystem()
        {
            var fields = SystemFieldDefinitions.Fixed;
            Assert.All(fields, f => Assert.True(f.IsDefinedBySystem));
        }


        /// <summary>
        /// Verifies that the quality characteristic field has a default value
        /// </summary>
        [Fact]
        [CoveredRequirement("TB-FIELDS-001")]
        public void QCharField_HasFunctionalSuitabilityDefaultValue()
        {
            var qchar = SystemFieldDefinitions.Fixed.FirstOrDefault(f => f.Name == "Q-Char");
            Assert.NotNull(qchar);
            Assert.Equal("Functional Suitability", qchar.DefaultValue);
        }

        /// <summary>
        /// Verifies that the "Approved" field requires Approve permission.
        /// </summary>
        [Fact]
        public void ApprovedField_ShouldRequireApprovePermission()
        {
            var approved = SystemFieldDefinitions.Fixed.FirstOrDefault(f => f.Name == "Approved");
            Assert.NotNull(approved);
            Assert.Equal(PermissionLevel.Approve, approved.RequiredPermission);
        }

        /// <summary>
        /// Verifies that the "Feature" field uses a classifier and has the correct type.
        /// </summary>
        [Fact]
        public void FeatureField_ShouldUseClassifier_AndBeSingleSelection()
        {
            var feature = SystemFieldDefinitions.Fixed.FirstOrDefault(f => f.Name == "Feature");
            Assert.NotNull(feature);
            Assert.True(feature.UseClassifier);
            Assert.Equal(FieldType.SingleSelection, feature.Type);
        }

        /// <summary>
        /// Verifies that the "Milestone" field is visible and has the correct data source type.
        /// </summary>
        [Fact]
        public void MilestoneField_ShouldBeVisible_AndHaveMilestonesDataSource()
        {
            var milestone = SystemFieldDefinitions.Fixed.FirstOrDefault(f => f.Name == "Milestone");
            Assert.NotNull(milestone);
            Assert.True(milestone.IsVisible);
            Assert.Equal(FieldDataSourceType.Milestones, milestone.DataSourceType);
        }

        /// <summary>
        /// Verifies that the "Label" field targets Issue and Requirement.
        /// </summary>
        [Fact]
        public void LabelField_ShouldTargetIssueAndRequirement()
        {
            var label = SystemFieldDefinitions.Fixed.FirstOrDefault(f => f.Name == "Label");
            Assert.NotNull(label);
            Assert.True((label.Target & FieldTarget.Issue) == FieldTarget.Issue);
            Assert.True((label.Target & FieldTarget.Requirement) == FieldTarget.Requirement);
        }

        /// <summary>
        /// Verifies that the "Branch" field does not use a classifier and is of type String.
        /// </summary>
        [Fact]
        public void BranchField_ShouldNotUseClassifier_AndBeStringType()
        {
            var branch = SystemFieldDefinitions.Fixed.FirstOrDefault(f => f.Name == "Branch");
            Assert.NotNull(branch);
            Assert.False(branch.UseClassifier);
            Assert.Equal(FieldType.String, branch.Type);
        }

        /// <summary>
        /// Verifies that the "Commit" field has the correct data source type and targets TestRun, TestCaseRun, and Issue.
        /// </summary>
        [Fact]
        public void CommitField_ShouldHaveCommitDataSource_AndCorrectTargets()
        {
            var commit = SystemFieldDefinitions.Fixed.FirstOrDefault(f => f.Name == "Commit");
            Assert.NotNull(commit);
            Assert.Equal(FieldDataSourceType.Commit, commit.DataSourceType);
            Assert.True((commit.Target & FieldTarget.TestRun) == FieldTarget.TestRun);
            Assert.True((commit.Target & FieldTarget.TestCaseRun) == FieldTarget.TestCaseRun);
            Assert.True((commit.Target & FieldTarget.Issue) == FieldTarget.Issue);
        }

        /// <summary>
        /// Verifies that the "Approved" field exists in the system fields.
        /// </summary>
        [Fact]
        [CoveredRequirement("TB-REVIEW-001")]
        public void ApprovedField_ShouldExist()
        {
            var approved = SystemFieldDefinitions.Fixed.FirstOrDefault(f => f.Name == "Approved");
            Assert.NotNull(approved);
        }

        /// <summary>
        /// Verifies that the "Approved" field is of Boolean type.
        /// </summary>
        [Fact]
        [CoveredRequirement("TB-REVIEW-001")]
        public void ApprovedField_ShouldBeBooleanType()
        {
            var approved = SystemFieldDefinitions.Fixed.FirstOrDefault(f => f.Name == "Approved");
            Assert.NotNull(approved);
            Assert.Equal(FieldType.Boolean, approved.Type);
        }

        /// <summary>
        /// Verifies that the "Approved" field is visible.
        /// </summary>
        [Fact]
        [CoveredRequirement("TB-REVIEW-001")]
        public void ApprovedField_ShouldBeVisible()
        {
            var approved = SystemFieldDefinitions.Fixed.FirstOrDefault(f => f.Name == "Approved");
            Assert.NotNull(approved);
            Assert.True(approved.IsVisible);
        }

        /// <summary>
        /// Verifies that the "Approved" field has a target that contains TestCase
        /// </summary>
        [Fact]
        [CoveredRequirement("TB-REVIEW-001")]
        public void ApprovedField_ShouldTargetTests()
        {
            var approved = SystemFieldDefinitions.Fixed.FirstOrDefault(f => f.Name == "Approved");
            Assert.NotNull(approved);
            Assert.Equal(FieldTarget.TestCase, (approved.Target & FieldTarget.TestCase));
        }

        /// <summary>
        /// Verifies that the "Approved" field is defined by the system.
        /// </summary>
        [Fact]
        [CoveredRequirement("TB-REVIEW-001")]
        public void ApprovedField_ShouldBeDefinedBySystem()
        {
            var approved = SystemFieldDefinitions.Fixed.FirstOrDefault(f => f.Name == "Approved");
            Assert.NotNull(approved);
            Assert.True(approved.IsDefinedBySystem);
        }
    }
}