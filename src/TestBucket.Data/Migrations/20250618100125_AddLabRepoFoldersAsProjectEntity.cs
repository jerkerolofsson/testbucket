using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLabRepoFoldersAsProjectEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Created",
                table: "testrepository__folders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "testrepository__folders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Modified",
                table: "testrepository__folders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "testrepository__folders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TeamId",
                table: "testrepository__folders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "testrepository__folders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TestProjectId",
                table: "testrepository__folders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Created",
                table: "testlab__folders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "testlab__folders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Modified",
                table: "testlab__folders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "testlab__folders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TeamId",
                table: "testlab__folders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "testlab__folders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TestProjectId",
                table: "testlab__folders",
                type: "bigint",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_testlab__folders_Tenants_TenantId",
                table: "testlab__folders",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_testlab__folders_projects_TestProjectId",
                table: "testlab__folders",
                column: "TestProjectId",
                principalTable: "projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_testlab__folders_teams_TeamId",
                table: "testlab__folders",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_testrepository__folders_Tenants_TenantId",
                table: "testrepository__folders",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_testrepository__folders_projects_TestProjectId",
                table: "testrepository__folders",
                column: "TestProjectId",
                principalTable: "projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_testrepository__folders_teams_TeamId",
                table: "testrepository__folders",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_testlab__folders_Tenants_TenantId",
                table: "testlab__folders");

            migrationBuilder.DropForeignKey(
                name: "FK_testlab__folders_projects_TestProjectId",
                table: "testlab__folders");

            migrationBuilder.DropForeignKey(
                name: "FK_testlab__folders_teams_TeamId",
                table: "testlab__folders");

            migrationBuilder.DropForeignKey(
                name: "FK_testrepository__folders_Tenants_TenantId",
                table: "testrepository__folders");

            migrationBuilder.DropForeignKey(
                name: "FK_testrepository__folders_projects_TestProjectId",
                table: "testrepository__folders");

            migrationBuilder.DropForeignKey(
                name: "FK_testrepository__folders_teams_TeamId",
                table: "testrepository__folders");

            migrationBuilder.DropIndex(
                name: "IX_testrepository__folders_TeamId",
                table: "testrepository__folders");

            migrationBuilder.DropIndex(
                name: "IX_testrepository__folders_TenantId",
                table: "testrepository__folders");

            migrationBuilder.DropIndex(
                name: "IX_testrepository__folders_TestProjectId",
                table: "testrepository__folders");

            migrationBuilder.DropIndex(
                name: "IX_testlab__folders_TeamId",
                table: "testlab__folders");

            migrationBuilder.DropIndex(
                name: "IX_testlab__folders_TenantId",
                table: "testlab__folders");

            migrationBuilder.DropIndex(
                name: "IX_testlab__folders_TestProjectId",
                table: "testlab__folders");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "testrepository__folders");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "testrepository__folders");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "testrepository__folders");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "testrepository__folders");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "testrepository__folders");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "testrepository__folders");

            migrationBuilder.DropColumn(
                name: "TestProjectId",
                table: "testrepository__folders");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "testlab__folders");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "testlab__folders");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "testlab__folders");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "testlab__folders");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "testlab__folders");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "testlab__folders");

            migrationBuilder.DropColumn(
                name: "TestProjectId",
                table: "testlab__folders");
        }
    }
}
