using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class DockEnumAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExplorerDock",
                table: "UserPreferences",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Dock",
                table: "field_definitions",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExplorerDock",
                table: "UserPreferences");

            migrationBuilder.DropColumn(
                name: "Dock",
                table: "field_definitions");
        }
    }
}
