using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class Comments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Markdown = table.Column<string>(type: "text", nullable: true),
                    LoggedAction = table.Column<string>(type: "text", nullable: true),
                    LoggedActionArgument = table.Column<string>(type: "text", nullable: true),
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");
        }
    }
}
