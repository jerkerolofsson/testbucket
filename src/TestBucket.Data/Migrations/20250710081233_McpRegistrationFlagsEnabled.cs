using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class McpRegistrationFlagsEnabled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "McpServerRegistrations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Locked",
                table: "McpServerRegistrations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PublicForProject",
                table: "McpServerRegistrations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "McpServerRegistrations");

            migrationBuilder.DropColumn(
                name: "Locked",
                table: "McpServerRegistrations");

            migrationBuilder.DropColumn(
                name: "PublicForProject",
                table: "McpServerRegistrations");
        }
    }
}
