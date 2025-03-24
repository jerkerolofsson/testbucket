using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStringValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StringValues",
                table: "test_run_fields");

            migrationBuilder.DropColumn(
                name: "StringValues",
                table: "test_case_run_fields");

            migrationBuilder.DropColumn(
                name: "StringValues",
                table: "test_case_fields");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "StringValues",
                table: "test_run_fields",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "StringValues",
                table: "test_case_run_fields",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "StringValues",
                table: "test_case_fields",
                type: "text[]",
                nullable: true);
        }
    }
}
