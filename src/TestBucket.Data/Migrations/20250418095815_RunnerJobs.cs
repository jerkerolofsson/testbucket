using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class RunnerJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jobs");
        }
    }
}
