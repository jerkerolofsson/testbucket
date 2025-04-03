using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class TestRunCiCd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CiCdRef",
                table: "runs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CiCdSystem",
                table: "runs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CiCdRef",
                table: "runs");

            migrationBuilder.DropColumn(
                name: "CiCdSystem",
                table: "runs");
        }
    }
}
