using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class FileResourceRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RequirementId",
                table: "Files",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RequirementSpecificationId",
                table: "Files",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequirementId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "RequirementSpecificationId",
                table: "Files");
        }
    }
}
