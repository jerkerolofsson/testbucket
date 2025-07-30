using NSubstitute;
using System.Security.Claims;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Insights;
using TestBucket.Domain.Insights.Model;

namespace TestBucket.Domain.UnitTests.Insights
{
    /// <summary>
    /// Unit tests for the <see cref="DashboardManager"/> class.
    /// Tests dashboard management operations including creation, retrieval, updating, deletion, and permission enforcement.
    /// </summary>
    [Component("Insights")]
    [Feature("Insights")]
    [UnitTest]
    [EnrichedTest]
    public class DashboardManagerTests
    {
        /// <summary>
        /// Mocked dashboard repository for testing.
        /// </summary>
        private readonly IDashboardRepository _mockRepository;

        /// <summary>
        /// Mocked time provider for controlling time-related operations in tests.
        /// </summary>
        private readonly TimeProvider _mockTimeProvider;

        /// <summary>
        /// The dashboard manager instance under test.
        /// </summary>
        private readonly DashboardManager _dashboardManager;

        /// <summary>
        /// Fixed test time used across all tests.
        /// </summary>
        private readonly DateTimeOffset _testTime = new(2024, 1, 15, 10, 30, 0, TimeSpan.Zero);

        /// <summary>
        /// Test project ID used in tests.
        /// </summary>
        private const long TestProjectId = 123L;

        /// <summary>
        /// Test tenant ID used in tests.
        /// </summary>
        private const string TestTenantId = "test-tenant";

        /// <summary>
        /// Test user name used in tests.
        /// </summary>
        private const string TestUserName = "testuser@example.com";

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardManagerTests"/> class.
        /// Sets up mock dependencies and the dashboard manager instance for testing.
        /// </summary>
        public DashboardManagerTests()
        {
            _mockRepository = Substitute.For<IDashboardRepository>();
            _mockTimeProvider = Substitute.For<TimeProvider>();
            _mockTimeProvider.GetUtcNow().Returns(_testTime);
            _dashboardManager = new DashboardManager(_mockRepository, _mockTimeProvider);
        }

        #region Constructor Tests

        /// <summary>
        /// Verifies that the constructor throws an <see cref="ArgumentNullException"/> when repository is null.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DashboardManager(null!, _mockTimeProvider));
        }

