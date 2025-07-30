using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddArchiveToTestRun : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "runs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_runs_TenantId_TestProjectId_Archived",
                table: "runs",
                columns: new[] { "TenantId", "TestProjectId", "Archived" });

            migrationBuilder.CreateIndex(
                name: "IX_runs_TenantId_TestProjectId_Open",
                table: "runs",
                columns: new[] { "TenantId", "TestProjectId", "Open" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_runs_TenantId_TestProjectId_Archived",
                table: "runs");

            migrationBuilder.DropIndex(
                name: "IX_runs_TenantId_TestProjectId_Open",
                table: "runs");

            migrationBuilder.DropColumn(
                name: "Archived",
                table: "runs");
        }
    }
}
