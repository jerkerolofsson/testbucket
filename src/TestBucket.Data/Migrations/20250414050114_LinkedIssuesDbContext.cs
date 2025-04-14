using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class LinkedIssuesDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinkedIssue_Tenants_TenantId",
                table: "LinkedIssue");

            migrationBuilder.DropForeignKey(
                name: "FK_LinkedIssue_projects_TestProjectId",
                table: "LinkedIssue");

            migrationBuilder.DropForeignKey(
                name: "FK_LinkedIssue_teams_TeamId",
                table: "LinkedIssue");

            migrationBuilder.DropForeignKey(
                name: "FK_LinkedIssue_testcaseruns_TestCaseRunId",
                table: "LinkedIssue");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LinkedIssue",
                table: "LinkedIssue");

            migrationBuilder.RenameTable(
                name: "LinkedIssue",
                newName: "LinkedIssues");

            migrationBuilder.RenameIndex(
                name: "IX_LinkedIssue_TestProjectId",
                table: "LinkedIssues",
                newName: "IX_LinkedIssues_TestProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_LinkedIssue_TestCaseRunId",
                table: "LinkedIssues",
                newName: "IX_LinkedIssues_TestCaseRunId");

            migrationBuilder.RenameIndex(
                name: "IX_LinkedIssue_TenantId",
                table: "LinkedIssues",
                newName: "IX_LinkedIssues_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_LinkedIssue_TeamId",
                table: "LinkedIssues",
                newName: "IX_LinkedIssues_TeamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LinkedIssues",
                table: "LinkedIssues",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkedIssues_Tenants_TenantId",
                table: "LinkedIssues",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkedIssues_projects_TestProjectId",
                table: "LinkedIssues",
                column: "TestProjectId",
                principalTable: "projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkedIssues_teams_TeamId",
                table: "LinkedIssues",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkedIssues_testcaseruns_TestCaseRunId",
                table: "LinkedIssues",
                column: "TestCaseRunId",
                principalTable: "testcaseruns",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinkedIssues_Tenants_TenantId",
                table: "LinkedIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_LinkedIssues_projects_TestProjectId",
                table: "LinkedIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_LinkedIssues_teams_TeamId",
                table: "LinkedIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_LinkedIssues_testcaseruns_TestCaseRunId",
                table: "LinkedIssues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LinkedIssues",
                table: "LinkedIssues");

            migrationBuilder.RenameTable(
                name: "LinkedIssues",
                newName: "LinkedIssue");

            migrationBuilder.RenameIndex(
                name: "IX_LinkedIssues_TestProjectId",
                table: "LinkedIssue",
                newName: "IX_LinkedIssue_TestProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_LinkedIssues_TestCaseRunId",
                table: "LinkedIssue",
                newName: "IX_LinkedIssue_TestCaseRunId");

            migrationBuilder.RenameIndex(
                name: "IX_LinkedIssues_TenantId",
                table: "LinkedIssue",
                newName: "IX_LinkedIssue_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_LinkedIssues_TeamId",
                table: "LinkedIssue",
                newName: "IX_LinkedIssue_TeamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LinkedIssue",
                table: "LinkedIssue",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkedIssue_Tenants_TenantId",
                table: "LinkedIssue",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkedIssue_projects_TestProjectId",
                table: "LinkedIssue",
                column: "TestProjectId",
                principalTable: "projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkedIssue_teams_TeamId",
                table: "LinkedIssue",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkedIssue_testcaseruns_TestCaseRunId",
                table: "LinkedIssue",
                column: "TestCaseRunId",
                principalTable: "testcaseruns",
                principalColumn: "Id");
        }
    }
}
