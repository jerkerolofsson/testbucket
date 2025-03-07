using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class TestRunTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TeamId",
                table: "runs",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_runs_TeamId",
                table: "runs",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_runs_teams_TeamId",
                table: "runs",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_runs_teams_TeamId",
                table: "runs");

            migrationBuilder.DropIndex(
                name: "IX_runs_TeamId",
                table: "runs");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "runs");
        }
    }
}
