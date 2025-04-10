using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalSystemCapability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnableMilestones",
                table: "external_systems");

            migrationBuilder.DropColumn(
                name: "EnableReleases",
                table: "external_systems");

            migrationBuilder.AddColumn<int>(
                name: "EnabledCapabilities",
                table: "external_systems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SupportedCapabilities",
                table: "external_systems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnabledCapabilities",
                table: "external_systems");

            migrationBuilder.DropColumn(
                name: "SupportedCapabilities",
                table: "external_systems");

            migrationBuilder.AddColumn<bool>(
                name: "EnableMilestones",
                table: "external_systems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableReleases",
                table: "external_systems",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
