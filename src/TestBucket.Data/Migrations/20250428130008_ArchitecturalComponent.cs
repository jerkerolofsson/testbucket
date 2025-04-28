using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class ArchitecturalComponent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Component_Commits_CommitId",
                table: "Component");

            migrationBuilder.DropForeignKey(
                name: "FK_Component_Tenants_TenantId",
                table: "Component");

            migrationBuilder.DropForeignKey(
                name: "FK_Component_projects_TestProjectId",
                table: "Component");

            migrationBuilder.DropForeignKey(
                name: "FK_Component_teams_TeamId",
                table: "Component");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Component",
                table: "Component");

            migrationBuilder.RenameTable(
                name: "Component",
                newName: "Components");

            migrationBuilder.RenameIndex(
                name: "IX_Component_TestProjectId",
                table: "Components",
                newName: "IX_Components_TestProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Component_TenantId",
                table: "Components",
                newName: "IX_Components_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_Component_TeamId",
                table: "Components",
                newName: "IX_Components_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_Component_CommitId",
                table: "Components",
                newName: "IX_Components_CommitId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Components",
                table: "Components",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Commits_CommitId",
                table: "Components",
                column: "CommitId",
                principalTable: "Commits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Tenants_TenantId",
                table: "Components",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Components_projects_TestProjectId",
                table: "Components",
                column: "TestProjectId",
                principalTable: "projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Components_teams_TeamId",
                table: "Components",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Components_Commits_CommitId",
                table: "Components");

            migrationBuilder.DropForeignKey(
                name: "FK_Components_Tenants_TenantId",
                table: "Components");

            migrationBuilder.DropForeignKey(
                name: "FK_Components_projects_TestProjectId",
                table: "Components");

            migrationBuilder.DropForeignKey(
                name: "FK_Components_teams_TeamId",
                table: "Components");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Components",
                table: "Components");

            migrationBuilder.RenameTable(
                name: "Components",
                newName: "Component");

            migrationBuilder.RenameIndex(
                name: "IX_Components_TestProjectId",
                table: "Component",
                newName: "IX_Component_TestProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Components_TenantId",
                table: "Component",
                newName: "IX_Component_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_Components_TeamId",
                table: "Component",
                newName: "IX_Component_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_Components_CommitId",
                table: "Component",
                newName: "IX_Component_CommitId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Component",
                table: "Component",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Component_Commits_CommitId",
                table: "Component",
                column: "CommitId",
                principalTable: "Commits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Component_Tenants_TenantId",
                table: "Component",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Component_projects_TestProjectId",
                table: "Component",
                column: "TestProjectId",
                principalTable: "projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Component_teams_TeamId",
                table: "Component",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");
        }
    }
}
