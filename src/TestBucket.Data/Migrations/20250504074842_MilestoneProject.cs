using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class MilestoneProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Created",
                table: "Milestones",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Milestones",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Modified",
                table: "Milestones",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Milestones",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TeamId",
                table: "Milestones",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Milestones",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TestProjectId",
                table: "Milestones",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_TeamId",
                table: "Milestones",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_TenantId",
                table: "Milestones",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_TestProjectId",
                table: "Milestones",
                column: "TestProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Milestones_Tenants_TenantId",
                table: "Milestones",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Milestones_projects_TestProjectId",
                table: "Milestones",
                column: "TestProjectId",
                principalTable: "projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Milestones_teams_TeamId",
                table: "Milestones",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Milestones_Tenants_TenantId",
                table: "Milestones");

            migrationBuilder.DropForeignKey(
                name: "FK_Milestones_projects_TestProjectId",
                table: "Milestones");

            migrationBuilder.DropForeignKey(
                name: "FK_Milestones_teams_TeamId",
                table: "Milestones");

            migrationBuilder.DropIndex(
                name: "IX_Milestones_TeamId",
                table: "Milestones");

            migrationBuilder.DropIndex(
                name: "IX_Milestones_TenantId",
                table: "Milestones");

            migrationBuilder.DropIndex(
                name: "IX_Milestones_TestProjectId",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "TestProjectId",
                table: "Milestones");
        }
    }
}
