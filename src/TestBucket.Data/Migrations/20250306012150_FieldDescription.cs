using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class FieldDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "field_definitions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowDescription",
                table: "field_definitions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "field_definitions");

            migrationBuilder.DropColumn(
                name: "ShowDescription",
                table: "field_definitions");
        }
    }
}
