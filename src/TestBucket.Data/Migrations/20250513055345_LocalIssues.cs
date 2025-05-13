using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class LocalIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LocalIssueId",
                table: "LinkedIssues",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LocalIssues",
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

            migrationBuilder.CreateIndex(
                name: "IX_LinkedIssues_LocalIssueId",
                table: "LinkedIssues",
                column: "LocalIssueId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_LinkedIssues_LocalIssues_LocalIssueId",
                table: "LinkedIssues",
                column: "LocalIssueId",
                principalTable: "LocalIssues",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinkedIssues_LocalIssues_LocalIssueId",
                table: "LinkedIssues");

            migrationBuilder.DropTable(
                name: "LocalIssues");

            migrationBuilder.DropIndex(
                name: "IX_LinkedIssues_LocalIssueId",
                table: "LinkedIssues");

            migrationBuilder.DropColumn(
                name: "LocalIssueId",
                table: "LinkedIssues");
        }
    }
}
