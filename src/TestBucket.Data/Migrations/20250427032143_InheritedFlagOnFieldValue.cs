using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class InheritedFlagOnFieldValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Inherited",
                table: "test_run_fields",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Inherited",
                table: "test_case_run_fields",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Inherited",
                table: "test_case_fields",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Inherited",
                table: "requirement_fields",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Inherited",
                table: "test_run_fields");

            migrationBuilder.DropColumn(
                name: "Inherited",
                table: "test_case_run_fields");

            migrationBuilder.DropColumn(
                name: "Inherited",
                table: "test_case_fields");

            migrationBuilder.DropColumn(
                name: "Inherited",
                table: "requirement_fields");
        }
    }
}
