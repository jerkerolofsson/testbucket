using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TestBucket.Domain.AI.Mcp.Models;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMcpServerRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "McpServerRegistrations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Configuration = table.Column<McpServerConfiguration>(type: "jsonb", nullable: false),
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
                    table.PrimaryKey("PK_McpServerRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_McpServerRegistrations_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_McpServerRegistrations_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_McpServerRegistrations_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "McpServerUserInputs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    McpServerRegistrationId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_McpServerUserInputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_McpServerUserInputs_McpServerRegistrations_McpServerRegistr~",
                        column: x => x.McpServerRegistrationId,
                        principalTable: "McpServerRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_McpServerUserInputs_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_McpServerUserInputs_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_McpServerUserInputs_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_McpServerRegistrations_TeamId",
                table: "McpServerRegistrations",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_McpServerRegistrations_TenantId",
                table: "McpServerRegistrations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_McpServerRegistrations_TestProjectId",
                table: "McpServerRegistrations",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_McpServerUserInputs_McpServerRegistrationId",
                table: "McpServerUserInputs",
                column: "McpServerRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_McpServerUserInputs_TeamId",
                table: "McpServerUserInputs",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_McpServerUserInputs_TenantId",
                table: "McpServerUserInputs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_McpServerUserInputs_TestProjectId",
                table: "McpServerUserInputs",
                column: "TestProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "McpServerUserInputs");

            migrationBuilder.DropTable(
                name: "McpServerRegistrations");
        }
    }
}
