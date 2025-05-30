using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class FileResourceForIssuesAndCodeComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ComponentId",
                table: "Files",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FeatureId",
                table: "Files",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LayerId",
                table: "Files",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LocalIssueId",
                table: "Files",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SystemId",
                table: "Files",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComponentId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "FeatureId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "LayerId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "LocalIssueId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "SystemId",
                table: "Files");
        }
    }
}
