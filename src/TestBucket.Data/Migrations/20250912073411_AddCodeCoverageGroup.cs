using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCodeCoverageGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CodeCoverageGroups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Group = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ResourceIds = table.Column<List<long>>(type: "jsonb", nullable: true),
                    ClassCount = table.Column<long>(type: "bigint", nullable: false),
                    CoveredClassCount = table.Column<long>(type: "bigint", nullable: false),
                    MethodCount = table.Column<long>(type: "bigint", nullable: false),
                    CoveredMethodCount = table.Column<long>(type: "bigint", nullable: false),
                    CoveredLineCount = table.Column<long>(type: "bigint", nullable: false),
                    LineCount = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_CodeCoverageGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CodeCoverageGroups_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CodeCoverageGroups_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CodeCoverageGroups_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CodeCoverageGroups_TeamId",
                table: "CodeCoverageGroups",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_CodeCoverageGroups_TenantId_Group_Name",
                table: "CodeCoverageGroups",
                columns: new[] { "TenantId", "Group", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_CodeCoverageGroups_TestProjectId",
                table: "CodeCoverageGroups",
                column: "TestProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CodeCoverageGroups");
        }
    }
}
