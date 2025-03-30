using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class RunTestEnvironment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TestEnvironmentId",
                table: "runs",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_runs_TestEnvironmentId",
                table: "runs",
                column: "TestEnvironmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_runs_TestEnvironments_TestEnvironmentId",
                table: "runs",
                column: "TestEnvironmentId",
                principalTable: "TestEnvironments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_runs_TestEnvironments_TestEnvironmentId",
                table: "runs");

            migrationBuilder.DropIndex(
                name: "IX_runs_TestEnvironmentId",
                table: "runs");

            migrationBuilder.DropColumn(
                name: "TestEnvironmentId",
                table: "runs");
        }
    }
}