        /// <summary>
        /// Verifies that the constructor throws an <see cref="ArgumentNullException"/> when time provider is null.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public void Constructor_WithNullTimeProvider_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DashboardManager(_mockRepository, null!));
        }

        #endregion

        #region GetDashboardByNameAsync Tests

        /// <summary>
        /// Verifies that <see cref="DashboardManager.GetDashboardByNameAsync"/> throws <see cref="UnauthorizedAccessException"/> 
        /// when the principal lacks read permission.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task GetDashboardByNameAsync_WithoutReadPermission_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var principal = CreatePrincipal(b => b.Add(PermissionEntityType.Dashboard, PermissionLevel.None));

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _dashboardManager.GetDashboardByNameAsync(principal, TestProjectId, "test-dashboard"));
        }

        /// <summary>
        /// Verifies that <see cref="DashboardManager.GetDashboardByNameAsync"/> returns existing dashboard when found in repository.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task GetDashboardByNameAsync_WithExistingDashboard_ReturnsExistingDashboard()
        {
            // Arrange
            var principal = CreateValidPrincipal();
            var existingDashboard = CreateTestDashboard("test-dashboard");
            _mockRepository.GetDashboardByNameAsync(TestTenantId, TestProjectId, "test-dashboard")
                .Returns(existingDashboard);

            // Act
            var result = await _dashboardManager.GetDashboardByNameAsync(principal, TestProjectId, "test-dashboard");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingDashboard.Name, result.Name);
            Assert.Equal(existingDashboard.Id, result.Id);
        }

        /// <summary>
        /// Verifies that <see cref="DashboardManager.GetDashboardByNameAsync"/> creates and returns default issues dashboard 
        /// when requested dashboard doesn't exist and name is "issues".
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task GetDashboardByNameAsync_WithNonExistentIssuesDashboard_CreatesAndReturnsDefaultIssuesDashboard()
        {
            // Arrange
            var principal = CreateValidPrincipal();
            _mockRepository.GetDashboardByNameAsync(TestTenantId, TestProjectId, "issues").Returns((Dashboard?)null);

            // Act
            var result = await _dashboardManager.GetDashboardByNameAsync(principal, TestProjectId, "issues");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("issues", result.Name);
            Assert.Equal(TestProjectId, result.TestProjectId);
            Assert.NotEmpty(result.Specifications!);
            await _mockRepository.Received(1).AddDashboardAsync(Arg.Any<Dashboard>());
        }

        /// <summary>
        /// Verifies that <see cref="DashboardManager.GetDashboardByNameAsync"/> creates and returns default test results dashboard 
        /// when requested dashboard doesn't exist and name is "testrun".
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task GetDashboardByNameAsync_WithNonExistentTestrunDashboard_CreatesAndReturnsDefaultTestrunDashboard()
        {
            // Arrange
            var principal = CreateValidPrincipal();
            _mockRepository.GetDashboardByNameAsync(TestTenantId, TestProjectId, "testrun").Returns((Dashboard?)null);

            // Act
            var result = await _dashboardManager.GetDashboardByNameAsync(principal, TestProjectId, "testrun");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testrun", result.Name);
            Assert.Equal(TestProjectId, result.TestProjectId);
            Assert.NotEmpty(result.Specifications!);
            await _mockRepository.Received(1).AddDashboardAsync(Arg.Any<Dashboard>());
        }

        /// <summary>
        /// Verifies that <see cref="DashboardManager.GetDashboardByNameAsync"/> creates and returns default test results dashboard 
        /// when requested dashboard doesn't exist and name is "reporting".
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task GetDashboardByNameAsync_WithNonExistentReportingDashboard_CreatesAndReturnsDefaultReportingDashboard()
        {
            // Arrange
            var principal = CreateValidPrincipal();
            _mockRepository.GetDashboardByNameAsync(TestTenantId, TestProjectId, "reporting").Returns((Dashboard?)null);

            // Act
            var result = await _dashboardManager.GetDashboardByNameAsync(principal, TestProjectId, "reporting");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("reporting", result.Name);
            Assert.Equal(TestProjectId, result.TestProjectId);
            Assert.NotEmpty(result.Specifications!);
            await _mockRepository.Received(1).AddDashboardAsync(Arg.Any<Dashboard>());
        }

        /// <summary>
        /// Verifies that <see cref="DashboardManager.GetDashboardByNameAsync"/> returns null 
        /// when requested dashboard doesn't exist and name is not a known default dashboard type.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task GetDashboardByNameAsync_WithNonExistentCustomDashboard_ReturnsNull()
        {
            // Arrange
            var principal = CreateValidPrincipal();
            _mockRepository.GetDashboardByNameAsync(TestTenantId, TestProjectId, "custom-dashboard").Returns((Dashboard?)null);

            // Act
            var result = await _dashboardManager.GetDashboardByNameAsync(principal, TestProjectId, "custom-dashboard");

            // Assert
            Assert.Null(result);
            await _mockRepository.DidNotReceive().AddDashboardAsync(Arg.Any<Dashboard>());
        }

        #endregion

        #region GetDashboardAsync Tests

        /// <summary>
        /// Verifies that <see cref="DashboardManager.GetDashboardAsync"/> throws <see cref="UnauthorizedAccessException"/> 
        /// when the principal lacks read permission.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task GetDashboardAsync_WithoutReadPermission_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var principal = CreatePrincipal(b => b.Add(PermissionEntityType.Dashboard, PermissionLevel.None));

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _dashboardManager.GetDashboardAsync(principal, 1L));
        }

        /// <summary>
        /// Verifies that <see cref="DashboardManager.GetDashboardAsync"/> returns dashboard when found.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task GetDashboardAsync_WithValidId_ReturnsDashboard()
        {
            // Arrange
            var principal = CreateValidPrincipal();
            var dashboard = CreateTestDashboard("test-dashboard");
            _mockRepository.GetDashboardAsync(TestTenantId, dashboard.Id).Returns(dashboard);

            // Act
            var result = await _dashboardManager.GetDashboardAsync(principal, dashboard.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dashboard.Name, result.Name);
            Assert.Equal(dashboard.Id, result.Id);
        }

        /// <summary>
        /// Verifies that <see cref="DashboardManager.GetDashboardAsync"/> returns null when dashboard not found.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task GetDashboardAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var principal = CreateValidPrincipal();
            _mockRepository.GetDashboardAsync(TestTenantId, 999L).Returns((Dashboard?)null);

            // Act
            var result = await _dashboardManager.GetDashboardAsync(principal, 999L);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetAllDashboardsAsync Tests

        /// <summary>
        /// Verifies that <see cref="DashboardManager.GetAllDashboardsAsync"/> throws <see cref="UnauthorizedAccessException"/> 
        /// when the principal lacks read permission.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task GetAllDashboardsAsync_WithoutReadPermission_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var principal = CreatePrincipal(b => b.Add(PermissionEntityType.Dashboard, PermissionLevel.None));

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _dashboardManager.GetAllDashboardsAsync(principal, TestProjectId));
        }

        /// <summary>
        /// Verifies that <see cref="DashboardManager.GetAllDashboardsAsync"/> returns existing dashboards when they exist.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task GetAllDashboardsAsync_WithExistingDashboards_ReturnsExistingDashboards()
        {
            // Arrange
            var principal = CreateValidPrincipal();
            var dashboards = new[]
            {
                CreateTestDashboard("dashboard1"),
                CreateTestDashboard("dashboard2")
            };
            _mockRepository.GetAllDashboardsAsync(TestProjectId).Returns(dashboards);

            // Act
            var result = await _dashboardManager.GetAllDashboardsAsync(principal, TestProjectId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, d => d.Name == "dashboard1");
            Assert.Contains(result, d => d.Name == "dashboard2");
        }

        /// <summary>
        /// Verifies that <see cref="DashboardManager.GetAllDashboardsAsync"/> creates default dashboards 
        /// when no dashboards exist for the project.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task GetAllDashboardsAsync_WithNoDashboards_CreatesDefaultDashboards()
        {
            // Arrange
            var principal = CreateValidPrincipal();
            _mockRepository.GetAllDashboardsAsync(TestProjectId).Returns(Array.Empty<Dashboard>());
            _mockRepository.GetDashboardByNameAsync(TestTenantId, TestProjectId, "issues").Returns((Dashboard?)null);
            _mockRepository.GetDashboardByNameAsync(TestTenantId, TestProjectId, "testrun").Returns((Dashboard?)null);

            // Act
            var result = await _dashboardManager.GetAllDashboardsAsync(principal, TestProjectId);

            // Assert
            Assert.NotNull(result);
            // Should have called to create default dashboards
            await _mockRepository.Received().GetDashboardByNameAsync(TestTenantId, TestProjectId, "issues");
            await _mockRepository.Received().GetDashboardByNameAsync(TestTenantId, TestProjectId, "testrun");
        }

        #endregion

        #region AddDashboardAsync Tests

        /// <summary>
        /// Verifies that <see cref="DashboardManager.AddDashboardAsync"/> throws <see cref="UnauthorizedAccessException"/> 
        /// when the principal lacks write permission.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task AddDashboardAsync_WithoutWritePermission_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var principal = CreatePrincipal(b =>
            {
                b.UserName = TestUserName;
                b.Email = TestUserName;
                b.Add(PermissionEntityType.Dashboard, PermissionLevel.Read);
            });
            var dashboard = CreateTestDashboard("new-dashboard");

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _dashboardManager.AddDashboardAsync(principal, dashboard));
        }


        /// <summary>
        /// Verifies that <see cref="DashboardManager.AddDashboardAsync"/> successfully adds dashboard with correct metadata.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task AddDashboardAsync_WithValidDashboard_AddsDashboardWithCorrectMetadata()
        {
            // Arrange
            var principal = CreateValidPrincipal();
            var dashboard = CreateTestDashboard("new-dashboard");

            // Act
            await _dashboardManager.AddDashboardAsync(principal, dashboard);

            // Assert
            await _mockRepository.Received(1).AddDashboardAsync(Arg.Is<Dashboard>(d =>
                d.TenantId == TestTenantId &&
                d.Created == _testTime &&
                d.Modified == _testTime &&
                d.CreatedBy == TestUserName &&
                d.ModifiedBy == TestUserName));
        }

        #endregion

        #region UpdateDashboardAsync Tests

        /// <summary>
        /// Verifies that <see cref="DashboardManager.UpdateDashboardAsync"/> throws <see cref="UnauthorizedAccessException"/> 
        /// when the principal lacks write permission.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task UpdateDashboardAsync_WithoutWritePermission_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var principal = CreatePrincipal(b => b.Add(PermissionEntityType.Dashboard, PermissionLevel.Read));
            var dashboard = CreateTestDashboard("existing-dashboard");

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _dashboardManager.UpdateDashboardAsync(principal, dashboard));
        }

        /// <summary>
        /// Verifies that <see cref="DashboardManager.UpdateDashboardAsync"/> successfully updates dashboard with correct metadata.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task UpdateDashboardAsync_WithValidDashboard_UpdatesDashboardWithCorrectMetadata()
        {
            // Arrange
            var principal = CreateValidPrincipal();
            var dashboard = CreateTestDashboard("existing-dashboard");

            // Act
            await _dashboardManager.UpdateDashboardAsync(principal, dashboard);

            // Assert
            await _mockRepository.Received(1).UpdateDashboardAsync(Arg.Is<Dashboard>(d =>
                d.TenantId == TestTenantId &&
                d.Modified == _testTime &&
                d.ModifiedBy == TestUserName));
        }

        #endregion

        #region DeleteDashboardAsync Tests

        /// <summary>
        /// Verifies that <see cref="DashboardManager.DeleteDashboardAsync"/> throws <see cref="UnauthorizedAccessException"/> 
        /// when the principal lacks delete permission.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task DeleteDashboardAsync_WithoutDeletePermission_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var principal = CreatePrincipal(b => b.Add(PermissionEntityType.Dashboard, PermissionLevel.ReadWrite));

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _dashboardManager.DeleteDashboardAsync(principal, 1L));
        }

        /// <summary>
        /// Verifies that <see cref="DashboardManager.DeleteDashboardAsync"/> successfully deletes dashboard.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task DeleteDashboardAsync_WithValidId_DeletesDashboard()
        {
            // Arrange
            var principal = CreateValidPrincipal();
            const long dashboardId = 123L;

            // Act
            await _dashboardManager.DeleteDashboardAsync(principal, dashboardId);

            // Assert
            await _mockRepository.Received(1).DeleteDashboardAsync(TestTenantId, dashboardId);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a <see cref="ClaimsPrincipal"/> with the specified permission configuration.
        /// </summary>
        /// <param name="configure">Action to configure permissions using <see cref="EntityPermissionBuilder"/>.</param>
        /// <returns>A <see cref="ClaimsPrincipal"/> with the specified permissions.</returns>
        private static ClaimsPrincipal CreatePrincipal(Action<EntityPermissionBuilder> configure)
        {
            return Impersonation.Impersonate(configure);
        }

        /// <summary>
        /// Creates a valid <see cref="ClaimsPrincipal"/> with all dashboard permissions.
        /// </summary>
        /// <returns>A <see cref="ClaimsPrincipal"/> with full dashboard permissions.</returns>
        private static ClaimsPrincipal CreateValidPrincipal()
        {
            return CreatePrincipal(b =>
            {
                b.UserName = TestUserName;
                b.TenantId = TestTenantId;
                b.Add(PermissionEntityType.Dashboard, PermissionLevel.All);
            });
        }

        /// <summary>
        /// Creates a test dashboard with the specified name.
        /// </summary>
        /// <param name="name">The name of the dashboard to create.</param>
        /// <returns>A <see cref="Dashboard"/> instance for testing.</returns>
        private static Dashboard CreateTestDashboard(string name)
        {
            return new Dashboard
            {
                Id = Random.Shared.NextInt64(1, 1000),
                Name = name,
                TenantId = TestTenantId,
                TestProjectId = TestProjectId,
                Specifications = []
            };
        }

        #endregion
    }
}
