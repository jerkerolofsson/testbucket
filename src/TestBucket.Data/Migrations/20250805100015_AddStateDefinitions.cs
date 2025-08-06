using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Testing.States;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStateDefinitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StateDefinitions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IssueStates = table.Column<List<IssueState>>(type: "jsonb", nullable: false),
                    RequirementStates = table.Column<List<RequirementState>>(type: "jsonb", nullable: false),
                    TestCaseStates = table.Column<List<TestState>>(type: "jsonb", nullable: false),
                    TestCaseRunStates = table.Column<List<TestState>>(type: "jsonb", nullable: false),
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
                    table.PrimaryKey("PK_StateDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StateDefinitions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StateDefinitions_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StateDefinitions_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StateDefinitions_TeamId",
                table: "StateDefinitions",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_StateDefinitions_TenantId",
                table: "StateDefinitions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_StateDefinitions_TestProjectId",
                table: "StateDefinitions",
                column: "TestProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StateDefinitions");
        }
    }
}
