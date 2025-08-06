namespace TestBucket.Domain.Identity.Permissions
{
    /// <summary>
    /// Types or groups of entities that can be protected with permission access levels
    /// If adding an entity, make sure to update:
    /// 1. PermissionClaimSerializer
    /// 2. ManageRoles.razor
    /// 3. EntityPermissionBuilder.AddAllPermissions() 
    /// 4. MigrationServices adding default roles
    /// </summary>
    public enum PermissionEntityType
    {
        Tenant,
        User,
        Project,
        Team,

        RequirementSpecification,
        Requirement,

        TestSuite,
        TestCase,

        TestRun,
        TestCaseRun,

        /// <summary>
        /// Accounts & credentials for testing
        /// </summary>
        TestAccount,

        /// <summary>
        /// Test resources / shared devices
        /// </summary>
        TestResource,

        /// <summary>
        /// Test runners
        /// </summary>
        Runner,

        /// <summary>
        /// Features/components/systems/layers and commits
        /// </summary>
        Architecture,

        Issue,

        Heuristic,

        Dashboard,

        /// <summary>
        /// Configure MCP servers on the server
        /// </summary>
        McpServer,
    }
}
