using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class IssueComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LocalIssueId",
                table: "Comments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_LocalIssueId",
                table: "Comments",
                column: "LocalIssueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_LocalIssues_LocalIssueId",
                table: "Comments",
                column: "LocalIssueId",
                principalTable: "LocalIssues",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_LocalIssues_LocalIssueId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_LocalIssueId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "LocalIssueId",
                table: "Comments");
        }
    }
}
