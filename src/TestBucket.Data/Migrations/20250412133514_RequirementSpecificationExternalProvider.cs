using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class RequirementSpecificationExternalProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "spec",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalProvider",
                table: "spec",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalProvider",
                table: "requirements",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_spec_ExternalId",
                table: "spec",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_requirements_ExternalId",
                table: "requirements",
                column: "ExternalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_spec_ExternalId",
                table: "spec");

            migrationBuilder.DropIndex(
                name: "IX_requirements_ExternalId",
                table: "requirements");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "spec");

            migrationBuilder.DropColumn(
                name: "ExternalProvider",
                table: "spec");

            migrationBuilder.DropColumn(
                name: "ExternalProvider",
                table: "requirements");
        }
    }
}
