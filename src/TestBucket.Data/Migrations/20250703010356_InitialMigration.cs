using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pgvector;
using TestBucket.Contracts;
using TestBucket.Contracts.Code.Models;
using TestBucket.Contracts.Insights;
using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Keyboard;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,")
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<byte[]>(type: "bytea", nullable: false),
                    Length = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RequirementId = table.Column<long>(type: "bigint", nullable: true),
                    RequirementSpecificationId = table.Column<long>(type: "bigint", nullable: true),
                    TestRunId = table.Column<long>(type: "bigint", nullable: true),
                    TestCaseId = table.Column<long>(type: "bigint", nullable: true),
                    TestCaseRunId = table.Column<long>(type: "bigint", nullable: true),
                    TestSuiteId = table.Column<long>(type: "bigint", nullable: true),
                    TestSuiteFolderId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true),
                    ComponentId = table.Column<long>(type: "bigint", nullable: true),
                    FeatureId = table.Column<long>(type: "bigint", nullable: true),
                    LayerId = table.Column<long>(type: "bigint", nullable: true),
                    SystemId = table.Column<long>(type: "bigint", nullable: true),
                    LocalIssueId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GlobalSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DefaultTenant = table.Column<string>(type: "text", nullable: false),
                    AiProvider = table.Column<string>(type: "text", nullable: false),
                    SymmetricJwtKey = table.Column<string>(type: "text", nullable: true),
                    JwtIssuer = table.Column<string>(type: "text", nullable: true),
                    JwtAudience = table.Column<string>(type: "text", nullable: true),
                    LlmModel = table.Column<string>(type: "text", nullable: false),
                    LlmEmbeddingModel = table.Column<string>(type: "text", nullable: true),
                    LlmClassificationModel = table.Column<string>(type: "text", nullable: true),
                    AiProviderUrl = table.Column<string>(type: "text", nullable: true),
                    GithubModelsDeveloperKey = table.Column<string>(type: "text", nullable: true),
                    AzureAiProductionKey = table.Column<string>(type: "text", nullable: true),
                    Revision = table.Column<int>(type: "integer", nullable: false),
                    PublicEndpointUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    CiCdAccessToken = table.Column<string>(type: "text", nullable: true),
                    CiCdAccessTokenExpires = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CanRegisterNewUsers = table.Column<bool>(type: "boolean", nullable: false),
                    RequireConfirmedAccount = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    ActiveTeamId = table.Column<long>(type: "bigint", nullable: true),
                    ActiveProjectId = table.Column<long>(type: "bigint", nullable: true),
                    KeyboardBindings = table.Column<KeyboardBindings>(type: "jsonb", nullable: true),
                    DarkMode = table.Column<bool>(type: "boolean", nullable: false),
                    ExplorerDock = table.Column<int>(type: "integer", nullable: true),
                    Theme = table.Column<string>(type: "text", nullable: true),
                    IncreasedContrast = table.Column<bool>(type: "boolean", nullable: false),
                    IncreasedFontSize = table.Column<bool>(type: "boolean", nullable: false),
                    PreferTextToIcons = table.Column<bool>(type: "boolean", nullable: false),
                    ShowFailureMessageDialogWhenFailingTestCaseRun = table.Column<bool>(type: "boolean", nullable: false),
                    AdvanceToNextNotCompletedTestWhenSettingResult = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Entity = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ShortName = table.Column<string>(type: "text", nullable: false),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    GrantAccessToAllTenantUsers = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_teams_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TestAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Owner = table.Column<string>(type: "text", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Locked = table.Column<bool>(type: "boolean", nullable: false),
                    LockOwner = table.Column<string>(type: "text", nullable: true),
                    LockExpires = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SubType = table.Column<string>(type: "text", nullable: true),
                    Variables = table.Column<Dictionary<string, string>>(type: "hstore", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestAccounts_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TestResources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ResourceId = table.Column<string>(type: "text", nullable: false),
                    Owner = table.Column<string>(type: "text", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Locked = table.Column<bool>(type: "boolean", nullable: false),
                    LockOwner = table.Column<string>(type: "text", nullable: true),
                    LockExpires = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    Types = table.Column<string[]>(type: "text[]", nullable: false),
                    Manufacturer = table.Column<string>(type: "text", nullable: true),
                    Model = table.Column<string>(type: "text", nullable: true),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Variables = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false),
                    Health = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResources_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ShortName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    GrantAccessToAllTenantUsers = table.Column<bool>(type: "boolean", nullable: false),
                    GrantAccessToAllTeamUsers = table.Column<bool>(type: "boolean", nullable: false),
                    NumberOfTestSuites = table.Column<int>(type: "integer", nullable: false),
                    NumberOfTestCases = table.Column<int>(type: "integer", nullable: false),
                    NumberOfIssues = table.Column<int>(type: "integer", nullable: false),
                    NumberOfOpenIssues = table.Column<int>(type: "integer", nullable: false),
                    NumberOfRuns = table.Column<int>(type: "integer", nullable: false),
                    TestStates = table.Column<TestState[]>(type: "jsonb", nullable: true),
                    RequirementStates = table.Column<RequirementState[]>(type: "jsonb", nullable: true),
                    IssueStates = table.Column<IssueState[]>(type: "jsonb", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_projects_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_projects_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArchitecturalLayers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    GlobPatterns = table.Column<List<string>>(type: "jsonb", nullable: false),
                    Display = table.Column<DisplayOptions>(type: "jsonb", nullable: true),
                    DevResponsible = table.Column<string>(type: "text", nullable: true),
                    DevLead = table.Column<string>(type: "text", nullable: true),
                    TestLead = table.Column<string>(type: "text", nullable: true),
                    Embedding = table.Column<Vector>(type: "vector(384)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchitecturalLayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchitecturalLayers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArchitecturalLayers_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArchitecturalLayers_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    ProfileImageUri = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetUsers_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Components",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    GlobPatterns = table.Column<List<string>>(type: "jsonb", nullable: false),
                    Display = table.Column<DisplayOptions>(type: "jsonb", nullable: true),
                    DevResponsible = table.Column<string>(type: "text", nullable: true),
                    DevLead = table.Column<string>(type: "text", nullable: true),
                    TestLead = table.Column<string>(type: "text", nullable: true),
                    Embedding = table.Column<Vector>(type: "vector(384)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Components", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Components_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Components_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Components_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Dashboards",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Specifications = table.Column<List<InsightsVisualizationSpecification>>(type: "jsonb", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dashboards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dashboards_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Dashboards_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Dashboards_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "external_systems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Provider = table.Column<string>(type: "text", nullable: true),
                    BaseUrl = table.Column<string>(type: "text", nullable: true),
                    ExternalProjectId = table.Column<string>(type: "text", nullable: true),
                    AccessToken = table.Column<string>(type: "text", nullable: true),
                    ApiKey = table.Column<string>(type: "text", nullable: true),
                    ReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    EnabledCapabilities = table.Column<int>(type: "integer", nullable: false),
                    SupportedCapabilities = table.Column<int>(type: "integer", nullable: false),
                    TestResultsArtifactsPattern = table.Column<string>(type: "text", nullable: true),
                    CoverageReportArtifactsPattern = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_external_systems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_external_systems_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_external_systems_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_external_systems_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    GlobPatterns = table.Column<List<string>>(type: "jsonb", nullable: false),
                    Display = table.Column<DisplayOptions>(type: "jsonb", nullable: true),
                    DevResponsible = table.Column<string>(type: "text", nullable: true),
                    DevLead = table.Column<string>(type: "text", nullable: true),
                    TestLead = table.Column<string>(type: "text", nullable: true),
                    Embedding = table.Column<Vector>(type: "vector(384)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Features_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Features_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Features_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "field_definitions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Trait = table.Column<string>(type: "text", nullable: true),
                    TraitType = table.Column<int>(type: "integer", nullable: false),
                    Target = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Inherit = table.Column<bool>(type: "boolean", nullable: false),
                    Options = table.Column<List<string>>(type: "jsonb", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ShowDescription = table.Column<bool>(type: "boolean", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    DataSourceType = table.Column<int>(type: "integer", nullable: false),
                    DataSourceUri = table.Column<string>(type: "text", nullable: true),
                    OptionIcons = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: true),
                    OptionColors = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    WriteOnly = table.Column<bool>(type: "boolean", nullable: false),
                    IsVisible = table.Column<bool>(type: "boolean", nullable: false),
                    UseClassifier = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefinedBySystem = table.Column<bool>(type: "boolean", nullable: false),
                    RequiredPermission = table.Column<int>(type: "integer", nullable: false),
                    Dock = table.Column<int>(type: "integer", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_field_definitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_field_definitions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_field_definitions_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_field_definitions_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "heuristics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_heuristics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_heuristics_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_heuristics_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_heuristics_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Labels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    ExternalSystemName = table.Column<string>(type: "text", nullable: true),
                    ExternalSystemId = table.Column<long>(type: "bigint", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    ReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Labels_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Labels_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Labels_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LocalIssues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: true),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    ExternalSystemName = table.Column<string>(type: "text", nullable: true),
                    ExternalSystemId = table.Column<long>(type: "bigint", nullable: true),
                    ClassificationRequired = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    MappedState = table.Column<int>(type: "integer", nullable: true),
                    Closed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Author = table.Column<string>(type: "text", nullable: true),
                    AssignedTo = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    IssueType = table.Column<string>(type: "text", nullable: true),
                    MappedType = table.Column<int>(type: "integer", nullable: true),
                    ExternalDisplayId = table.Column<string>(type: "text", nullable: true),
                    Embedding = table.Column<Vector>(type: "vector(384)", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalIssues_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LocalIssues_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LocalIssues_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Milestones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    ExternalSystemName = table.Column<string>(type: "text", nullable: true),
                    ExternalSystemId = table.Column<long>(type: "bigint", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Milestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Milestones_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Milestones_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Milestones_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductSystems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    GlobPatterns = table.Column<List<string>>(type: "jsonb", nullable: false),
                    Display = table.Column<DisplayOptions>(type: "jsonb", nullable: true),
                    DevResponsible = table.Column<string>(type: "text", nullable: true),
                    DevLead = table.Column<string>(type: "text", nullable: true),
                    TestLead = table.Column<string>(type: "text", nullable: true),
                    Embedding = table.Column<Vector>(type: "vector(384)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSystems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSystems_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductSystems_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductSystems_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Runners",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Languages = table.Column<string[]>(type: "jsonb", nullable: true),
                    Tags = table.Column<string[]>(type: "jsonb", nullable: false),
                    PublicBaseUrl = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Runners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Runners_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Runners_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Runners_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "spec",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    ExternalProvider = table.Column<string>(type: "text", nullable: true),
                    FileResourceId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    ReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    SearchFolders = table.Column<List<SearchFolder>>(type: "jsonb", nullable: true),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    SpecificationType = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_spec", x => x.Id);
                    table.ForeignKey(
                        name: "FK_spec_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_spec_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_spec_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TestEnvironments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Variables = table.Column<Dictionary<string, string>>(type: "hstore", nullable: false),
                    Dependencies = table.Column<List<TestCaseDependency>>(type: "jsonb", nullable: true),
                    Default = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestEnvironments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestEnvironments_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TestEnvironments_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TestEnvironments_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "testlab__folders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: true),
                    PathIds = table.Column<long[]>(type: "bigint[]", nullable: true),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testlab__folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_testlab__folders_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testlab__folders_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testlab__folders_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testlab__folders_testlab__folders_ParentId",
                        column: x => x.ParentId,
                        principalTable: "testlab__folders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "testrepository__folders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: true),
                    PathIds = table.Column<long[]>(type: "bigint[]", nullable: true),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testrepository__folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_testrepository__folders_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testrepository__folders_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testrepository__folders_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testrepository__folders_testrepository__folders_ParentId",
                        column: x => x.ParentId,
                        principalTable: "testrepository__folders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Expiry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiKeys_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApiKeys_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectUserPermissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Entity = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUserPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectUserPermissions_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectUserPermissions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectUserPermissions_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectUserPermissions_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Repositories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Url = table.Column<string>(type: "text", nullable: false),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    LastIndexTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExternalSystemId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repositories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repositories_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Repositories_external_systems_ExternalSystemId",
                        column: x => x.ExternalSystemId,
                        principalTable: "external_systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Repositories_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Repositories_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "issue_fields",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LocalIssueId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Inherited = table.Column<bool>(type: "boolean", nullable: true),
                    BooleanValue = table.Column<bool>(type: "boolean", nullable: true),
                    LongValue = table.Column<long>(type: "bigint", nullable: true),
                    DoubleValue = table.Column<double>(type: "double precision", nullable: true),
                    StringValue = table.Column<string>(type: "text", nullable: true),
                    DateValue = table.Column<DateOnly>(type: "date", nullable: true),
                    TimeSpanValue = table.Column<TimeSpan>(type: "interval", nullable: true),
                    DateTimeOffsetValue = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    StringValuesList = table.Column<List<string>>(type: "jsonb", nullable: true),
                    FieldDefinitionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_issue_fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_issue_fields_LocalIssues_LocalIssueId",
                        column: x => x.LocalIssueId,
                        principalTable: "LocalIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_issue_fields_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_issue_fields_field_definitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalTable: "field_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "spec__folders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: true),
                    PathIds = table.Column<long[]>(type: "bigint[]", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    RequirementSpecificationId = table.Column<long>(type: "bigint", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_spec__folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_spec__folders_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_spec__folders_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_spec__folders_spec_RequirementSpecificationId",
                        column: x => x.RequirementSpecificationId,
                        principalTable: "spec",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_spec__folders_spec__folders_ParentId",
                        column: x => x.ParentId,
                        principalTable: "spec__folders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_spec__folders_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "runs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    SystemOut = table.Column<string>(type: "text", nullable: true),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    CiCdRef = table.Column<string>(type: "text", nullable: true),
                    CiCdSystem = table.Column<string>(type: "text", nullable: true),
                    ExternalSystemId = table.Column<long>(type: "bigint", nullable: true),
                    Open = table.Column<bool>(type: "boolean", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    TestEnvironmentId = table.Column<long>(type: "bigint", nullable: true),
                    FolderId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_runs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_runs_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_runs_TestEnvironments_TestEnvironmentId",
                        column: x => x.TestEnvironmentId,
                        principalTable: "TestEnvironments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_runs_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_runs_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_runs_testlab__folders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "testlab__folders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "testsuites",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    Variables = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: true),
                    Dependencies = table.Column<List<TestCaseDependency>>(type: "jsonb", nullable: true),
                    DefaultCiCdRef = table.Column<string>(type: "text", nullable: true),
                    CiCdWorkflow = table.Column<string>(type: "text", nullable: true),
                    AddPipelinesStartedFromOutside = table.Column<bool>(type: "boolean", nullable: true),
                    CiCdSystem = table.Column<string>(type: "text", nullable: true),
                    ExternalSystemId = table.Column<long>(type: "bigint", nullable: true),
                    FolderId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testsuites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_testsuites_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testsuites_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testsuites_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testsuites_testrepository__folders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "testrepository__folders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Commits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Message = table.Column<string>(type: "text", nullable: true),
                    ShortDescription = table.Column<string>(type: "text", nullable: true),
                    LongDescription = table.Column<string>(type: "text", nullable: true),
                    Commited = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CommitedBy = table.Column<string>(type: "text", nullable: true),
                    Reference = table.Column<string>(type: "text", nullable: false),
                    Sha = table.Column<string>(type: "text", nullable: false),
                    RepositoryId = table.Column<long>(type: "bigint", nullable: false),
                    SystemNames = table.Column<List<string>>(type: "text[]", nullable: true),
                    FeatureNames = table.Column<List<string>>(type: "text[]", nullable: true),
                    ComponentNames = table.Column<List<string>>(type: "text[]", nullable: true),
                    LayerNames = table.Column<List<string>>(type: "text[]", nullable: true),
                    References = table.Column<List<string>>(type: "text[]", nullable: true),
                    Fixes = table.Column<List<string>>(type: "text[]", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Commits_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "Repositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Commits_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Commits_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Commits_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "requirements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: true),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    ExternalProvider = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Path = table.Column<string>(type: "text", nullable: false),
                    PathIds = table.Column<long[]>(type: "bigint[]", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    MappedState = table.Column<int>(type: "integer", nullable: true),
                    RootRequirementId = table.Column<long>(type: "bigint", nullable: true),
                    ParentRequirementId = table.Column<long>(type: "bigint", nullable: true),
                    ReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    RequirementType = table.Column<string>(type: "text", nullable: true),
                    AssignedTo = table.Column<string>(type: "text", nullable: true),
                    MappedType = table.Column<int>(type: "integer", nullable: true),
                    RequirementSpecificationId = table.Column<long>(type: "bigint", nullable: false),
                    RequirementSpecificationFolderId = table.Column<long>(type: "bigint", nullable: true),
                    Progress = table.Column<double>(type: "double precision", nullable: true),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DueDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_requirements_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_requirements_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_requirements_spec_RequirementSpecificationId",
                        column: x => x.RequirementSpecificationId,
                        principalTable: "spec",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_requirements_spec__folders_RequirementSpecificationFolderId",
                        column: x => x.RequirementSpecificationFolderId,
                        principalTable: "spec__folders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_requirements_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Guid = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Script = table.Column<string>(type: "text", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false),
                    EnvironmentVariables = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    StdOut = table.Column<string>(type: "text", nullable: true),
                    StdErr = table.Column<string>(type: "text", nullable: true),
                    Result = table.Column<string>(type: "text", nullable: true),
                    Format = table.Column<int>(type: "integer", nullable: true),
                    ArtifactContent = table.Column<Dictionary<string, byte[]>>(type: "json", nullable: true),
                    TestRunId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Jobs_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Jobs_runs_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "runs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Jobs_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Pipelines",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Guid = table.Column<string>(type: "text", nullable: true),
                    DisplayTitle = table.Column<string>(type: "text", nullable: true),
                    HeadCommit = table.Column<string>(type: "text", nullable: true),
                    CiCdPipelineIdentifier = table.Column<string>(type: "text", nullable: true),
                    CiCdSystem = table.Column<string>(type: "text", nullable: true),
                    CiCdProjectId = table.Column<string>(type: "text", nullable: true),
                    StartError = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    WebUrl = table.Column<string>(type: "text", nullable: true),
                    TestRunId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pipelines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pipelines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pipelines_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pipelines_runs_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "runs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pipelines_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "test_run_fields",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestRunId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Inherited = table.Column<bool>(type: "boolean", nullable: true),
                    BooleanValue = table.Column<bool>(type: "boolean", nullable: true),
                    LongValue = table.Column<long>(type: "bigint", nullable: true),
                    DoubleValue = table.Column<double>(type: "double precision", nullable: true),
                    StringValue = table.Column<string>(type: "text", nullable: true),
                    DateValue = table.Column<DateOnly>(type: "date", nullable: true),
                    TimeSpanValue = table.Column<TimeSpan>(type: "interval", nullable: true),
                    DateTimeOffsetValue = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    StringValuesList = table.Column<List<string>>(type: "jsonb", nullable: true),
                    FieldDefinitionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_run_fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_test_run_fields_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_test_run_fields_field_definitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalTable: "field_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_test_run_fields_runs_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "testsuite__folders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: true),
                    PathIds = table.Column<long[]>(type: "bigint[]", nullable: true),
                    IsFeature = table.Column<bool>(type: "boolean", nullable: false),
                    FeatureDescription = table.Column<string>(type: "text", nullable: true),
                    TestSuiteId = table.Column<long>(type: "bigint", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testsuite__folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_testsuite__folders_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testsuite__folders_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testsuite__folders_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testsuite__folders_testsuite__folders_ParentId",
                        column: x => x.ParentId,
                        principalTable: "testsuite__folders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testsuite__folders_testsuites_TestSuiteId",
                        column: x => x.TestSuiteId,
                        principalTable: "testsuites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommitFiless",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Path = table.Column<string>(type: "text", nullable: false),
                    Sha = table.Column<string>(type: "text", nullable: false),
                    Additions = table.Column<int>(type: "integer", nullable: false),
                    Deletions = table.Column<int>(type: "integer", nullable: false),
                    Changes = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: true),
                    CommitId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitFiless", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommitFiless_Commits_CommitId",
                        column: x => x.CommitId,
                        principalTable: "Commits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommitFiless_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommitFiless_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommitFiless_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "requirement_fields",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequirementId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Inherited = table.Column<bool>(type: "boolean", nullable: true),
                    BooleanValue = table.Column<bool>(type: "boolean", nullable: true),
                    LongValue = table.Column<long>(type: "bigint", nullable: true),
                    DoubleValue = table.Column<double>(type: "double precision", nullable: true),
                    StringValue = table.Column<string>(type: "text", nullable: true),
                    DateValue = table.Column<DateOnly>(type: "date", nullable: true),
                    TimeSpanValue = table.Column<TimeSpan>(type: "interval", nullable: true),
                    DateTimeOffsetValue = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    StringValuesList = table.Column<List<string>>(type: "jsonb", nullable: true),
                    FieldDefinitionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requirement_fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_requirement_fields_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_requirement_fields_field_definitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalTable: "field_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_requirement_fields_requirements_RequirementId",
                        column: x => x.RequirementId,
                        principalTable: "requirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PipelineJobs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CiCdJobIdentifier = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FinishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Stage = table.Column<string>(type: "text", nullable: true),
                    Coverage = table.Column<double>(type: "double precision", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    AllowFailure = table.Column<bool>(type: "boolean", nullable: true),
                    WebUrl = table.Column<string>(type: "text", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    QueuedDuration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    TagList = table.Column<string[]>(type: "jsonb", nullable: true),
                    FailureReason = table.Column<string>(type: "text", nullable: true),
                    PipelineId = table.Column<long>(type: "bigint", nullable: false),
                    TestRunId = table.Column<long>(type: "bigint", nullable: true),
                    HasArtifacts = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PipelineJobs_Pipelines_PipelineId",
                        column: x => x.PipelineId,
                        principalTable: "Pipelines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PipelineJobs_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PipelineJobs_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PipelineJobs_runs_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "runs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PipelineJobs_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "testcases",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: true),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    ExternalDisplayId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ClassName = table.Column<string>(type: "text", nullable: true),
                    Module = table.Column<string>(type: "text", nullable: true),
                    SessionDuration = table.Column<int>(type: "integer", nullable: true),
                    Method = table.Column<string>(type: "text", nullable: true),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Preconditions = table.Column<string>(type: "text", nullable: true),
                    Postconditions = table.Column<string>(type: "text", nullable: true),
                    ScriptType = table.Column<int>(type: "integer", nullable: false),
                    ExecutionType = table.Column<int>(type: "integer", nullable: false),
                    RunnerLanguage = table.Column<string>(type: "text", nullable: true),
                    AutomationAssembly = table.Column<string>(type: "text", nullable: true),
                    Path = table.Column<string>(type: "text", nullable: false),
                    PathIds = table.Column<long[]>(type: "bigint[]", nullable: true),
                    TestSuiteId = table.Column<long>(type: "bigint", nullable: false),
                    TestSuiteFolderId = table.Column<long>(type: "bigint", nullable: true),
                    ClassificationRequired = table.Column<bool>(type: "boolean", nullable: false),
                    IsTemplate = table.Column<bool>(type: "boolean", nullable: false),
                    TestParameters = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: true),
                    Dependencies = table.Column<List<TestCaseDependency>>(type: "jsonb", nullable: true),
                    Embedding = table.Column<Vector>(type: "vector(384)", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testcases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_testcases_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testcases_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testcases_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testcases_testsuite__folders_TestSuiteFolderId",
                        column: x => x.TestSuiteFolderId,
                        principalTable: "testsuite__folders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "requirement_test_links",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequirementId = table.Column<long>(type: "bigint", nullable: false),
                    RequirementExternalId = table.Column<string>(type: "text", nullable: true),
                    TestCaseId = table.Column<long>(type: "bigint", nullable: false),
                    RequirementSpecificationId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requirement_test_links", x => x.Id);
                    table.ForeignKey(
                        name: "FK_requirement_test_links_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_requirement_test_links_requirements_RequirementId",
                        column: x => x.RequirementId,
                        principalTable: "requirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_requirement_test_links_spec_RequirementSpecificationId",
                        column: x => x.RequirementSpecificationId,
                        principalTable: "spec",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_requirement_test_links_testcases_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "testcases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "steps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ExpectedResult = table.Column<string>(type: "text", nullable: true),
                    TestCaseId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_steps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_steps_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_steps_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_steps_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_steps_testcases_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "testcases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "test_case_fields",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestCaseId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Inherited = table.Column<bool>(type: "boolean", nullable: true),
                    BooleanValue = table.Column<bool>(type: "boolean", nullable: true),
                    LongValue = table.Column<long>(type: "bigint", nullable: true),
                    DoubleValue = table.Column<double>(type: "double precision", nullable: true),
                    StringValue = table.Column<string>(type: "text", nullable: true),
                    DateValue = table.Column<DateOnly>(type: "date", nullable: true),
                    TimeSpanValue = table.Column<TimeSpan>(type: "interval", nullable: true),
                    DateTimeOffsetValue = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    StringValuesList = table.Column<List<string>>(type: "jsonb", nullable: true),
                    FieldDefinitionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_case_fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_test_case_fields_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_test_case_fields_field_definitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalTable: "field_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_test_case_fields_testcases_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "testcases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "testcaseruns",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AssignedToUserId = table.Column<long>(type: "bigint", nullable: true),
                    AssignedToUserName = table.Column<string>(type: "text", nullable: true),
                    Result = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<string>(type: "text", nullable: true),
                    MappedState = table.Column<int>(type: "integer", nullable: true),
                    Charter = table.Column<string>(type: "text", nullable: true),
                    ScriptType = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: true),
                    CallStack = table.Column<string>(type: "text", nullable: true),
                    SystemOut = table.Column<string>(type: "text", nullable: true),
                    SystemErr = table.Column<string>(type: "text", nullable: true),
                    Estimate = table.Column<int>(type: "integer", nullable: true),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    TestCaseId = table.Column<long>(type: "bigint", nullable: false),
                    TestRunId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testcaseruns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_testcaseruns_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testcaseruns_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testcaseruns_runs_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_testcaseruns_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_testcaseruns_testcases_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "testcases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Markdown = table.Column<string>(type: "text", nullable: true),
                    LoggedActionIcon = table.Column<string>(type: "text", nullable: true),
                    LoggedActionColor = table.Column<string>(type: "text", nullable: true),
                    LoggedAction = table.Column<string>(type: "text", nullable: true),
                    LoggedActionArgument = table.Column<string>(type: "text", nullable: true),
                    LocalIssueId = table.Column<long>(type: "bigint", nullable: true),
                    RequirementId = table.Column<long>(type: "bigint", nullable: true),
                    RequirementSpecificationId = table.Column<long>(type: "bigint", nullable: true),
                    TestCaseId = table.Column<long>(type: "bigint", nullable: true),
                    TestRunId = table.Column<long>(type: "bigint", nullable: true),
                    TestCaseRunId = table.Column<long>(type: "bigint", nullable: true),
                    TestSuiteId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_LocalIssues_LocalIssueId",
                        column: x => x.LocalIssueId,
                        principalTable: "LocalIssues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_requirements_RequirementId",
                        column: x => x.RequirementId,
                        principalTable: "requirements",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_runs_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "runs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_spec_RequirementSpecificationId",
                        column: x => x.RequirementSpecificationId,
                        principalTable: "spec",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_testcaseruns_TestCaseRunId",
                        column: x => x.TestCaseRunId,
                        principalTable: "testcaseruns",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_testcases_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "testcases",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_testsuites_TestSuiteId",
                        column: x => x.TestSuiteId,
                        principalTable: "testsuites",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LinkedIssues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    ExternalSystemName = table.Column<string>(type: "text", nullable: true),
                    ExternalSystemId = table.Column<long>(type: "bigint", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Author = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    MilestoneName = table.Column<string>(type: "text", nullable: true),
                    IssueType = table.Column<string>(type: "text", nullable: true),
                    ExternalDisplayId = table.Column<string>(type: "text", nullable: true),
                    LocalIssueId = table.Column<long>(type: "bigint", nullable: true),
                    TestCaseRunId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkedIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LinkedIssues_LocalIssues_LocalIssueId",
                        column: x => x.LocalIssueId,
                        principalTable: "LocalIssues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LinkedIssues_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LinkedIssues_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LinkedIssues_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LinkedIssues_testcaseruns_TestCaseRunId",
                        column: x => x.TestCaseRunId,
                        principalTable: "testcaseruns",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Metrics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MeterName = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    TestCaseRunId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Metrics_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Metrics_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Metrics_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Metrics_testcaseruns_TestCaseRunId",
                        column: x => x.TestCaseRunId,
                        principalTable: "testcaseruns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "test_case_run_fields",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestCaseRunId = table.Column<long>(type: "bigint", nullable: false),
                    TestRunId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Inherited = table.Column<bool>(type: "boolean", nullable: true),
                    BooleanValue = table.Column<bool>(type: "boolean", nullable: true),
                    LongValue = table.Column<long>(type: "bigint", nullable: true),
                    DoubleValue = table.Column<double>(type: "double precision", nullable: true),
                    StringValue = table.Column<string>(type: "text", nullable: true),
                    DateValue = table.Column<DateOnly>(type: "date", nullable: true),
                    TimeSpanValue = table.Column<TimeSpan>(type: "interval", nullable: true),
                    DateTimeOffsetValue = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    StringValuesList = table.Column<List<string>>(type: "jsonb", nullable: true),
                    FieldDefinitionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_case_run_fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_test_case_run_fields_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_test_case_run_fields_field_definitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalTable: "field_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_test_case_run_fields_runs_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_test_case_run_fields_testcaseruns_TestCaseRunId",
                        column: x => x.TestCaseRunId,
                        principalTable: "testcaseruns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_ApplicationUserId",
                table: "ApiKeys",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_TenantId",
                table: "ApiKeys",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchitecturalLayers_TeamId",
                table: "ArchitecturalLayers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchitecturalLayers_TenantId",
                table: "ArchitecturalLayers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchitecturalLayers_TestProjectId",
                table: "ArchitecturalLayers",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TeamId",
                table: "AspNetUsers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TestProjectId",
                table: "AspNetUsers",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_LocalIssueId",
                table: "Comments",
                column: "LocalIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_RequirementId",
                table: "Comments",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_RequirementSpecificationId",
                table: "Comments",
                column: "RequirementSpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TeamId",
                table: "Comments",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TenantId",
                table: "Comments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TestCaseId",
                table: "Comments",
                column: "TestCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TestCaseRunId",
                table: "Comments",
                column: "TestCaseRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TestProjectId",
                table: "Comments",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TestRunId",
                table: "Comments",
                column: "TestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TestSuiteId",
                table: "Comments",
                column: "TestSuiteId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitFiless_CommitId",
                table: "CommitFiless",
                column: "CommitId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitFiless_TeamId",
                table: "CommitFiless",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitFiless_TenantId",
                table: "CommitFiless",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitFiless_TestProjectId",
                table: "CommitFiless",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Commits_Reference",
                table: "Commits",
                column: "Reference");

            migrationBuilder.CreateIndex(
                name: "IX_Commits_RepositoryId",
                table: "Commits",
                column: "RepositoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Commits_Sha",
                table: "Commits",
                column: "Sha");

            migrationBuilder.CreateIndex(
                name: "IX_Commits_TeamId",
                table: "Commits",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Commits_TenantId",
                table: "Commits",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Commits_TestProjectId",
                table: "Commits",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_TeamId",
                table: "Components",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_TenantId",
                table: "Components",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_TestProjectId",
                table: "Components",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Dashboards_TeamId",
                table: "Dashboards",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Dashboards_TenantId",
                table: "Dashboards",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Dashboards_TestProjectId",
                table: "Dashboards",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_external_systems_TeamId",
                table: "external_systems",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_external_systems_TenantId",
                table: "external_systems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_external_systems_TestProjectId",
                table: "external_systems",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Features_TeamId",
                table: "Features",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Features_TenantId",
                table: "Features",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Features_TestProjectId",
                table: "Features",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_field_definitions_TeamId",
                table: "field_definitions",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_field_definitions_TenantId_IsDeleted",
                table: "field_definitions",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_field_definitions_TestProjectId",
                table: "field_definitions",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_heuristics_TeamId",
                table: "heuristics",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_heuristics_TenantId",
                table: "heuristics",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_heuristics_TestProjectId",
                table: "heuristics",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_issue_fields_FieldDefinitionId",
                table: "issue_fields",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_issue_fields_LocalIssueId",
                table: "issue_fields",
                column: "LocalIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_issue_fields_TenantId_LocalIssueId",
                table: "issue_fields",
                columns: new[] { "TenantId", "LocalIssueId" });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Guid",
                table: "Jobs",
                column: "Guid");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Status_Priority",
                table: "Jobs",
                columns: new[] { "Status", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_TeamId",
                table: "Jobs",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_TenantId",
                table: "Jobs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_TestProjectId",
                table: "Jobs",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_TestRunId",
                table: "Jobs",
                column: "TestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_TeamId",
                table: "Labels",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_TenantId",
                table: "Labels",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_TestProjectId",
                table: "Labels",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkedIssues_LocalIssueId",
                table: "LinkedIssues",
                column: "LocalIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkedIssues_TeamId",
                table: "LinkedIssues",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkedIssues_TenantId",
                table: "LinkedIssues",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkedIssues_TestCaseRunId",
                table: "LinkedIssues",
                column: "TestCaseRunId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkedIssues_TestProjectId",
                table: "LinkedIssues",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalIssues_ExternalSystemId_Created",
                table: "LocalIssues",
                columns: new[] { "ExternalSystemId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_LocalIssues_State_Created",
                table: "LocalIssues",
                columns: new[] { "State", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_LocalIssues_TeamId",
                table: "LocalIssues",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalIssues_TenantId",
                table: "LocalIssues",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalIssues_TestProjectId",
                table: "LocalIssues",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Metrics_TeamId",
                table: "Metrics",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Metrics_TenantId",
                table: "Metrics",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Metrics_TestCaseRunId",
                table: "Metrics",
                column: "TestCaseRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Metrics_TestProjectId",
                table: "Metrics",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_TeamId",
                table: "Milestones",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_TenantId",
                table: "Milestones",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_TestProjectId",
                table: "Milestones",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineJobs_PipelineId",
                table: "PipelineJobs",
                column: "PipelineId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineJobs_TeamId",
                table: "PipelineJobs",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineJobs_TenantId",
                table: "PipelineJobs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineJobs_TestProjectId",
                table: "PipelineJobs",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineJobs_TestRunId",
                table: "PipelineJobs",
                column: "TestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Pipelines_TeamId",
                table: "Pipelines",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Pipelines_TenantId",
                table: "Pipelines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Pipelines_TestProjectId",
                table: "Pipelines",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Pipelines_TestRunId",
                table: "Pipelines",
                column: "TestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSystems_TeamId",
                table: "ProductSystems",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSystems_TenantId",
                table: "ProductSystems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSystems_TestProjectId",
                table: "ProductSystems",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_projects_TeamId",
                table: "projects",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_projects_TenantId_Slug",
                table: "projects",
                columns: new[] { "TenantId", "Slug" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUserPermissions_ApplicationUserId",
                table: "ProjectUserPermissions",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUserPermissions_TeamId",
                table: "ProjectUserPermissions",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUserPermissions_TenantId",
                table: "ProjectUserPermissions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUserPermissions_TestProjectId",
                table: "ProjectUserPermissions",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_ExternalSystemId",
                table: "Repositories",
                column: "ExternalSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_TeamId",
                table: "Repositories",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_TenantId",
                table: "Repositories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_TestProjectId",
                table: "Repositories",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_Url",
                table: "Repositories",
                column: "Url");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_fields_FieldDefinitionId",
                table: "requirement_fields",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_fields_RequirementId",
                table: "requirement_fields",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_fields_TenantId_RequirementId",
                table: "requirement_fields",
                columns: new[] { "TenantId", "RequirementId" });

            migrationBuilder.CreateIndex(
                name: "IX_requirement_test_links_RequirementId",
                table: "requirement_test_links",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_test_links_RequirementSpecificationId",
                table: "requirement_test_links",
                column: "RequirementSpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_test_links_TenantId",
                table: "requirement_test_links",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_test_links_TestCaseId",
                table: "requirement_test_links",
                column: "TestCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_requirements_Created",
                table: "requirements",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_requirements_ExternalId",
                table: "requirements",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_requirements_Name",
                table: "requirements",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_requirements_RequirementSpecificationFolderId",
                table: "requirements",
                column: "RequirementSpecificationFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_requirements_RequirementSpecificationId",
                table: "requirements",
                column: "RequirementSpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_requirements_TeamId",
                table: "requirements",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_requirements_TenantId_Slug",
                table: "requirements",
                columns: new[] { "TenantId", "Slug" });

            migrationBuilder.CreateIndex(
                name: "IX_requirements_TestProjectId",
                table: "requirements",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_TenantId",
                table: "RolePermissions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Runners_TeamId",
                table: "Runners",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Runners_TenantId",
                table: "Runners",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Runners_TestProjectId",
                table: "Runners",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_runs_FolderId",
                table: "runs",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_runs_TeamId",
                table: "runs",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_runs_TenantId_Created",
                table: "runs",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_runs_TestEnvironmentId",
                table: "runs",
                column: "TestEnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_runs_TestProjectId",
                table: "runs",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_spec_ExternalId",
                table: "spec",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_spec_TeamId",
                table: "spec",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_spec_TenantId_Created",
                table: "spec",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_spec_TenantId_Slug",
                table: "spec",
                columns: new[] { "TenantId", "Slug" });

            migrationBuilder.CreateIndex(
                name: "IX_spec_TestProjectId",
                table: "spec",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_spec__folders_ParentId",
                table: "spec__folders",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_spec__folders_RequirementSpecificationId",
                table: "spec__folders",
                column: "RequirementSpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_spec__folders_TeamId",
                table: "spec__folders",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_spec__folders_TenantId_Created",
                table: "spec__folders",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_spec__folders_TestProjectId",
                table: "spec__folders",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_steps_TeamId",
                table: "steps",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_steps_TenantId",
                table: "steps",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_steps_TestCaseId",
                table: "steps",
                column: "TestCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_steps_TestProjectId",
                table: "steps",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_teams_TenantId",
                table: "teams",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_test_case_fields_FieldDefinitionId",
                table: "test_case_fields",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_test_case_fields_TenantId_TestCaseId",
                table: "test_case_fields",
                columns: new[] { "TenantId", "TestCaseId" });

            migrationBuilder.CreateIndex(
                name: "IX_test_case_fields_TestCaseId",
                table: "test_case_fields",
                column: "TestCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_test_case_run_fields_FieldDefinitionId",
                table: "test_case_run_fields",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_test_case_run_fields_TenantId_TestCaseRunId",
                table: "test_case_run_fields",
                columns: new[] { "TenantId", "TestCaseRunId" });

            migrationBuilder.CreateIndex(
                name: "IX_test_case_run_fields_TestCaseRunId",
                table: "test_case_run_fields",
                column: "TestCaseRunId");

            migrationBuilder.CreateIndex(
                name: "IX_test_case_run_fields_TestRunId",
                table: "test_case_run_fields",
                column: "TestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_test_run_fields_FieldDefinitionId",
                table: "test_run_fields",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_test_run_fields_TenantId",
                table: "test_run_fields",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_test_run_fields_TestRunId",
                table: "test_run_fields",
                column: "TestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAccounts_TenantId",
                table: "TestAccounts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_testcaseruns_Created",
                table: "testcaseruns",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_testcaseruns_Name",
                table: "testcaseruns",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_testcaseruns_TeamId",
                table: "testcaseruns",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_testcaseruns_TenantId",
                table: "testcaseruns",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_testcaseruns_TestCaseId",
                table: "testcaseruns",
                column: "TestCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_testcaseruns_TestProjectId",
                table: "testcaseruns",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_testcaseruns_TestRunId",
                table: "testcaseruns",
                column: "TestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_testcases_Created",
                table: "testcases",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_testcases_Name",
                table: "testcases",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_testcases_TeamId",
                table: "testcases",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_testcases_TenantId_Slug",
                table: "testcases",
                columns: new[] { "TenantId", "Slug" });

            migrationBuilder.CreateIndex(
                name: "IX_testcases_TenantId_TestProjectId_AutomationAssembly_ClassNa~",
                table: "testcases",
                columns: new[] { "TenantId", "TestProjectId", "AutomationAssembly", "ClassName", "Module", "Method" });

            migrationBuilder.CreateIndex(
                name: "IX_testcases_TenantId_TestProjectId_ExternalId",
                table: "testcases",
                columns: new[] { "TenantId", "TestProjectId", "ExternalId" });

            migrationBuilder.CreateIndex(
                name: "IX_testcases_TestProjectId",
                table: "testcases",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_testcases_TestSuiteFolderId",
                table: "testcases",
                column: "TestSuiteFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_TestEnvironments_TeamId",
                table: "TestEnvironments",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TestEnvironments_TenantId",
                table: "TestEnvironments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TestEnvironments_TestProjectId",
                table: "TestEnvironments",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_testlab__folders_ParentId",
                table: "testlab__folders",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_testlab__folders_TeamId",
                table: "testlab__folders",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_testlab__folders_TenantId",
                table: "testlab__folders",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_testlab__folders_TestProjectId",
                table: "testlab__folders",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_testrepository__folders_ParentId",
                table: "testrepository__folders",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_testrepository__folders_TeamId",
                table: "testrepository__folders",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_testrepository__folders_TenantId",
                table: "testrepository__folders",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_testrepository__folders_TestProjectId",
                table: "testrepository__folders",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResources_TenantId",
                table: "TestResources",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_testsuite__folders_ParentId",
                table: "testsuite__folders",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_testsuite__folders_TeamId",
                table: "testsuite__folders",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_testsuite__folders_TenantId_Created",
                table: "testsuite__folders",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_testsuite__folders_TestProjectId",
                table: "testsuite__folders",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_testsuite__folders_TestSuiteId",
                table: "testsuite__folders",
                column: "TestSuiteId");

            migrationBuilder.CreateIndex(
                name: "IX_testsuites_FolderId",
                table: "testsuites",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_testsuites_TeamId",
                table: "testsuites",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_testsuites_TenantId_Created",
                table: "testsuites",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_testsuites_TenantId_Slug",
                table: "testsuites",
                columns: new[] { "TenantId", "Slug" });

            migrationBuilder.CreateIndex(
                name: "IX_testsuites_TestProjectId",
                table: "testsuites",
                column: "TestProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropTable(
                name: "ArchitecturalLayers");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "CommitFiless");

            migrationBuilder.DropTable(
                name: "Components");

            migrationBuilder.DropTable(
                name: "Dashboards");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "GlobalSettings");

            migrationBuilder.DropTable(
                name: "heuristics");

            migrationBuilder.DropTable(
                name: "issue_fields");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Labels");

            migrationBuilder.DropTable(
                name: "LinkedIssues");

            migrationBuilder.DropTable(
                name: "Metrics");

            migrationBuilder.DropTable(
                name: "Milestones");

            migrationBuilder.DropTable(
                name: "PipelineJobs");

            migrationBuilder.DropTable(
                name: "ProductSystems");

            migrationBuilder.DropTable(
                name: "ProjectUserPermissions");

            migrationBuilder.DropTable(
                name: "requirement_fields");

            migrationBuilder.DropTable(
                name: "requirement_test_links");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Runners");

            migrationBuilder.DropTable(
                name: "steps");

            migrationBuilder.DropTable(
                name: "test_case_fields");

            migrationBuilder.DropTable(
                name: "test_case_run_fields");

            migrationBuilder.DropTable(
                name: "test_run_fields");

            migrationBuilder.DropTable(
                name: "TestAccounts");

            migrationBuilder.DropTable(
                name: "TestResources");

            migrationBuilder.DropTable(
                name: "UserPreferences");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Commits");

            migrationBuilder.DropTable(
                name: "LocalIssues");

            migrationBuilder.DropTable(
                name: "Pipelines");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "requirements");

            migrationBuilder.DropTable(
                name: "testcaseruns");

            migrationBuilder.DropTable(
                name: "field_definitions");

            migrationBuilder.DropTable(
                name: "Repositories");

            migrationBuilder.DropTable(
                name: "spec__folders");

            migrationBuilder.DropTable(
                name: "runs");

            migrationBuilder.DropTable(
                name: "testcases");

            migrationBuilder.DropTable(
                name: "external_systems");

            migrationBuilder.DropTable(
                name: "spec");

            migrationBuilder.DropTable(
                name: "TestEnvironments");

            migrationBuilder.DropTable(
                name: "testlab__folders");

            migrationBuilder.DropTable(
                name: "testsuite__folders");

            migrationBuilder.DropTable(
                name: "testsuites");

            migrationBuilder.DropTable(
                name: "testrepository__folders");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
