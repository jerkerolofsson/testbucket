using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class JsonBStringValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StringArrayValue",
                table: "test_run_fields",
                newName: "StringValues");

            migrationBuilder.RenameColumn(
                name: "StringArrayValue",
                table: "test_case_run_fields",
                newName: "StringValues");

            migrationBuilder.RenameColumn(
                name: "StringArrayValue",
                table: "test_case_fields",
                newName: "StringValues");

            migrationBuilder.AddColumn<bool>(
                name: "UseClassifier",
                table: "field_definitions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseClassifier",
                table: "field_definitions");

            migrationBuilder.RenameColumn(
                name: "StringValues",
                table: "test_run_fields",
                newName: "StringArrayValue");

            migrationBuilder.RenameColumn(
                name: "StringValues",
                table: "test_case_run_fields",
                newName: "StringArrayValue");

            migrationBuilder.RenameColumn(
                name: "StringValues",
                table: "test_case_fields",
                newName: "StringArrayValue");
        }
    }
}
