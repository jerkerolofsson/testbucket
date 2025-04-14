using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class LinkedIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LinkedIssue",
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
                    table.PrimaryKey("PK_LinkedIssue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LinkedIssue_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LinkedIssue_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LinkedIssue_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LinkedIssue_testcaseruns_TestCaseRunId",
                        column: x => x.TestCaseRunId,
                        principalTable: "testcaseruns",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LinkedIssue_TeamId",
                table: "LinkedIssue",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkedIssue_TenantId",
                table: "LinkedIssue",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkedIssue_TestCaseRunId",
                table: "LinkedIssue",
                column: "TestCaseRunId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkedIssue_TestProjectId",
                table: "LinkedIssue",
                column: "TestProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LinkedIssue");
        }
    }
}
