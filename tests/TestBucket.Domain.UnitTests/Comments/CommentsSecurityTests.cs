using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Comments;
using TestBucket.Domain.Comments.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;

namespace TestBucket.Domain.UnitTests.Comments
{
    /// <summary>
    /// Permission tests
    /// </summary>
    [UnitTest]
    [SecurityTest]
    [EnrichedTest]
    [Component("Comments")]
    public class CommentsSecurityTests
    {
        /// <summary>
        /// Verifies that an exception is not thrown if trying to add a comment without test-suite write permission
        /// </summary>
        [Fact]
        public void AddComment_WithTestRunWritePermission_Success()
        {
            // Arrange
            var comment = new Comment { TestRunId = 1 };
            var user = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "abcd";
                builder.Add(PermissionEntityType.TestRun, PermissionLevel.Write);
            });

            // Act
            CommentsManager.ThrowIfNoPermission(user, comment, PermissionLevel.Write);
        }

        /// <summary>
        /// Verifies that an exception is not thrown if trying to add a comment without test-run write permission
        /// </summary>
        [Fact]
        public void AddComment_WithoutTestRunWritePermission_Success()
        {
            // Arrange
            var comment = new Comment { TestRunId = 1 };
            var user = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "abcd";
                builder.Add(PermissionEntityType.TestRun, PermissionLevel.Read);
            });

            // Act
            Assert.Throws<UnauthorizedAccessException>(() =>
            {
                CommentsManager.ThrowIfNoPermission(user, comment, PermissionLevel.Write);
            });
        }


        /// <summary>
        /// Verifies that an exception is not thrown if trying to add a comment without test-suite write permission
        /// </summary>
        [Fact]
        public void AddComment_WithTestSuiteWritePermission_Success()
        {
            // Arrange
            var comment = new Comment { TestSuiteId = 1 };
            var user = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "abcd";
                builder.Add(PermissionEntityType.TestSuite, PermissionLevel.Write);
            });

            // Act
            CommentsManager.ThrowIfNoPermission(user, comment, PermissionLevel.Write);
        }

        /// <summary>
        /// Verifies that an exception is not thrown if trying to add a comment without test-suite write permission
        /// </summary>
        [Fact]
        public void AddComment_WithoutTestSuiteWritePermission_Success()
        {
            // Arrange
            var comment = new Comment { TestSuiteId = 1 };
            var user = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "abcd";
                builder.Add(PermissionEntityType.TestSuite, PermissionLevel.Read);
            });

            // Act
            Assert.Throws<UnauthorizedAccessException>(() =>
            {
                CommentsManager.ThrowIfNoPermission(user, comment, PermissionLevel.Write);
            });
        }

        /// <summary>
        /// Verifies that an exception is not thrown if trying to add a comment without test-case write permission
        /// </summary>
        [Fact]
        public void AddComment_WithTestCaseWritePermission_Success()
        {
            // Arrange
            var comment = new Comment { TestCaseId = 1 };
            var user = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "abcd";
                builder.Add(PermissionEntityType.TestCase, PermissionLevel.Write);
            });

            // Act
            CommentsManager.ThrowIfNoPermission(user, comment, PermissionLevel.Write);
        }

        /// <summary>
        /// Verifies that an exception is not thrown if trying to add a comment without test-case write permission
        /// </summary>
        [Fact]
        public void AddComment_WithoutTestCaseWritePermission_Success()
        {
            // Arrange
            var comment = new Comment { TestCaseId = 1 };
            var user = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "abcd";
                builder.Add(PermissionEntityType.TestCase, PermissionLevel.Read);
            });

            // Act
            Assert.Throws<UnauthorizedAccessException>(() =>
            {
                CommentsManager.ThrowIfNoPermission(user, comment, PermissionLevel.Write);
            });
        }

        /// <summary>
        /// Verifies that an exception is not thrown if trying to add a comment without requirement write permission
        /// </summary>
        [Fact]
        public void AddComment_WithRequirementWritePermission_Success()
        {
            // Arrange
            var comment = new Comment { RequirementId = 1 };
            var user = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "abcd";
                builder.Add(PermissionEntityType.Requirement, PermissionLevel.Write);
            });

            // Act
            CommentsManager.ThrowIfNoPermission(user, comment, PermissionLevel.Write);
        }

        /// <summary>
        /// Verifies that an exception is not thrown if trying to delete a comment without requirement delete permission
        /// </summary>
        [Fact]
        public void DeleteComment_WithRequirementDeletePermission_Success()
        {
            // Arrange
            var comment = new Comment { RequirementId = 1 };
            var user = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "abcd";
                builder.Add(PermissionEntityType.Requirement, PermissionLevel.Write);
                builder.Add(Domain.Identity.Permissions.PermissionEntityType.Requirement, PermissionLevel.Delete);
            });

            // Act
            CommentsManager.ThrowIfNoPermission(user, comment, PermissionLevel.Delete);
        }

        /// <summary>
        /// Verifies that an exception is thrown if trying to add a comment without requirement write permission
        /// </summary>
        [Fact]
        public void AddComment_WithoutRequirementWritePermission_UnauthorizedAccessExceptionThrown()
        {
            // Arrange
            var comment = new Comment { RequirementId = 1 };
            var user = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "abcd";
            });

            // Assert
            Assert.Throws<UnauthorizedAccessException>(() =>
            {
                CommentsManager.ThrowIfNoPermission(user, comment, PermissionLevel.Write);
            });
        }

        /// <summary>
        /// Verifies that an exception is thrown if trying to add a comment without requirement delete permission
        /// </summary>
        [Fact]
        public void DeleteComment_WithoutRequirementDeletePermission_UnauthorizedAccessExceptionThrown()
        {
            // Arrange
            var comment = new Comment { RequirementId = 1 };
            var user = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "abcd";
            });

            // Assert
            Assert.Throws<UnauthorizedAccessException>(() =>
            {
                CommentsManager.ThrowIfNoPermission(user, comment, PermissionLevel.Delete);
            });
        }
    }
}
