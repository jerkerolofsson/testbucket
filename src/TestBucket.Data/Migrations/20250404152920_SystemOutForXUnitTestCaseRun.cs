using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class SystemOutForXUnitTestCaseRun : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SystemErr",
                table: "testcaseruns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SystemOut",
                table: "testcaseruns",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SystemErr",
                table: "testcaseruns");

            migrationBuilder.DropColumn(
                name: "SystemOut",
                table: "testcaseruns");
        }
    }
}
