using Microsoft.EntityFrameworkCore.Migrations;
using TestBucket.Domain.Keyboard;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class KeyboardBindingsInUserPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<KeyboardBindings>(
                name: "KeyboardBindings",
                table: "UserPreferences",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyboardBindings",
                table: "UserPreferences");
        }
    }
}
