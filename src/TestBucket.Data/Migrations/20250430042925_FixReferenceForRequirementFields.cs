using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixReferenceForRequirementFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_test_case_fields_requirements_RequirementId",
                table: "test_case_fields");

            migrationBuilder.DropIndex(
                name: "IX_test_case_fields_RequirementId",
                table: "test_case_fields");

            migrationBuilder.DropColumn(
                name: "RequirementId",
                table: "test_case_fields");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RequirementId",
                table: "test_case_fields",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_test_case_fields_RequirementId",
                table: "test_case_fields",
                column: "RequirementId");

            migrationBuilder.AddForeignKey(
                name: "FK_test_case_fields_requirements_RequirementId",
                table: "test_case_fields",
                column: "RequirementId",
                principalTable: "requirements",
                principalColumn: "Id");
        }
    }
}
