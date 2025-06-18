using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExploratoryCharter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Charter",
                table: "testcaseruns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Estimate",
                table: "testcaseruns",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScriptType",
                table: "testcaseruns",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Charter",
                table: "testcaseruns");

            migrationBuilder.DropColumn(
                name: "Estimate",
                table: "testcaseruns");

            migrationBuilder.DropColumn(
                name: "ScriptType",
                table: "testcaseruns");
        }
    }
}
