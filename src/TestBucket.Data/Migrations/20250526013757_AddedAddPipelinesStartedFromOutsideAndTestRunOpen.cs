using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedAddPipelinesStartedFromOutsideAndTestRunOpen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AddPipelinesStartedFromOutside",
                table: "testsuites",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Open",
                table: "runs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddPipelinesStartedFromOutside",
                table: "testsuites");

            migrationBuilder.DropColumn(
                name: "Open",
                table: "runs");
        }
    }
}
