using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class MultipleExternalSystems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ExternalSystemId",
                table: "testsuites",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ExternalSystemId",
                table: "runs",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalSystemId",
                table: "testsuites");

            migrationBuilder.DropColumn(
                name: "ExternalSystemId",
                table: "runs");
        }
    }
}
