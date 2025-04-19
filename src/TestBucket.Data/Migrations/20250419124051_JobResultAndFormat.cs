using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class JobResultAndFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Format",
                table: "Jobs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "Jobs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Format",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Result",
                table: "Jobs");
        }
    }
}
