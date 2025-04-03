using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class RequirementLinkExternalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequirementExternalId",
                table: "requirement_test_links",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RequirementSpecificationId",
                table: "requirement_test_links",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_requirement_test_links_RequirementSpecificationId",
                table: "requirement_test_links",
                column: "RequirementSpecificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_requirement_test_links_spec_RequirementSpecificationId",
                table: "requirement_test_links",
                column: "RequirementSpecificationId",
                principalTable: "spec",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_requirement_test_links_spec_RequirementSpecificationId",
                table: "requirement_test_links");

            migrationBuilder.DropIndex(
                name: "IX_requirement_test_links_RequirementSpecificationId",
                table: "requirement_test_links");

            migrationBuilder.DropColumn(
                name: "RequirementExternalId",
                table: "requirement_test_links");

            migrationBuilder.DropColumn(
                name: "RequirementSpecificationId",
                table: "requirement_test_links");
        }
    }
}
